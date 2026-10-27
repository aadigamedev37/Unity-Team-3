using System;
using UnityEngine;

/// <summary>
/// Environmental hazard that signals when the Shadow enters
/// its trigger volume.
/// </summary>
public class TrapLight : LightHazardBase
{
    public event Action OnShadowTrapped;

    protected override void HandleHazardEnter(Collider other)
    {
        if (!IsShadow(other))
        {
            return;
        }

        OnShadowTrapped?.Invoke();
    }

    private bool IsShadow(Collider other)
    {
        MonoBehaviour[] behaviours =
            other.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IShadowController)
            {
                return true;
            }
        }

        return false;
    }
}