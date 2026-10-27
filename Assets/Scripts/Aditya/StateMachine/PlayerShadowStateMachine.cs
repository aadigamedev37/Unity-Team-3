using System;
using UnityEngine;

public class PlayerShadowStateMachine : MonoBehaviour
{
    public PlayerShadowState CurrentState { get; private set; }

    public event Action<PlayerShadowState> OnStateChanged;

    private void Awake()
    {
        CurrentState = PlayerShadowState.Attached;
    }

    public bool CanPerformAction(InputAction action)
    {
        switch (action)
        {
            case InputAction.Detach:
                return CurrentState == PlayerShadowState.Attached;

            case InputAction.Recall:
                return CurrentState == PlayerShadowState.Detached;

            case InputAction.Steal:
                return CurrentState == PlayerShadowState.Detached;

            case InputAction.Recover:
                return CurrentState == PlayerShadowState.Trapped;

            default:
                return false;
        }
    }

    public bool Detach()
    {
        if (!CanPerformAction(InputAction.Detach))
        {
            Debug.Log("Detach blocked. Current state: " + CurrentState);
            return false;
        }

        ChangeState(PlayerShadowState.Detached);

        Debug.Log("Shadow Detached");
        return true;
    }

    public bool Recall()
    {
        if (!CanPerformAction(InputAction.Recall))
        {
            Debug.Log("Recall blocked. Current state: " + CurrentState);
            return false;
        }

        ChangeState(PlayerShadowState.Reuniting);

        Debug.Log("Shadow Reuniting");
        return true;
    }

    public bool CompleteReunion()
    {
        if (CurrentState != PlayerShadowState.Reuniting)
        {
            Debug.Log("CompleteReunion blocked. Current state: " + CurrentState);
            return false;
        }

        ChangeState(PlayerShadowState.Attached);

        Debug.Log("Shadow Reunited");
        return true;
    }

    public bool Trap()
    {
        if (CurrentState != PlayerShadowState.Detached)
        {
            Debug.Log("Trap blocked. Current state: " + CurrentState);
            return false;
        }

        ChangeState(PlayerShadowState.Trapped);

        Debug.Log("Shadow Trapped");
        return true;
    }

    public bool Steal()
    {
        if (!CanPerformAction(InputAction.Steal))
        {
            Debug.Log("Steal blocked. Current state: " + CurrentState);
            return false;
        }

        Debug.Log("Steal triggered");
        return true;
    }

    public bool Recover()
    {
        if (!CanPerformAction(InputAction.Recover))
        {
            Debug.Log("Recover blocked. Current state: " + CurrentState);
            return false;
        }

        ChangeState(PlayerShadowState.Reuniting);

        Debug.Log("Recover triggered. Shadow is reuniting.");
        return true;
    }

    public bool IsState(PlayerShadowState state)
    {
        return CurrentState == state;
    }

    public void ResetToAttached()
    {
        ChangeState(PlayerShadowState.Attached);
        Debug.Log("PlayerShadowStateMachine: Reset to Attached.");
    }

    private void ChangeState(PlayerShadowState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }
}