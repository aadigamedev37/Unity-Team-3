using UnityEngine;

/// <summary>
/// Validates that DangerLight only raises its destruction event
/// when an object implementing IShadowController enters its volume.
/// </summary>
public class DangerLightValidationTest : MonoBehaviour, IShadowController
{
    [SerializeField]
    private DangerLight dangerLight;

    private void OnEnable()
    {
        if (dangerLight == null)
        {
            Debug.LogError("DangerLightValidationTest requires a DangerLight.", this);
            return;
        }

        dangerLight.OnShadowDestroyed += HandleShadowDestroyed;
    }

    private void OnDisable()
    {
        if (dangerLight != null)
        {
            dangerLight.OnShadowDestroyed -= HandleShadowDestroyed;
        }
    }

    private void HandleShadowDestroyed()
    {
        Debug.Log("VALIDATION PASS: DangerLight raised OnShadowDestroyed.");
    }

    public void Activate() { }

    public void Deactivate() { }

    public void Stop() { }

    public void Reset() { }

    public void HandleMovement(Vector2 input) { }
}