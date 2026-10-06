using UnityEngine;

public class StealableDrawbridge : StealableBase
{
    [Header("Drawbridge References")]
    [SerializeField]
    private Collider drawbridgeCollider;

    private void Awake()
    {
        if (drawbridgeCollider == null)
        {
            Debug.LogError(
                "StealableDrawbridge: Physical Drawbridge Collider is NOT assigned!",
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

        Debug.Log("Drawbridge State Changed To: " + newState);
    }

    private void ApplyCurrentState()
    {
        if (drawbridgeCollider == null)
            return;

        drawbridgeCollider.enabled = !IsGhost;
    }

    // TEMPORARY TEST ONLY
    private void Update()
    {
        
    }
}