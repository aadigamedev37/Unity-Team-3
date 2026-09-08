using UnityEngine;

public class StealableDoor : StealableBase
{
    [Header("Door References")]
    [SerializeField]
    private Collider doorCollider;

    private void Awake()
    {
        if (doorCollider == null)
        {
            Debug.LogError(
                "StealableDoor: Physical Door Collider is NOT assigned!",
                this
            );

            return;
        }

        ApplyCurrentState();
    }

    protected override StealableState GetStateAfterSteal()
    {
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

        doorCollider.enabled = !IsGhost;
    }

    private void Update()
    {
    }
}