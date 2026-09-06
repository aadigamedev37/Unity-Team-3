using UnityEngine;

public abstract class StealableBase : MonoBehaviour
{
    public enum StealableState
    {
        Solid,
        Ghost,
        Frozen
    }

    [SerializeField]
    private StealableState currentState = StealableState.Solid;

    public StealableState CurrentState => currentState;

    public bool IsSolid => currentState == StealableState.Solid;
    public bool IsGhost => currentState == StealableState.Ghost;
    public bool IsFrozen => currentState == StealableState.Frozen;

    /// <summary>
    /// Returns true when this object can currently be stolen.
    /// </summary>
    public virtual bool CanSteal()
    {
        return currentState == StealableState.Solid;
    }

    /// <summary>
    /// Called by the interaction system when the player steals this object.
    /// </summary>
    public void Steal()
    {
        if (!CanSteal())
            return;

        SetState(GetStateAfterSteal());
    }

    /// <summary>
    /// Restores the object back to its normal solid state.
    /// </summary>
    public void Restore()
    {
        SetState(StealableState.Solid);
    }

    protected abstract StealableState GetStateAfterSteal();

    protected virtual void OnStateChanged(StealableState newState)
    {
    }

    protected void SetState(StealableState newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        OnStateChanged(newState);
    }
}