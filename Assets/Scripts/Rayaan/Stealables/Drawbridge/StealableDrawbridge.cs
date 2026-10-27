using UnityEngine;

/// <summary>
/// A drawbridge obstacle that lowers into a walkable crossing when its shadow is stolen.
/// Extends StealableBase following Rayaan's interaction framework.
/// </summary>
public class StealableDrawbridge : StealableBase
{
    [Header("Bridge References")]
    [SerializeField] private Collider walkableCollider;
    [SerializeField] private Transform bridgeArm;

    [Header("Rotation Configuration")]
    [SerializeField] private Vector3 raisedEuler = new Vector3(-60f, 0f, 0f);
    [SerializeField] private Vector3 loweredEuler = Vector3.zero;

    private void Awake()
    {
        ApplyCurrentState();
    }

    protected override StealableState GetStateAfterSteal()
    {
        // When stolen, the bridge drops down (becomes usable/walkable)
        return StealableState.Ghost;
    }

    protected override void OnStateChanged(StealableState newState)
    {
        ApplyCurrentState();
        Debug.Log("StealableDrawbridge: State changed to " + newState);
    }

    private void ApplyCurrentState()
    {
        bool isLowered = IsGhost;

        if (walkableCollider != null)
        {
            walkableCollider.enabled = isLowered;
        }

        if (bridgeArm != null)
        {
            bridgeArm.localEulerAngles = isLowered ? loweredEuler : raisedEuler;
        }
    }
}
