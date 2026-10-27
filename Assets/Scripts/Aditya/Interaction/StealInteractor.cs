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
    [SerializeField] private float detectionRadius = 2.0f;
    [SerializeField] private LayerMask stealableLayer = ~0;

    private readonly Collider[] hitBuffer = new Collider[16];
    private StealableBase currentTarget;

    public StealableBase CurrentTarget => currentTarget;
    public bool HasTarget => currentTarget != null && currentTarget.CanSteal();

    public event Action<StealableBase> OnStealExecuted;

    private void Awake()
    {
        if (stateMachine == null)
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();

        if (inputSystem == null)
            inputSystem = FindAnyObjectByType<InputSystem>();

        if (actionButton == null)
            actionButton = FindAnyObjectByType<ContextualActionButton>();
    }

    private void OnEnable()
    {
        if (inputSystem != null)
        {
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

            StealableBase stealable = col.GetComponentInParent<StealableBase>();
            if (stealable != null && stealable.CanSteal())
            {
                float dist = Vector3.Distance(transform.position, stealable.transform.position);
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
