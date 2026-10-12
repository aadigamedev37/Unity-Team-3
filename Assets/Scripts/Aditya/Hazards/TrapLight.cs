using System;
using UnityEngine;

/// <summary>
/// Light hazard volume that traps the Shadow entity, raising OnShadowTrapped
/// to initiate the TRAPPED state and recovery sequence.
/// </summary>
public class TrapLight : LightHazardBase
{
    /// <summary>
    /// Raised when the Shadow enters the active Trap Light volume.
    /// </summary>
    public event Action OnShadowTrapped;

    protected override void HandleHazardEnter(Collider other)
    {
        MonoBehaviour[] behaviours = other.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is not IShadowController)
                continue;

            OnShadowTrapped?.Invoke();
            return;
        }
    }
}
