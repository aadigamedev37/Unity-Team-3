using UnityEngine;

/// <summary>
/// Drives a lightweight "camera focus" transform that the gameplay CinemachineCamera follows.
///
/// The focus tracks the ROOT transform of the currently controlled entity (never a child
/// model/skeleton transform), which avoids orbital motion when the character turns.
///
/// Active entity is resolved from the authoritative PlayerShadowStateMachine:
///   Attached  -> Player
///   Detached  -> Shadow
///   Trapped   -> Shadow
///   Reuniting -> Player (camera returns to the player while the shadow travels back)
/// Falls back to InputSystem.ActiveTarget if no state machine is assigned.
///
/// Camera rotation is NOT controlled here; the CinemachineCamera keeps a fixed yaw/pitch so
/// camera-relative joystick movement stays stable and dragging the joystick never orbits.
/// </summary>
[DefaultExecutionOrder(-100)]
public class ActiveEntityCameraFocus : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform shadowRoot;

    [Header("Framing")]
    [Tooltip("Height of the focus point above the entity's feet.")]
    [SerializeField] private float focusHeight = 1.0f;

    [Tooltip("Constant world-space look-ahead toward the course direction (exit is +Z in M1).")]
    [SerializeField] private Vector3 courseLookAhead = new Vector3(0f, 0f, 1.8f);

    [Tooltip("Extra look-ahead in the entity's horizontal movement direction.")]
    [SerializeField] private float movementLookAhead = 1.0f;

    [SerializeField] private float lookAheadSmoothTime = 0.4f;

    [Header("Damping")]
    [Tooltip("Positional smoothing while following the same entity.")]
    [SerializeField] private float followSmoothTime = 0.15f;

    [Tooltip("Positional smoothing while transitioning to a newly controlled entity.")]
    [SerializeField] private float transitionSmoothTime = 0.4f;

    [Tooltip("How long the softer transition smoothing is applied after a target switch.")]
    [SerializeField] private float transitionDuration = 0.8f;

    [SerializeField] private float maxFocusSpeed = 60f;

    private Vector3 focusVelocity;
    private Vector3 lookAheadOffset;
    private Vector3 lookAheadVelocity;
    private Vector3 lastTargetPosition;
    private float transitionTimer;
    private bool hasLastTargetPosition;

    public InputTarget CurrentFocus { get; private set; } = InputTarget.Player;
    public Transform CurrentTarget => CurrentFocus == InputTarget.Shadow ? shadowRoot : playerRoot;
    public bool IsTransitioning => transitionTimer > 0f;

    private void Awake()
    {
        if (stateMachine == null) stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();
        if (inputSystem == null) inputSystem = FindAnyObjectByType<InputSystem>();

        if (playerRoot == null)
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null) playerRoot = player.transform;
        }

        if (shadowRoot == null)
        {
            var shadow = FindAnyObjectByType<ShadowMovementAdapter>();
            if (shadow != null) shadowRoot = shadow.transform;
        }
    }

    private void OnEnable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }
    }

    private void Start()
    {
        CurrentFocus = ResolveFocus();
        SnapToTarget();
    }

    private void HandleStateChanged(PlayerShadowState newState)
    {
        InputTarget resolved = ResolveFocus();
        if (resolved != CurrentFocus)
        {
            SwitchFocus(resolved);
        }
    }

    private InputTarget ResolveFocus()
    {
        if (stateMachine != null)
        {
            switch (stateMachine.CurrentState)
            {
                case PlayerShadowState.Detached:
                case PlayerShadowState.Trapped:
                    return InputTarget.Shadow;
                default:
                    return InputTarget.Player;
            }
        }

        return inputSystem != null ? inputSystem.ActiveTarget : InputTarget.Player;
    }

    private void SwitchFocus(InputTarget newFocus)
    {
        CurrentFocus = newFocus;
        transitionTimer = transitionDuration;
        hasLastTargetPosition = false;
        lookAheadVelocity = Vector3.zero;
    }

    /// <summary>Immediately places the focus on the active entity (used at start / after resets).</summary>
    public void SnapToTarget()
    {
        Transform target = CurrentTarget;
        if (target == null) return;

        lookAheadOffset = Vector3.zero;
        lookAheadVelocity = Vector3.zero;
        focusVelocity = Vector3.zero;
        transform.position = GetFeetPosition(target) + Vector3.up * focusHeight + courseLookAhead;
        lastTargetPosition = target.position;
        hasLastTargetPosition = true;
    }

    private void LateUpdate()
    {
        // Safety net: keep in sync even if a state event was missed (e.g. reset paths).
        InputTarget resolved = ResolveFocus();
        if (resolved != CurrentFocus)
        {
            SwitchFocus(resolved);
        }

        Transform target = CurrentTarget;
        if (target == null) return;

        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        // Estimate horizontal velocity of the target root for movement look-ahead.
        Vector3 desiredLookAhead = Vector3.zero;
        if (hasLastTargetPosition)
        {
            Vector3 delta = target.position - lastTargetPosition;
            delta.y = 0f;
            float speed = delta.magnitude / dt;
            if (speed > 0.2f)
            {
                desiredLookAhead = delta.normalized * movementLookAhead * Mathf.Clamp01(speed / 4f);
            }
        }
        lastTargetPosition = target.position;
        hasLastTargetPosition = true;

        lookAheadOffset = Vector3.SmoothDamp(lookAheadOffset, desiredLookAhead, ref lookAheadVelocity, lookAheadSmoothTime);

        Vector3 desired = GetFeetPosition(target) + Vector3.up * focusHeight + courseLookAhead + lookAheadOffset;

        float smoothTime = followSmoothTime;
        if (transitionTimer > 0f)
        {
            transitionTimer -= dt;
            smoothTime = transitionSmoothTime;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desired, ref focusVelocity, smoothTime, maxFocusSpeed, dt);
    }

    private static Vector3 GetFeetPosition(Transform target)
    {
        var cc = target.GetComponent<CharacterController>();
        if (cc != null)
        {
            Vector3 center = target.TransformPoint(cc.center);
            float halfHeight = cc.height * 0.5f * target.lossyScale.y;
            return new Vector3(target.position.x, center.y - halfHeight, target.position.z);
        }

        return target.position;
    }
}
