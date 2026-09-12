using UnityEngine;

/// <summary>
/// Moves a hazard back and forth around its initial local rotation.
/// </summary>
public class SweepingHazard : MonoBehaviour
{
    [Header("Sweep Configuration")]
    [SerializeField]
    private bool sweepEnabled = true;

    [SerializeField]
    private Vector3 rotationAxis = Vector3.up;

    [SerializeField]
    private float sweepAngle = 45f;

    [SerializeField]
    private float sweepSpeed = 30f;

    private Quaternion initialRotation;
    private float currentAngle;

    private void Awake()
    {
        initialRotation = transform.localRotation;
        currentAngle = -sweepAngle;
    }

    private void Update()
    {
        if (!sweepEnabled)
        {
            return;
        }

        currentAngle = Mathf.PingPong(
            Time.time * sweepSpeed,
            sweepAngle * 2f) - sweepAngle;

        transform.localRotation =
            initialRotation *
            Quaternion.AngleAxis(currentAngle, rotationAxis.normalized);
    }

    public void ResetSweep()
    {
        currentAngle = -sweepAngle;
        transform.localRotation = initialRotation;
    }
}