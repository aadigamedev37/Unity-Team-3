using UnityEngine;

public class StealablePlatform : StealableBase
{
    [Header("Platform References")]
    [SerializeField] private Collider platformCollider;
    [SerializeField] private MeshRenderer platformRenderer;

    [Header("Movement Configuration")]
    [SerializeField] private Vector3 moveAxis = new Vector3(1f, 0f, 0f);
    [SerializeField] private float moveDistance = 2.0f;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private bool autoMove = true;

    [Header("Frozen Visuals")]
    [SerializeField] private Color frozenColor = new Color(0.4f, 0.85f, 1f, 1f);

    private Vector3 initialPosition;
    private Color originalColor;
    private bool hasOriginalColor = false;

    private void Awake()
    {
        if (platformCollider == null)
        {
            platformCollider = GetComponent<Collider>();
        }

        if (platformRenderer == null)
        {
            platformRenderer = GetComponent<MeshRenderer>();
        }

        if (platformRenderer != null && platformRenderer.material != null)
        {
            originalColor = platformRenderer.material.color;
            hasOriginalColor = true;
        }

        initialPosition = transform.position;

        if (platformCollider == null)
        {
            Debug.LogError("StealablePlatform: Platform Collider is NOT assigned!", this);
            return;
        }

        ApplyCurrentState();
    }

    private void Update()
    {
        // When Solid and autoMove is enabled, move/oscillate the platform
        if (IsSolid && autoMove && moveDistance > 0f)
        {
            float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
            transform.position = initialPosition + moveAxis.normalized * offset;
        }
        // When Frozen, the platform stays completely stationary in place
    }

    protected override StealableState GetStateAfterSteal()
    {
        // A stolen platform becomes FROZEN.
        return StealableState.Frozen;
    }

    protected override void OnStateChanged(StealableState newState)
    {
        ApplyCurrentState();
        Debug.Log("Platform State Changed To: " + newState);
    }

    private void ApplyCurrentState()
    {
        if (platformCollider != null)
        {
            // IMPORTANT:
            // Solid  = collider ON
            // Frozen = collider ON (retains solid collider so player can stand on it)
            // Ghost  = collider OFF
            platformCollider.enabled = !IsGhost;
        }

        if (platformRenderer != null && hasOriginalColor)
        {
            platformRenderer.material.color = IsFrozen ? frozenColor : originalColor;
        }
    }
}