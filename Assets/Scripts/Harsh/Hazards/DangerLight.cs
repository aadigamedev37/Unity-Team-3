using System;
using UnityEngine;

/// <summary>
/// Static light hazard that detects the Shadow entering its volume
/// and raises the destruction event required by the level systems.
/// </summary>
public class DangerLight : LightHazardBase
{
    /// <summary>
    /// Raised when the Shadow enters the active Danger Light volume.
    /// The Shadow/gameplay system is responsible for handling the
    /// actual destruction response.
    /// </summary>
    public event Action OnShadowDestroyed;

    protected override void HandleHazardEnter(Collider other)
    {
        // Find an existing Shadow implementation through the established interface.
        MonoBehaviour[] behaviours = other.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is not IShadowController)
                continue;

            OnShadowDestroyed?.Invoke();
            return;
        }
    }
}