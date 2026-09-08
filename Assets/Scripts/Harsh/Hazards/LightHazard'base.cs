using UnityEngine;

/// <summary>
/// Base framework for environmental light hazards.
/// Handles shared activation state and trigger detection.
/// Derived hazards define their own gameplay response.
/// </summary>
public abstract class LightHazardBase : MonoBehaviour
{
    [Header("Hazard Configuration")]
    [SerializeField]
    private bool isActive = true;

    public bool IsActive => isActive;

    // Shared lifecycle controls allow other systems
    // to enable or disable a hazard without knowing
    // its specific implementation.
    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;

        HandleHazardEnter(other);
    }

    /// <summary>
    /// Implemented by each hazard type to define
    /// what happens when something enters its volume.
    /// </summary>
    protected abstract void HandleHazardEnter(Collider other);
}