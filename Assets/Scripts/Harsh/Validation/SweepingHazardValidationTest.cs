using UnityEngine;

/// <summary>
/// Validates that a sweeping hazard always returns
/// to its original rotation when reset.
/// </summary>
public class SweepingHazardValidationTest : MonoBehaviour
{
    [SerializeField]
    private SweepingHazard sweepingHazard;

    private Quaternion initialRotation;

    private void Start()
    {
        if (sweepingHazard == null)
        {
            Debug.LogError(
                "SweepingHazardValidationTest requires a SweepingHazard.",
                this);
            return;
        }

        initialRotation = sweepingHazard.transform.localRotation;

        sweepingHazard.enabled = false;

        sweepingHazard.transform.localRotation =
            initialRotation * Quaternion.Euler(0f, 30f, 0f);

        sweepingHazard.ResetSweep();

        float angleDifference =
            Quaternion.Angle(
                sweepingHazard.transform.localRotation,
                initialRotation);

        if (angleDifference < 0.01f)
        {
            Debug.Log(
                "VALIDATION PASS: Sweep reset returned to the deterministic starting rotation.");
        }
        else
        {
            Debug.LogError(
                $"VALIDATION FAIL: Sweep reset differed by {angleDifference:F3} degrees.");
        }
    }
}