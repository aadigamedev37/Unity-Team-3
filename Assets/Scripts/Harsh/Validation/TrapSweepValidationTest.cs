using UnityEngine;

/// <summary>
/// Validates that TrapLight and SweepingHazard
/// work together on the same hazard object.
/// </summary>
public class TrapSweepValidationTest : MonoBehaviour
{
    [SerializeField]
    private TrapLight trapLight;

    [SerializeField]
    private SweepingHazard sweepingHazard;

    private Quaternion initialRotation;
    private bool trapEventReceived;

    private void OnEnable()
    {
        if (trapLight != null)
        {
            trapLight.OnShadowTrapped += HandleShadowTrapped;
        }
    }

    private void OnDisable()
    {
        if (trapLight != null)
        {
            trapLight.OnShadowTrapped -= HandleShadowTrapped;
        }
    }

    private void Start()
    {
        if (trapLight == null)
        {
            Debug.LogError(
                "TrapSweepValidationTest requires a TrapLight.",
                this);
            return;
        }

        if (sweepingHazard == null)
        {
            Debug.LogError(
                "TrapSweepValidationTest requires a SweepingHazard.",
                this);
            return;
        }

        if (trapLight.transform != sweepingHazard.transform)
        {
            Debug.LogError(
                "VALIDATION FAIL: TrapLight and SweepingHazard must use the same GameObject.",
                this);
            return;
        }

        initialRotation =
            sweepingHazard.transform.localRotation;

        ValidateSweepReset();
        ValidateTrapConfiguration();
    }

    private void ValidateSweepReset()
    {
        sweepingHazard.enabled = false;

        sweepingHazard.transform.localRotation =
            initialRotation *
            Quaternion.Euler(0f, 30f, 0f);

        sweepingHazard.ResetSweep();

        float difference =
            Quaternion.Angle(
                sweepingHazard.transform.localRotation,
                initialRotation);

        if (difference < 0.01f)
        {
            Debug.Log(
                "VALIDATION PASS: TrapLight sweep reset returned to its deterministic starting rotation.");
        }
        else
        {
            Debug.LogError(
                $"VALIDATION FAIL: TrapLight sweep reset differed by {difference:F3} degrees.");
        }

        sweepingHazard.enabled = true;
    }

    private void ValidateTrapConfiguration()
    {
        Debug.Log(
            "VALIDATION PASS: TrapLight and SweepingHazard are configured on the same hazard object.");
    }

    private void HandleShadowTrapped()
    {
        if (trapEventReceived)
        {
            return;
        }

        trapEventReceived = true;

        Debug.Log(
            "VALIDATION PASS: Sweeping TrapLight raised OnShadowTrapped.");
    }
}