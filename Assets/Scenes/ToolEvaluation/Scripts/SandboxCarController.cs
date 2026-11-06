using UnityEngine;

namespace XorinSandbox.Driving
{
    /// <summary>
    /// Arcade rigidbody car controller for the sandbox race track.
    /// Not related to any StealMan production gameplay script.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class SandboxCarController : MonoBehaviour
    {
        [Header("Drive")]
        [SerializeField] private float accelerationForce = 28f;
        [SerializeField] private float maxForwardSpeed = 32f;
        [SerializeField] private float maxReverseSpeed = 10f;
        [SerializeField] private float brakeDeceleration = 40f;

        [Header("Steering")]
        [SerializeField] private float steerRate = 110f;
        [SerializeField] private float steerSpeedThreshold = 6f;

        [Header("Handling")]
        [SerializeField] private float lateralGrip = 9f;
        [SerializeField] private float handbrakeGrip = 2.2f;
        [SerializeField] private float downforce = 18f;
        [SerializeField] private float groundCheckDistance = 0.85f;
        [SerializeField] private LayerMask groundMask = ~0;

        [Header("Wheels (visual only)")]
        [SerializeField] private Transform[] steeringWheels;
        [SerializeField] private Transform[] allWheels;
        [SerializeField] private float maxVisualSteerAngle = 26f;

        private Rigidbody rb;
        private float throttleInput;
        private float steerInput;
        private bool handbrakeInput;
        private bool isGrounded;
        private bool inputEnabled = true;

        public float ForwardSpeed { get; private set; }
        public float SpeedKph => Mathf.Abs(ForwardSpeed) * 3.6f;
        public bool IsGrounded => isGrounded;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        private void OnDisable()
        {
            // Safe shutdown: drop all input so the car cannot keep driving itself.
            throttleInput = 0f;
            steerInput = 0f;
            handbrakeInput = false;
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            if (!enabled)
            {
                throttleInput = 0f;
                steerInput = 0f;
                handbrakeInput = false;
            }
        }

        private void Update()
        {
            if (!inputEnabled)
            {
                return;
            }

            throttleInput = SandboxDriveInput.Throttle;
            steerInput = SandboxDriveInput.Steer;
            handbrakeInput = SandboxDriveInput.HandbrakeHeld;

            UpdateWheelVisuals();
        }

        private void FixedUpdate()
        {
            if (rb == null)
            {
                return;
            }

            Vector3 velocity = rb.linearVelocity;
            ForwardSpeed = Vector3.Dot(velocity, transform.forward);

            isGrounded = Physics.Raycast(
                transform.position + Vector3.up * 0.25f,
                Vector3.down,
                groundCheckDistance + 0.25f,
                groundMask,
                QueryTriggerInteraction.Ignore);

            if (isGrounded)
            {
                ApplyDrive();
                ApplySteering();
                ApplyGrip();
                rb.AddForce(-transform.up * downforce, ForceMode.Acceleration);
            }

            ClampSpeed();
        }

        private void ApplyDrive()
        {
            bool braking = (throttleInput < -0.01f && ForwardSpeed > 0.5f) ||
                           (throttleInput > 0.01f && ForwardSpeed < -0.5f);

            if (braking)
            {
                Vector3 decel = -transform.forward * Mathf.Sign(ForwardSpeed) * brakeDeceleration;
                rb.AddForce(decel, ForceMode.Acceleration);
                return;
            }

            if (Mathf.Abs(throttleInput) > 0.01f && !handbrakeInput)
            {
                rb.AddForce(transform.forward * throttleInput * accelerationForce, ForceMode.Acceleration);
            }
            else if (Mathf.Abs(ForwardSpeed) > 0.05f)
            {
                // Engine braking / rolling resistance.
                rb.AddForce(-transform.forward * Mathf.Sign(ForwardSpeed) * 4f, ForceMode.Acceleration);
            }
        }

        private void ApplySteering()
        {
            if (Mathf.Abs(steerInput) < 0.01f)
            {
                return;
            }

            float speedFactor = Mathf.Clamp01(Mathf.Abs(ForwardSpeed) / steerSpeedThreshold);
            float direction = ForwardSpeed < -0.1f ? -1f : 1f;
            float yaw = steerInput * steerRate * speedFactor * direction * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, yaw, 0f));
        }

        private void ApplyGrip()
        {
            float grip = handbrakeInput ? handbrakeGrip : lateralGrip;
            Vector3 right = transform.right;
            float lateralSpeed = Vector3.Dot(rb.linearVelocity, right);
            rb.AddForce(-right * lateralSpeed * grip, ForceMode.Acceleration);
        }

        private void ClampSpeed()
        {
            Vector3 velocity = rb.linearVelocity;
            float forward = Vector3.Dot(velocity, transform.forward);
            float limit = forward >= 0f ? maxForwardSpeed : maxReverseSpeed;

            if (Mathf.Abs(forward) > limit)
            {
                Vector3 lateral = velocity - transform.forward * forward;
                rb.linearVelocity = lateral + transform.forward * Mathf.Sign(forward) * limit;
            }
        }

        private void UpdateWheelVisuals()
        {
            if (steeringWheels != null)
            {
                float angle = steerInput * maxVisualSteerAngle;
                for (int i = 0; i < steeringWheels.Length; i++)
                {
                    if (steeringWheels[i] != null)
                    {
                        steeringWheels[i].localRotation = Quaternion.Euler(0f, angle, 0f);
                    }
                }
            }

            if (allWheels != null && Mathf.Abs(ForwardSpeed) > 0.01f)
            {
                float spin = ForwardSpeed * 90f * Time.deltaTime;
                for (int i = 0; i < allWheels.Length; i++)
                {
                    if (allWheels[i] != null)
                    {
                        allWheels[i].Rotate(Vector3.right, spin, Space.Self);
                    }
                }
            }
        }

        /// <summary>Hard reset used by respawn and race restart.</summary>
        public void TeleportTo(Vector3 position, Quaternion rotation)
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.SetPositionAndRotation(position, rotation);
            rb.position = position;
            rb.rotation = rotation;
            ForwardSpeed = 0f;
        }
    }
}
