
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbitController : MonoBehaviour
{
    [Header("Camera Target")]
    [SerializeField] private Transform cameraTarget;

    [Header("Camera Settings")]
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;

    [Header("Input Settings")]
    [SerializeField] private bool invertY = false;

    private float yaw;
    private float pitch;

    private void Start()
    {
        if (cameraTarget == null)
        {
            cameraTarget = transform;
        }

        Vector3 angles = cameraTarget.eulerAngles;

        yaw = angles.y;
        pitch = angles.x;

        if (pitch > 180f)
        {
            pitch -= 360f;
        }
    }

    private void Update()
    {
        Vector2 lookInput = Vector2.zero;

        // Laptop / PC Mouse
        if (Mouse.current != null)
        {
            lookInput = Mouse.current.delta.ReadValue();
        }

        // Mobile Touch
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            lookInput = Touchscreen.current.primaryTouch.delta.ReadValue();
        }

        yaw += lookInput.x * sensitivity;

        float verticalInput = invertY
            ? lookInput.y
            : -lookInput.y;

        pitch += verticalInput * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
