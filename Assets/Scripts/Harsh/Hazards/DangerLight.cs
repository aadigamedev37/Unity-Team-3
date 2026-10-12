using System;
using UnityEngine;

/// <summary>
/// Environmental hazard that signals when the Shadow enters
/// its volume and should be destroyed.
/// </summary>
public class DangerLight : LightHazardBase
{
    public event Action OnShadowDestroyed;

    protected override void HandleHazardEnter(Collider other)
    {
        if (!IsShadow(other))
        {
            return;
        }

        OnShadowDestroyed?.Invoke();
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