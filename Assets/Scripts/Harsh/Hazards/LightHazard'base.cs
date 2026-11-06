
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared framework for environmental light hazards.
/// Detects Shadows using the spotlight cone, controls pulsing,
/// and lets derived hazards define their gameplay response.
/// </summary>
public abstract class LightHazardBase : MonoBehaviour
{
    public enum TemporalPattern
    {
        Continuous,
        Pulsing
    }

    [Header("Hazard Configuration")]
    [SerializeField]
    private bool isActive = true;

    [SerializeField]
    private TemporalPattern temporalPattern = TemporalPattern.Continuous;

    [Min(0.05f)]
    [SerializeField]
    private float pulseInterval = 1f;

    [Header("Spotlight Detection")]
    [SerializeField]
    private Light visualLight;

    [SerializeField]
    private LayerMask detectionLayers = ~0;

    [SerializeField]
    private LayerMask obstructionLayers = ~0;

    private const int CandidateCapacity = 128;
    private const float RayOriginOffset = 0.01f;

    private readonly Collider[] candidateBuffer =
        new Collider[CandidateCapacity];

    private readonly HashSet<MonoBehaviour> currentlyDetected = new();
    private readonly HashSet<MonoBehaviour> previouslyDetected = new();

    private float pulseStartTime;
    private bool pulseIsActive = true;

    public bool IsActive =>
        isActive &&
        (temporalPattern != TemporalPattern.Pulsing || pulseIsActive);

    public TemporalPattern CurrentTemporalPattern => temporalPattern;

    private void Awake()
    {
        if (visualLight == null)
        {
            visualLight = GetComponentInChildren<Light>(true);
        }

        pulseInterval = Mathf.Max(0.05f, pulseInterval);
        pulseStartTime = Time.time;

        if (visualLight == null)
        {
            Debug.LogError(
                "LightHazardBase requires a Light reference.",
                this);
        }
        else if (visualLight.type != LightType.Spot)
        {
            Debug.LogError(
                "LightHazardBase requires a Spot Light for cone detection.",
                this);
        }

        ApplyVisualState();
    }

    private void OnEnable()
    {
        pulseStartTime = Time.time;
        pulseIsActive = true;

        currentlyDetected.Clear();
        previouslyDetected.Clear();

        ApplyVisualState();
    }

    private void OnDisable()
    {
        currentlyDetected.Clear();
        previouslyDetected.Clear();

        if (visualLight != null)
        {
            visualLight.enabled = false;
        }
    }

    private void Update()
    {
        UpdatePulse();
        ApplyVisualState();
        DetectIlluminatedShadows();
    }

    private void UpdatePulse()
    {
        pulseInterval = Mathf.Max(0.05f, pulseInterval);

        if (temporalPattern != TemporalPattern.Pulsing)
        {
            pulseIsActive = true;
            return;
        }

        float elapsed = Time.time - pulseStartTime;
        float cycleLength = pulseInterval * 2f;

        pulseIsActive =
            Mathf.Repeat(elapsed, cycleLength) < pulseInterval;
    }

    public void Activate()
    {
        isActive = true;
        ApplyVisualState();
    }

    public void Deactivate()
    {
        isActive = false;
        ApplyVisualState();
    }

    private void ApplyVisualState()
    {
        if (visualLight != null)
        {
            visualLight.enabled = IsActive;
        }
    }

    private void DetectIlluminatedShadows()
    {
        currentlyDetected.Clear();

        if (!IsActive ||
            visualLight == null ||
            visualLight.type != LightType.Spot ||
            !visualLight.enabled)
        {
            previouslyDetected.Clear();
            return;
        }

        Vector3 lightPosition = visualLight.transform.position;

        int candidateCount = Physics.OverlapSphereNonAlloc(
            lightPosition,
            visualLight.range,
            candidateBuffer,
            detectionLayers,
            QueryTriggerInteraction.Collide);

        for (int i = 0; i < candidateCount; i++)
        {
            Collider candidate = candidateBuffer[i];
            candidateBuffer[i] = null;

            if (candidate == null)
            {
                continue;
            }

            if (!TryGetShadowController(candidate, out MonoBehaviour shadow))
            {
                continue;
            }

            if (!IsInsideSpotlight(candidate.bounds.center, lightPosition))
            {
                continue;
            }

            if (!HasClearLineOfSight(
                    candidate.bounds.center,
                    lightPosition,
                    shadow))
            {
                continue;
            }

            // Multiple colliders belonging to one Shadow must produce
            // only one response during the same detection update.
            if (!currentlyDetected.Add(shadow))
            {
                continue;
            }

            // Respond only when the Shadow first becomes illuminated.
            if (!previouslyDetected.Contains(shadow))
            {
                HandleHazardEnter(candidate);
            }
        }

        previouslyDetected.Clear();

        foreach (MonoBehaviour shadow in currentlyDetected)
        {
            if (shadow != null)
            {
                previouslyDetected.Add(shadow);
            }
        }
    }

    private bool TryGetShadowController(
        Collider candidate,
        out MonoBehaviour shadowController)
    {
        MonoBehaviour[] behaviours =
            candidate.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IShadowController &&
                behaviour.isActiveAndEnabled)
            {
                shadowController = behaviour;
                return true;
            }
        }

        shadowController = null;
        return false;
    }

    private bool IsInsideSpotlight(
        Vector3 targetPosition,
        Vector3 lightPosition)
    {
        Vector3 toTarget = targetPosition - lightPosition;
        float distance = toTarget.magnitude;

        if (distance > visualLight.range || distance <= 0.001f)
        {
            return false;
        }

        float halfAngleRadians =
            visualLight.spotAngle * 0.5f * Mathf.Deg2Rad;

        float minimumDot = Mathf.Cos(halfAngleRadians);

        float alignment = Vector3.Dot(
            visualLight.transform.forward,
            toTarget / distance);

        return alignment >= minimumDot;
    }

    private bool HasClearLineOfSight(
        Vector3 targetPosition,
        Vector3 lightPosition,
        MonoBehaviour shadowController)
    {
        Vector3 toTarget = targetPosition - lightPosition;
        float distance = toTarget.magnitude;

        if (distance <= RayOriginOffset)
        {
            return true;
        }

        Vector3 direction = toTarget / distance;
        Vector3 rayOrigin =
            lightPosition + direction * RayOriginOffset;

        if (!Physics.Raycast(
                rayOrigin,
                direction,
                out RaycastHit hit,
                distance,
                obstructionLayers,
                QueryTriggerInteraction.Ignore))
        {
            // No opaque collider blocks the ray.
            return true;
        }

        // The first hit may be the Shadow's own collider.
        MonoBehaviour[] behaviours =
            hit.collider.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == shadowController)
            {
                return true;
            }
        }

        // An unrelated collider blocks the spotlight.
        return false;
    }

    protected abstract void HandleHazardEnter(Collider other);
}
