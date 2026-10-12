using System;
using UnityEngine;

/// <summary>
/// Central integration manager connecting hazards (DangerLight, TrapLight),
/// state machine transitions, and level recovery/restart infrastructure.
/// </summary>
public class GameplayEventIntegration : MonoBehaviour
{
    [Header("Core Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private RestartSystem restartSystem;
    [SerializeField] private RecoverHandler recoverHandler;

    [Header("Hazard References (Optional - auto-found if unassigned)")]
    [SerializeField] private DangerLight[] dangerLights;
    [SerializeField] private TrapLight[] trapLights;

    private void Awake()
    {
        if (stateMachine == null)
        {
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();
        }

        if (restartSystem == null)
        {
            restartSystem = FindAnyObjectByType<RestartSystem>();
        }

        if (recoverHandler == null)
        {
            recoverHandler = FindAnyObjectByType<RecoverHandler>();
        }

        if (dangerLights == null || dangerLights.Length == 0)
        {
            dangerLights = FindObjectsByType<DangerLight>();
        }

        if (trapLights == null || trapLights.Length == 0)
        {
            trapLights = FindObjectsByType<TrapLight>();
        }
    }

    private void OnEnable()
    {
        SubscribeToHazards();

        if (recoverHandler != null)
        {
            recoverHandler.OnRecoveryFailed += HandleShadowDestroyed;
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromHazards();

        if (recoverHandler != null)
        {
            recoverHandler.OnRecoveryFailed -= HandleShadowDestroyed;
        }
    }

    private void SubscribeToHazards()
    {
        if (dangerLights != null)
        {
            foreach (var dl in dangerLights)
            {
                if (dl != null)
                {
                    dl.OnShadowDestroyed += HandleShadowDestroyed;
                }
            }
        }

        if (trapLights != null)
        {
            foreach (var tl in trapLights)
            {
                if (tl != null)
                {
                    tl.OnShadowTrapped += HandleShadowTrapped;
                }
            }
        }
    }

    private void UnsubscribeFromHazards()
    {
        if (dangerLights != null)
        {
            foreach (var dl in dangerLights)
            {
                if (dl != null)
                {
                    dl.OnShadowDestroyed -= HandleShadowDestroyed;
                }
            }
        }

        if (trapLights != null)
        {
            foreach (var tl in trapLights)
            {
                if (tl != null)
                {
                    tl.OnShadowTrapped -= HandleShadowTrapped;
                }
            }
        }
    }

    public void RegisterDangerLight(DangerLight dl)
    {
        if (dl != null)
        {
            dl.OnShadowDestroyed -= HandleShadowDestroyed;
            dl.OnShadowDestroyed += HandleShadowDestroyed;
        }
    }

    public void RegisterTrapLight(TrapLight tl)
    {
        if (tl != null)
        {
            tl.OnShadowTrapped -= HandleShadowTrapped;
            tl.OnShadowTrapped += HandleShadowTrapped;
        }
    }

    public void HandleShadowTrapped()
    {
        if (stateMachine == null)
        {
            Debug.LogWarning("GameplayEventIntegration: StateMachine reference is missing.", this);
            return;
        }

        if (stateMachine.CurrentState != PlayerShadowState.Detached)
        {
            Debug.Log($"GameplayEventIntegration: Trap event ignored. State is {stateMachine.CurrentState}.");
            return;
        }

        Debug.Log("GameplayEventIntegration: Shadow Trapped by hazard!");
        stateMachine.Trap();
    }

    public void HandleShadowDestroyed()
    {
        Debug.LogWarning("GameplayEventIntegration: Shadow destroyed by hazard or failed recovery! Initiating restart.");

        if (restartSystem != null)
        {
            restartSystem.RestartCurrentLevel();
        }
        else
        {
            Debug.LogWarning("GameplayEventIntegration: No RestartSystem assigned, attempting fallback state reset.", this);
            if (stateMachine != null)
            {
                stateMachine.ResetToAttached();
            }
        }
    }
}
