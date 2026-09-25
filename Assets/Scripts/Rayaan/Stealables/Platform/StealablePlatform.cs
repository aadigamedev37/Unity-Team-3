using UnityEngine;

public class StealablePlatform : StealableBase
{
    [Header("Platform References")]
    [SerializeField]
    private Collider platformCollider;

    private void Awake()
    {
        if (platformCollider == null)
        {
            Debug.LogError(
                "StealablePlatform: Platform Collider is NOT assigned!",
                this
            );

            return;
        }

        ApplyCurrentState();
    }

    protected override StealableState GetStateAfterSteal()
    {
        // A stolen platform becomes FROZEN.
        return StealableState.Frozen;
    }

    protected override void OnStateChanged(StealableState newState)
    {
        ApplyCurrentState();

        Debug.Log("Platform State Changed To: " + newState);
    }

    private void ApplyCurrentState()
    {
        if (platformCollider == null)
            return;

        // IMPORTANT:
        // Solid  = collider ON
        // Frozen = collider ON
        // Ghost  = collider OFF
        platformCollider.enabled = !IsGhost;
    }
}