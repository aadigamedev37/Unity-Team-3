using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// SHADOW WALK ABILITY CONTROLLER
/// 
/// Centralized gameplay ability system allowing the player to temporarily detach
/// their shadow, control it independently within a strict 3D leash distance,
/// traverse designated climbable surfaces, and automatically recall when duration expires.
/// 
/// Exposes all designer-configurable parameters in the Inspector.
/// </summary>
[DisallowMultipleComponent]
public class ShadowWalkAbility : MonoBehaviour
{
    public static ShadowWalkAbility Instance { get; private set; }

    public enum AbilityState
    {
        Inactive,
        Extracting,
        Active,
        Recalling
    }

    [Header("Core References")]
    [Tooltip("Reference to the Player root transform.")]
    [SerializeField] private Transform playerAnchor;

    [Tooltip("Point on the player's upper torso where the soul is extracted from and returns to.")]
    [SerializeField] private Transform soulOrigin;

    [Tooltip("Reference to the Shadow root transform.")]
    [SerializeField] private Transform shadow;

    [Tooltip("State machine managing player/shadow states.")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;

    [Tooltip("Shadow movement adapter component.")]
    [SerializeField] private ShadowMovementAdapter shadowAdapter;

    [Tooltip("Surface traversal component on the shadow.")]
    [SerializeField] private SurfaceTraversal surfaceTraversal;

    [Header("Ability Configuration (Inspector Editable)")]
    [Tooltip("Total duration the shadow can remain detached before auto-recall (seconds).")]
    [SerializeField] private float abilityDuration = 15f;

    [Tooltip("Maximum 3D distance the shadow can travel from the player (metres).")]
    [SerializeField] private float maxLeashLength = 10f;

    [Tooltip("Ground detection raycast distance for the shadow.")]
    [SerializeField] private float groundDetectionDistance = 0.6f;

    [Tooltip("Wall surface probe detection distance.")]
    [SerializeField] private float surfaceDetectionDistance = 1.5f;

    [Tooltip("Layer mask designating climbable surfaces.")]
    [SerializeField] private LayerMask climbableSurfaceLayer;

    [Header("Visual Effects")]
    [Tooltip("Particle system played at SoulOrigin when shadow detaches.")]
    [SerializeField] private ParticleSystem extractionParticles;

    [Tooltip("Particle system played at SoulOrigin when shadow reunites.")]
    [SerializeField] private ParticleSystem recallParticles;

    [Tooltip("LineRenderer rendering the spectral tether connecting player and shadow.")]
    [SerializeField] private LineRenderer tetherLine;

    [Tooltip("Number of curve points along the tether.")]
    [SerializeField] private int tetherSegments = 20;

    [Tooltip("Maximum catenary sag in metres when characters are close.")]
    [SerializeField] private float tetherMaxSag = 0.35f;

    [Header("UI Reference")]
    [SerializeField] private ShadowWalkAbilityUI abilityUI;

    [Header("Runtime State (Read Only)")]
    [SerializeField] private AbilityState currentState = AbilityState.Inactive;
    [SerializeField] private float currentTimer = 0f;
    [SerializeField] private float currentLeashDistance = 0f;
    [SerializeField] private bool isNearLeashBoundary = false;

    // Events
    public event Action<AbilityState> OnAbilityStateChanged;
    public event Action<float, float> OnTimerUpdated; // current, max
    public event Action<float, float> OnLeashDistanceUpdated; // current, max

    // Properties
    public AbilityState CurrentState => currentState;
    public float AbilityDuration { get => abilityDuration; set => abilityDuration = Mathf.Max(1f, value); }
    public float MaxLeashLength { get => maxLeashLength; set => maxLeashLength = Mathf.Max(1f, value); }
    public float RemainingTime => Mathf.Max(0f, currentTimer);
    public float NormalizedTimeRemaining => abilityDuration > 0.001f ? Mathf.Clamp01(currentTimer / abilityDuration) : 0f;
    public float CurrentLeashDistance => currentLeashDistance;
    public float NormalizedLeashDistance => maxLeashLength > 0.001f ? Mathf.Clamp01(currentLeashDistance / maxLeashLength) : 0f;
    public bool IsNearLeashBoundary => isNearLeashBoundary;
    public Transform SoulOrigin => soulOrigin;
    public Transform PlayerAnchor => playerAnchor;
    public Transform ShadowTransform => shadow;
    public LayerMask ClimbableSurfaceLayer => climbableSurfaceLayer;

    private Coroutine extractionCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this)
        {
            Destroy(this);
            return;
        }

