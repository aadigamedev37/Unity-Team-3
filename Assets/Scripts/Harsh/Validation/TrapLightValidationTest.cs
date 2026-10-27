using UnityEngine;

/// <summary>
/// Validates TrapLight detection and its shadow-trapped event.
/// </summary>
public class TrapLightValidationTest : MonoBehaviour, IShadowController
{
    [SerializeField]
    private TrapLight trapLight;

    private int trappedEventCount;

    private void OnEnable()
    {
        if (trapLight == null)
        {
            Debug.LogError(
                "TrapLightValidationTest requires a TrapLight.",
                this);
            return;
        }

        trapLight.OnShadowTrapped += HandleShadowTrapped;
    }

    private void OnDisable()
    {
        if (trapLight != null)
        {
            trapLight.OnShadowTrapped -= HandleShadowTrapped;
        }
    }

    private void HandleShadowTrapped()
    {
        trappedEventCount++;

        Debug.Log(
            $"VALIDATION PASS: TrapLight raised OnShadowTrapped. Count = {trappedEventCount}.");
    }

    public void Activate()
    {
    }

    public void Deactivate()
    {
    }

    public void Stop()
    {
    }

    public void Reset()
    {
    }

    public void HandleMovement(Vector2 input)
    {
    }
}