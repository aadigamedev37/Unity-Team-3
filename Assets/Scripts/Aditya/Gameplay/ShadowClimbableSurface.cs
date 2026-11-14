using UnityEngine;

/// <summary>
/// Explicit marker component designating a surface that the Shadow can climb.
/// Walls without this component (and not on the designated climbable layer)
/// are treated as solid, impassable obstacles that cannot be traversed.
/// </summary>
[DisallowMultipleComponent]
public class ShadowClimbableSurface : MonoBehaviour
{
    [Tooltip("If true, allows the shadow to traverse this surface.")]
    [SerializeField] private bool isClimbable = true;

    [Tooltip("Optional identifier or description for level designers.")]
    [SerializeField] private string surfaceId = "ClimbableConduitWall";

    public bool IsClimbable
    {
        get => isClimbable;
        set => isClimbable = value;
    }

    public string SurfaceId => surfaceId;

    private void Awake()
    {
        // Automatically ensure collider exists on this object or children
        if (GetComponent<Collider>() == null && GetComponentInChildren<Collider>() == null)
        {
            Debug.LogWarning($"[ShadowClimbableSurface] No collider found on {gameObject.name}. Shadow climbing requires a collider.", this);
        }
    }
}
