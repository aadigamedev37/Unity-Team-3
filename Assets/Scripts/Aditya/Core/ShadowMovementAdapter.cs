using UnityEngine;

/// <summary>
/// Universal adapter component for the Shadow entity.
/// Implements IShadowController, decoupling InputSystem from specific
/// shadow controller or physics internals (CharacterController, StarterAssets, SurfaceTraversal).
/// </summary>
public class ShadowMovementAdapter : MonoBehaviour, IShadowController
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private Transform movementReference;

    [Header("Components (Auto-detected if unassigned)")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private Renderer[] shadowRenderers;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothTime = 0.12f;

    [Header("Spawn Settings (Auto-detected if unassigned)")]
    [SerializeField] private Transform spawnAnchor;

    private bool isActive = false;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private float rotationVelocity;

    public bool IsActive => isActive;

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (shadowRenderers == null || shadowRenderers.Length == 0)
        {
            shadowRenderers = GetComponentsInChildren<Renderer>(true);
        }

        if (spawnAnchor == null)
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                Transform anchor = player.transform.Find("ShadowAnchor");
                spawnAnchor = anchor != null ? anchor : player.transform;
            }
        }

        // Deconflict: Disable ThirdPersonController and PlayerInput on Shadow so they do not fight this adapter
        var tpc = GetComponent<StarterAssets.ThirdPersonController>();
        if (tpc != null)
        {
            tpc.enabled = false;
        }

        var pi = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (pi != null)
        {
            pi.enabled = false;
        }

        if (movementReference == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                movementReference = mainCam.transform;
            }
            else
            {
                movementReference = transform;
            }
        }

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    private void Start()
    {
        // At start, the player and shadow begin in the Attached state.
        // Only the player must be present and visible in the beginning.
        var sm = FindAnyObjectByType<PlayerShadowStateMachine>();
        if (sm == null || sm.CurrentState == PlayerShadowState.Attached)
        {
            SnapToAnchor();
            Deactivate();
        }
    }

    private void SnapToAnchor()
    {
        if (spawnAnchor == null)
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                Transform anchor = player.transform.Find("ShadowAnchor");
                spawnAnchor = anchor != null ? anchor : player.transform;
            }
        }

        if (spawnAnchor != null)
        {
            bool ccWasEnabled = characterController != null && characterController.enabled;
            if (ccWasEnabled) characterController.enabled = false;
            transform.SetPositionAndRotation(spawnAnchor.position, spawnAnchor.rotation);
            if (ccWasEnabled) characterController.enabled = true;
        }
    }

    public void Activate()
    {
        isActive = true;
        gameObject.SetActive(true);

        SnapToAnchor();

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        SetRenderersVisible(true);

        if (animator != null)
        {
            animator.enabled = true;
            animator.SetBool("Isdetach", true);
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("MotionSpeed", 1f);
        }

        Debug.Log("ShadowMovementAdapter: Shadow Activated.");
    }

    public void Deactivate()
    {
        isActive = false;
        if (characterController != null)
        {
            characterController.enabled = false;
        }
        SetRenderersVisible(false);

        if (animator != null)
        {
            animator.SetBool("Isdetach", false);
            animator.SetFloat("Speed", 0f);
        }

        Debug.Log("ShadowMovementAdapter: Shadow Deactivated.");
    }

    public void Stop()
    {
        if (characterController != null && characterController.enabled)
        {
            characterController.Move(Vector3.zero);
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    public void Reset()
    {
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        SnapToAnchor();

        isActive = false;
        SetRenderersVisible(false);

        if (animator != null)
        {
            animator.SetBool("Isdetach", false);
            animator.SetFloat("Speed", 0f);
        }
    }

    public void SetSpawnPosition(Vector3 position, Quaternion rotation)
    {
        spawnPosition = position;
        spawnRotation = rotation;
    }

    public void HandleMovement(Vector2 input)
    {
        if (!isActive)
            return;

        float inputMag = input.magnitude;
        if (inputMag < 0.001f)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetFloat("MotionSpeed", 1f);
            }
            return;
        }

        Vector3 forward = movementReference != null ? movementReference.forward : Vector3.forward;
        Vector3 right = movementReference != null ? movementReference.right : Vector3.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (right * input.x + forward * input.y).normalized;
        Vector3 delta = moveDir * (moveSpeed * Time.deltaTime);

        if (characterController != null && characterController.enabled)
        {
            characterController.Move(delta);
        }
        else
        {
            transform.position += delta;
        }

        if (moveDir != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        if (animator != null)
        {
            float targetSpeed = Mathf.Clamp(moveSpeed * inputMag, 0f, 6f);
            animator.SetFloat("Speed", targetSpeed);
            animator.SetFloat("MotionSpeed", 1f);
        }
    }

    private void SetRenderersVisible(bool visible)
    {
        if (shadowRenderers == null)
            return;

        foreach (var r in shadowRenderers)
        {
            if (r != null)
            {
                r.enabled = visible;
            }
        }
    }
}
