using System;
using UnityEngine;

/// <summary>
/// Authoritative static gameplay events for Steal the Shadow.
/// Allows decoupled observation of core lifecycle moments across UI, Audio, VFX, and Levels.
/// </summary>
public static class GameEvents
{
    /// <summary>Fired when a level exit trigger is activated by the player body.</summary>
    public static event Action<int> OnLevelComplete;

    /// <summary>Fired when the shadow is detached from the player.</summary>
    public static event Action OnShadowDetached;

    /// <summary>Fired when the shadow finishes reunion with the player.</summary>
    public static event Action OnShadowRecalled;

    public static void RaiseLevelComplete(int levelId)
    {
        Debug.Log($"[GameEvents] OnLevelComplete raised for Level {levelId}");
        OnLevelComplete?.Invoke(levelId);
    }

    public static void RaiseShadowDetached()
    {
        Debug.Log("[GameEvents] OnShadowDetached raised");
        OnShadowDetached?.Invoke();
    }

    public static void RaiseShadowRecalled()
    {
        Debug.Log("[GameEvents] OnShadowRecalled raised");
        OnShadowRecalled?.Invoke();
    }
}
