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
    [SerializeField] private Renderer[] shadowRenderers;

    private bool isActive = false;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    public bool IsActive => isActive;

    private void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (shadowRenderers == null || shadowRenderers.Length == 0)
        {
            shadowRenderers = GetComponentsInChildren<Renderer>(true);
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

    public void Activate()
    {
        isActive = true;
        gameObject.SetActive(true);
        SetRenderersVisible(true);
        Debug.Log("ShadowMovementAdapter: Shadow Activated.");
    }

    public void Deactivate()
    {
        isActive = false;
        SetRenderersVisible(false);
        Debug.Log("ShadowMovementAdapter: Shadow Deactivated.");
    }

    public void Stop()
    {
        // Zero movement
        if (characterController != null && characterController.enabled)
        {
            characterController.Move(Vector3.zero);
        }
    }

    public void Reset()
    {
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        if (characterController != null)
        {
            characterController.enabled = true;
        }

        isActive = false;
        SetRenderersVisible(false);
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

        if (input.sqrMagnitude < 0.001f)
            return;

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
            transform.forward = moveDir;
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