        AutoResolveReferences();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnEnable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged += HandleStateMachineStateChanged;
        }
    }

    private void OnDisable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateMachineStateChanged;
        }

        if (extractionCoroutine != null)
        {
            StopCoroutine(extractionCoroutine);
            extractionCoroutine = null;
        }
    }

    public void AutoResolveReferences()
    {
        if (stateMachine == null)
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();

        if (playerAnchor == null)
        {
            var pc = FindAnyObjectByType<PlayerController>();
            if (pc != null) playerAnchor = pc.transform;
            else
            {
                var p = GameObject.FindWithTag("Player");
                if (p != null) playerAnchor = p.transform;
            }
        }

        if (soulOrigin == null && playerAnchor != null)
        {
            Transform so = playerAnchor.Find("SoulOrigin");
            if (so != null)
            {
                soulOrigin = so;
            }
            else
            {
                var go = new GameObject("SoulOrigin");
                go.transform.SetParent(playerAnchor, false);
                go.transform.localPosition = new Vector3(0f, 1.2f, 0f); // Upper torso level
                soulOrigin = go.transform;
            }
        }

        if (shadow == null)
        {
            var sma = FindAnyObjectByType<ShadowMovementAdapter>();
            if (sma != null)
            {
                shadow = sma.transform;
                shadowAdapter = sma;
            }
            else
            {
                var s = GameObject.Find("Shadow");
                if (s != null) shadow = s.transform;
            }
        }

        if (shadowAdapter == null && shadow != null)
        {
            shadowAdapter = shadow.GetComponent<ShadowMovementAdapter>();
        }

        if (surfaceTraversal == null && shadow != null)
        {
            surfaceTraversal = shadow.GetComponent<SurfaceTraversal>();
        }

        if (climbableSurfaceLayer.value == 0)
        {
            int shadowLayer = LayerMask.NameToLayer("ShadowSurface");
            if (shadowLayer >= 0)
            {
                climbableSurfaceLayer = 1 << shadowLayer;
            }
        }

        if (abilityUI == null)
        {
            abilityUI = FindAnyObjectByType<ShadowWalkAbilityUI>();
        }

        EnsureTetherLine();
    }

    private void EnsureTetherLine()
    {
        if (tetherLine == null)
        {
            Transform tetherObj = transform.Find("ShadowTether");
            if (tetherObj == null)
            {
                var go = new GameObject("ShadowTether");
                go.transform.SetParent(transform, false);
                tetherObj = go.transform;
            }

            tetherLine = tetherObj.GetComponent<LineRenderer>();
            if (tetherLine == null)
            {
                tetherLine = tetherObj.gameObject.AddComponent<LineRenderer>();
            }

            tetherLine.positionCount = tetherSegments;
            tetherLine.startWidth = 0.045f;
            tetherLine.endWidth = 0.025f;
            tetherLine.useWorldSpace = true;
            tetherLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tetherLine.receiveShadows = false;
            tetherLine.enabled = false;
        }
    }

    private void Update()
    {
        if (currentState == AbilityState.Active)
        {
            UpdateAbilityTimer();
            UpdateLeashDistance();
            UpdateTetherLine();
        }
        else if (currentState == AbilityState.Recalling)
        {
            UpdateTetherLine();
        }
    }

    private void UpdateAbilityTimer()
    {
        currentTimer -= Time.deltaTime;
        OnTimerUpdated?.Invoke(currentTimer, abilityDuration);

        if (currentTimer <= 0f)
        {
            Debug.Log("[ShadowWalkAbility] Duration expired. Initiating automatic recall.");
            TriggerRecall();
        }
    }

    private void UpdateLeashDistance()
    {
        if (playerAnchor == null || shadow == null) return;

        Vector3 origin = soulOrigin != null ? soulOrigin.position : playerAnchor.position;
        Vector3 target = shadow.position;
        currentLeashDistance = Vector3.Distance(origin, target);

        isNearLeashBoundary = currentLeashDistance >= maxLeashLength * 0.88f;

        OnLeashDistanceUpdated?.Invoke(currentLeashDistance, maxLeashLength);
    }

    private void UpdateTetherLine()
    {
        if (tetherLine == null || !tetherLine.enabled) return;
        if (soulOrigin == null || shadow == null) return;

        Vector3 start = soulOrigin.position;
        Vector3 end = shadow.position + Vector3.up * 0.85f;

        float dist = Vector3.Distance(start, end);
        float normalizedDist = maxLeashLength > 0.001f ? Mathf.Clamp01(dist / maxLeashLength) : 0f;

        // Catenary sag decreases as leash tightens
        float currentSag = (1f - normalizedDist) * tetherMaxSag;

        // Dynamic width & wave pulse near boundary
        float wave = Mathf.Sin(Time.time * 18f) * 0.01f;
        float width = isNearLeashBoundary ? (0.065f + wave) : 0.045f;
        tetherLine.startWidth = width;
        tetherLine.endWidth = width * 0.6f;

        // Populate spline points with natural downward catenary arc
        if (tetherLine.positionCount != tetherSegments)
        {
            tetherLine.positionCount = tetherSegments;
        }

        for (int i = 0; i < tetherSegments; i++)
        {
            float t = (float)i / (tetherSegments - 1);
            Vector3 point = Vector3.Lerp(start, end, t);

            // Parabolic curve for realistic hanging filament
            float sagOffset = Mathf.Sin(t * Mathf.PI) * currentSag;
            point.y -= sagOffset;

            // Subtle spectral vibration when tight
            if (isNearLeashBoundary)
            {
                float jitter = Mathf.Sin(Time.time * 24f + i * 0.5f) * 0.02f * Mathf.Sin(t * Mathf.PI);
                point += Vector3.Cross((end - start).normalized, Vector3.up) * jitter;
            }

            tetherLine.SetPosition(i, point);
        }
    }

    // =========================================================
    // LEASH CONSTRAINT VALIDATION
    // =========================================================

    /// <summary>
    /// Constrains a proposed horizontal movement vector so the shadow cannot exceed maxLeashLength.
    /// Preserves tangential movement (sliding along the boundary) while blocking outward movement.
    /// </summary>
    public Vector3 ConstrainHorizontalMovement(Vector3 currentPos, Vector3 proposedDelta)
    {
        if (currentState != AbilityState.Active && currentState != AbilityState.Extracting)
            return proposedDelta;

        Vector3 anchorPos = soulOrigin != null ? soulOrigin.position : (playerAnchor != null ? playerAnchor.position : currentPos);

        Vector3 toShadow = currentPos - anchorPos;
        float currentDist = toShadow.magnitude;

        Vector3 targetPos = currentPos + proposedDelta;
        Vector3 toTarget = targetPos - anchorPos;
        float targetDist = toTarget.magnitude;

        if (targetDist <= maxLeashLength)
        {
            return proposedDelta;
        }

        // Outside boundary! Constrain to tangential movement
        if (currentDist > 0.001f)
        {
            Vector3 radialDir = toShadow.normalized;
            float outwardComponent = Vector3.Dot(proposedDelta, radialDir);

            if (outwardComponent > 0f)
            {
                // Subtract the outward component so movement slides tangentially
                Vector3 tangentialDelta = proposedDelta - radialDir * outwardComponent;
                Vector3 clampedTarget = currentPos + tangentialDelta;

                Vector3 toClamped = clampedTarget - anchorPos;
                if (toClamped.magnitude > maxLeashLength)
                {
                    clampedTarget = anchorPos + toClamped.normalized * maxLeashLength;
                    tangentialDelta = clampedTarget - currentPos;
                }

                return tangentialDelta;
            }
        }

        return proposedDelta;
    }

    /// <summary>
    /// Full 3D leash validation used by wall climbing and surface traversal.
    /// Returns true if within leash distance; otherwise returns false and outputs clamped position.
    /// </summary>
    public bool ValidateProposedPosition(Vector3 proposedPos, out Vector3 clampedPos)
    {
        clampedPos = proposedPos;
        if (currentState != AbilityState.Active && currentState != AbilityState.Extracting)
            return true;

        Vector3 anchorPos = soulOrigin != null ? soulOrigin.position : (playerAnchor != null ? playerAnchor.position : proposedPos);
        Vector3 toProposed = proposedPos - anchorPos;
        float dist = toProposed.magnitude;

        if (dist <= maxLeashLength)
        {
            return true;
        }

        clampedPos = anchorPos + toProposed.normalized * maxLeashLength;
        return false;
    }

    // =========================================================
    // ABILITY ACTIVATION & RECALL FLOW
    // =========================================================

    public bool ActivateShadowWalk()
    {
        if (currentState != AbilityState.Inactive)
        {
            Debug.Log($"[ShadowWalkAbility] Cannot activate. Current state: {currentState}");
            return false;
        }

        if (stateMachine != null && !stateMachine.CanPerformAction(InputAction.Detach))
        {
            Debug.Log("[ShadowWalkAbility] State machine cannot perform Detach.");
            return false;
        }

        if (extractionCoroutine != null) StopCoroutine(extractionCoroutine);
        extractionCoroutine = StartCoroutine(ExecuteSoulExtractionSequence());
        return true;
    }

    private IEnumerator ExecuteSoulExtractionSequence()
    {
        SetState(AbilityState.Extracting);

        // 1. Play Soul Extraction VFX
        if (extractionParticles != null)
        {
            extractionParticles.gameObject.SetActive(true);
            extractionParticles.Play(true);
        }

        // Brief extraction windup pulse
        yield return new WaitForSeconds(0.12f);

        // 2. Perform actual detachment via authoritative state machine
        if (stateMachine != null)
        {
            stateMachine.Detach();
        }

        // 3. Reset and start countdown timer
        currentTimer = abilityDuration;

        // 4. Enable Tether
        if (tetherLine != null)
        {
            tetherLine.enabled = true;
        }

        SetState(AbilityState.Active);
        extractionCoroutine = null;
        Debug.Log($"[ShadowWalkAbility] Shadow Walk ACTIVE. Duration: {abilityDuration}s, Max Leash: {maxLeashLength}m.");
    }

    public bool TriggerRecall()
    {
        if (currentState != AbilityState.Active && currentState != AbilityState.Extracting)
        {
            return false;
        }

        SetState(AbilityState.Recalling);

        // Stop timer
        currentTimer = 0f;

        // Trigger existing authoritative recall
        if (stateMachine != null)
        {
            stateMachine.Recall();
        }

        return true;
    }

    private void HandleStateMachineStateChanged(PlayerShadowState newState)
    {
        switch (newState)
        {
            case PlayerShadowState.Attached:
                CompleteAbilityCycle();
                break;

            case PlayerShadowState.Detached:
                if (currentState == AbilityState.Inactive)
                {
                    // If detached externally (e.g. from test script), sync state
                    currentTimer = abilityDuration;
                    if (tetherLine != null) tetherLine.enabled = true;
                    SetState(AbilityState.Active);
                }
                break;

            case PlayerShadowState.Reuniting:
                if (currentState != AbilityState.Recalling)
                {
                    SetState(AbilityState.Recalling);
                }
                break;

            case PlayerShadowState.Trapped:
                // If trapped, pause timer but maintain leash/tether
                break;
        }
    }

    private void CompleteAbilityCycle()
    {
        // 1. Play Recall / Reunification VFX
        if (recallParticles != null)
        {
            recallParticles.gameObject.SetActive(true);
            recallParticles.Play(true);
        }

        // 2. Disable tether
        if (tetherLine != null)
        {
            tetherLine.enabled = false;
        }

        // 3. Stop extraction particles
        if (extractionParticles != null && extractionParticles.isPlaying)
        {
            extractionParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        currentTimer = 0f;
        isNearLeashBoundary = false;
        SetState(AbilityState.Inactive);
        Debug.Log("[ShadowWalkAbility] Shadow reunited. Ability reset to Inactive.");
    }

    private void SetState(AbilityState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        OnAbilityStateChanged?.Invoke(currentState);
    }
}
