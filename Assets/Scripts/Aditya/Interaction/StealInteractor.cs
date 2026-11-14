using System;
using UnityEngine;

/// <summary>
/// Detects Stealable objects within range of the Shadow and coordinates
/// with the ContextualActionButton and InputSystem to execute steals.
/// Optimized for zero per-frame GC allocations using NonAlloc physics queries.
/// </summary>
public class StealInteractor : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private ContextualActionButton actionButton;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 3.5f;
    [SerializeField] private LayerMask stealableLayer = ~0;

    // Buffer sized generously for scenes with dense modular floor/wall colliders
    private readonly Collider[] hitBuffer = new Collider[128];
    private StealableBase currentTarget;

    public StealableBase CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null && currentTarget.CanSteal();

    public event Action<StealableBase> OnStealExecuted;

    private void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        if (stateMachine == null)
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>(FindObjectsInactive.Include);

        if (inputSystem == null)
            inputSystem = FindAnyObjectByType<InputSystem>(FindObjectsInactive.Include);

        if (actionButton == null)
            actionButton = FindAnyObjectByType<ContextualActionButton>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        ResolveDependencies();
        if (inputSystem != null)
        {
            inputSystem.OnAction -= HandleActionTriggered;
            inputSystem.OnAction += HandleActionTriggered;
        }
    }

    private void OnDisable()
    {
        if (inputSystem != null)
        {
            inputSystem.OnAction -= HandleActionTriggered;
        }

        ClearTarget();
    }

    private void Update()
    {
        CheckForNearbyStealables();
    }

    private void CheckForNearbyStealables()
    {
        if (stateMachine == null || actionButton == null || inputSystem == null)
        {
            ResolveDependencies();
        }

        if (stateMachine != null && stateMachine.CurrentState != PlayerShadowState.Detached)
        {
            if (currentTarget != null)
            {
                ClearTarget();
            }
            return;
        }

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, hitBuffer, stealableLayer);
        StealableBase closestValid = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = hitBuffer[i];
            if (col == null) continue;

            StealableBase stealable = col.GetComponentInParent<StealableBase>() ?? col.GetComponentInChildren<StealableBase>();
            if (stealable != null && stealable.CanSteal())
            {
                // Bounds.ClosestPoint is universally supported on all collider types without throwing warnings
                Vector3 surfacePoint = col.bounds.ClosestPoint(transform.position);
                float dist = Vector3.Distance(transform.position, surfacePoint);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestValid = stealable;
                }
            }
        }

        if (closestValid != currentTarget)
        {
            currentTarget = closestValid;
            bool targetAvailable = currentTarget != null;

            if (actionButton != null)
            {
                actionButton.SetStealTargetInRange(targetAvailable);
            }
        }
    }

    private void ClearTarget()
    {
        currentTarget = null;
        if (actionButton != null)
        {
            actionButton.SetStealTargetInRange(false);
        }
    }

    private void HandleActionTriggered(InputAction action)
    {
        if (action == InputAction.Steal)
        {
            ExecuteSteal();
        }
    }

    public bool ExecuteSteal()
    {
        if (stateMachine != null && stateMachine.CurrentState != PlayerShadowState.Detached)
        {
            Debug.LogWarning("StealInteractor: Cannot steal while not in Detached state.");
            return false;
        }

        if (currentTarget == null || !currentTarget.CanSteal())
        {
            Debug.LogWarning("StealInteractor: No valid stealable target in range.");
            return false;
        }

        Debug.Log($"StealInteractor: Stealing {currentTarget.name}!");
        currentTarget.Steal();
        OnStealExecuted?.Invoke(currentTarget);

        ClearTarget();
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
