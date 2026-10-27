using UnityEngine;

public class StealableDoor : StealableBase
{
    [Header("Door References")]
    [SerializeField] private Collider doorCollider;
    [SerializeField] private MeshRenderer doorRenderer;

    [Header("Ghost Visuals")]
    [SerializeField] private Color ghostColor = new Color(0.3f, 0.7f, 1f, 0.35f);

    private Color originalColor;
    private bool hasOriginalColor = false;

    private void Awake()
    {
        if (doorCollider == null)
        {
            doorCollider = GetComponent<Collider>();
        }

        if (doorRenderer == null)
        {
            doorRenderer = GetComponent<MeshRenderer>();
        }

        if (doorRenderer != null && doorRenderer.material != null)
        {
            originalColor = doorRenderer.material.color;
            hasOriginalColor = true;
        }

        if (doorCollider == null)
        {
            Debug.LogError("StealableDoor: Physical Door Collider is NOT assigned!", this);
            return;
        }

        ApplyCurrentState();
    }

    protected override StealableState GetStateAfterSteal()
    {
        return StealableState.Ghost;
    }

    protected override void OnStateChanged(StealableState newState)
    {
        ApplyCurrentState();
        Debug.Log("Door State Changed To: " + newState);
    }

    private void ApplyCurrentState()
    {
        if (doorCollider != null)
        {
            doorCollider.enabled = !IsGhost;
        }

        if (doorRenderer != null)
        {
            // When Ghost, disable cast shadow (shadow was stolen) and tint transparently
            doorRenderer.shadowCastingMode = IsGhost 
                ? UnityEngine.Rendering.ShadowCastingMode.Off 
                : UnityEngine.Rendering.ShadowCastingMode.On;

            if (doorRenderer.material != null && hasOriginalColor)
            {
                doorRenderer.material.color = IsGhost ? ghostColor : originalColor;
            }
        }
    }
}