using UnityEngine;

public class PlayerShadowStateMachine : MonoBehaviour
{
    public PlayerShadowState CurrentState { get; private set; }

    private void Awake()
    {
        CurrentState = PlayerShadowState.Attached;
    }

    public bool Detach()
    {
        if (CurrentState != PlayerShadowState.Attached)
        {
            return false;
        }

        CurrentState = PlayerShadowState.Detached;
        return true;
    }

    public bool Recall()
    {
        if (CurrentState != PlayerShadowState.Detached)
        {
            return false;
        }

        CurrentState = PlayerShadowState.Reuniting;
        return true;
    }

    public bool CompleteReunion()
    {
        if (CurrentState != PlayerShadowState.Reuniting)
        {
            return false;
        }

        CurrentState = PlayerShadowState.Attached;
        return true;
    }

    public bool IsState(PlayerShadowState state)
    {
        return CurrentState == state;
    }
}