using UnityEngine;

public class StealableDoor : StealableBase
{
    [Header("Door References")]
    [SerializeField]
    private Collider doorCollider;

    private void Awake()
    {
        // Make sure the physical door collider is assigned.
        if (doorCollider == null)
        {
            Debug.LogError(
                "StealableDoor: Physical Door Collider is NOT assigned!",
                this
            );

            return;
        }

        // Apply the starting state.
        ApplyCurrentState();
    }

    protected override StealableState GetStateAfterSteal()
    {
        // Door becomes Ghost after being stolen.
        return StealableState.Ghost;
    }

    protected override void OnStateChanged(StealableState newState)
    {
        ApplyCurrentState();

        Debug.Log("Door State Changed To: " + newState);
    }

    private void ApplyCurrentState()
    {
        if (doorCollider == null)
            return;

        // Solid = collider ON
        // Ghost = collider OFF
        doorCollider.enabled = !IsGhost;
    }

    // ---------------------------------------------------------
    // TEMPORARY TEST
    // Press SPACE to steal the door.
    // Remove this method after testing.
    // ---------------------------------------------------------

    private void Update()
    {
       
    }

    // ---------------------------------------------------------
    // RESTORE
    // When the Player leaves the restore trigger,
    // the Ghost door becomes Solid again.
    // ---------------------------------------------------------

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("DOOR TRIGGER EXIT: " + other.name);

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Trigger exit was NOT the Player.");
            return;
        }

        if (!IsGhost)
        {
            Debug.Log("Door is not Ghost. No restore needed.");
            return;
        }

        Debug.Log("PLAYER EXITED GHOST DOOR - RESTORING");

        Restore();

        Debug.Log("Door Restored. Current State: " + CurrentState);
    }
}