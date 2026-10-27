using TMPro;
using UnityEngine;

/// <summary>
/// World-space gameplay label (TextMeshPro) that floats above a target GameObject, faces the
/// gameplay camera, keeps a roughly constant on-screen size, and shows the target's live
/// interaction state read from the existing gameplay components (no separate state machine).
///
/// The label is NOT parented to its target, so it never inherits non-uniform scale or rotation
/// (e.g. the scaled door/platform meshes) and keeps following the target while it moves,
/// lowers, freezes or becomes a ghost.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(TextMeshPro))]
public class GameplayObjectLabel : MonoBehaviour
{
    public enum LabelKind
    {
        Static,
        Player,
        Shadow,
        StealableDoor,
        StealableDrawbridge,
        StealablePlatform,
        DangerLight,
        TrapLight,
        ExitZone,
        Structure
    }

    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private LabelKind kind = LabelKind.Static;
    [SerializeField] private string displayName = "OBJECT";
    [Tooltip("Status line for Static/Structure labels (optional).")]
    [SerializeField] private string staticStatus = "";

    [Header("Placement")]
    [Tooltip("Distance above the top of the target's renderer bounds.")]
    [SerializeField] private float heightAboveBounds = 0.5f;
    [SerializeField] private Vector3 worldOffset = Vector3.zero;

    [Header("Style")]
    [SerializeField] private Color nameColor = Color.white;
    [SerializeField] private Color statusColor = new Color(1f, 0.84f, 0.35f, 1f);
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f, 0.6f);
    [SerializeField, Range(0.4f, 2f)] private float sizeMultiplier = 1f;

    [Header("Constant Screen Size")]
    [Tooltip("Label scale per metre of camera distance (keeps labels legible as the camera follows).")]
    [SerializeField] private float scalePerMeter = 0.075f;
    [SerializeField] private float minScale = 0.4f;
    [SerializeField] private float maxScale = 2.0f;

    [Header("Mobile Control Avoidance")]
    [Tooltip("Alpha used when the label overlaps the joystick, action button or HUD.")]
    [SerializeField, Range(0f, 1f)] private float overlappedAlpha = 0.12f;
    [SerializeField] private float controlMarginPixels = 16f;

    [Header("State Sources (auto-resolved)")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;

    private TextMeshPro tmp;
    private StealableBase stealable;
    private Renderer[] targetRenderers;
    private Collider targetCollider;
    private Vector3 lastTargetPosition;
    private float movementSpeed;
    private string lastText;
    private float currentAlpha = 1f;

    private static RectTransform[] controlRects;
    private static int controlRectsFrame = -1;
    private static readonly Vector3[] Corners = new Vector3[4];

    public Transform Target => target;
    public LabelKind Kind => kind;
    public string DisplayName => displayName;
    public string CurrentStatus { get; private set; } = "";
    public bool IsVisible => tmp != null && tmp.enabled;
    public bool IsOverlappingControls { get; private set; }

    /// <summary>Editor/setup helper used when building labels from tooling.</summary>
    public void Configure(Transform newTarget, LabelKind newKind, string newName, string newStaticStatus,
        float newHeight, Color newStatusColor, float newSizeMultiplier)
    {
        target = newTarget;
        kind = newKind;
        displayName = newName;
        staticStatus = newStaticStatus;
        heightAboveBounds = newHeight;
        statusColor = newStatusColor;
        sizeMultiplier = newSizeMultiplier;
        CacheTargetComponents();
        lastText = null;
    }

    private void OnEnable()
    {
        tmp = GetComponent<TextMeshPro>();
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.richText = true;

        if (stateMachine == null)
        {
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();
        }

        CacheTargetComponents();
        lastText = null;
    }

    private void CacheTargetComponents()
    {
        if (target == null)
        {
            stealable = null;
            targetRenderers = null;
            targetCollider = null;
            return;
        }

        stealable = target.GetComponent<StealableBase>();
        targetRenderers = target.GetComponentsInChildren<Renderer>(true);
        targetCollider = target.GetComponent<Collider>();
        lastTargetPosition = target.position;
    }

    private void LateUpdate()
    {
        if (tmp == null) return;

        bool visible = ShouldBeVisible();
        if (tmp.enabled != visible) tmp.enabled = visible;
        if (!visible) return;

        TrackMovement();

        Camera cam = Camera.main;
        Vector3 anchor = GetAnchorPosition();
        transform.position = anchor;

        if (cam != null)
        {
            // Billboard parallel to the camera plane (no skew, always readable).
            transform.rotation = cam.transform.rotation;

            float distance = Vector3.Distance(cam.transform.position, anchor);
            float scale = Mathf.Clamp(distance * scalePerMeter, minScale, maxScale) * sizeMultiplier;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        CurrentStatus = ResolveStatus();
        string text = BuildText(CurrentStatus);
        if (text != lastText)
        {
            tmp.text = text;
            lastText = text;
        }

        UpdateControlAvoidance(cam);
    }

    private bool ShouldBeVisible()
    {
        if (target == null || !target.gameObject.activeInHierarchy) return false;

        if (kind == LabelKind.Shadow)
        {
            // The detached shadow is only visible while its renderers are shown by ShadowMovementAdapter.
            if (stateMachine != null && stateMachine.CurrentState == PlayerShadowState.Attached) return false;
            if (!AnyRendererEnabled()) return false;
        }

        return true;
    }

    private bool AnyRendererEnabled()
    {
        if (targetRenderers == null) return false;
        foreach (var r in targetRenderers)
        {
            if (r != null && r.enabled && r.gameObject.activeInHierarchy) return true;
        }
        return false;
    }

    private void TrackMovement()
    {
        float dt = Application.isPlaying ? Time.deltaTime : 0f;
        Vector3 pos = target.position;
        float instantaneous = dt > 0f ? (pos - lastTargetPosition).magnitude / dt : 0f;
        movementSpeed = Mathf.Lerp(movementSpeed, instantaneous, 0.2f);
        lastTargetPosition = pos;
    }

    private Vector3 GetAnchorPosition()
    {
        bool hasBounds = false;
        Bounds bounds = new Bounds(target.position, Vector3.zero);

        if (targetRenderers != null)
        {
            foreach (var r in targetRenderers)
            {
                if (r == null || !r.enabled || !r.gameObject.activeInHierarchy) continue;
                if (r is TMP_SubMesh || r.GetComponent<TMP_Text>() != null) continue;

                if (!hasBounds)
                {
                    bounds = r.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(r.bounds);
                }
            }
        }

        if (!hasBounds && targetCollider != null)
        {
            bounds = targetCollider.bounds;
            hasBounds = true;
        }

        float topY = hasBounds ? bounds.max.y : target.position.y;
        Vector3 center = hasBounds ? bounds.center : target.position;
        return new Vector3(center.x, topY + heightAboveBounds, center.z) + worldOffset;
    }

    private string ResolveStatus()
    {
        PlayerShadowState state = stateMachine != null ? stateMachine.CurrentState : PlayerShadowState.Attached;

        switch (kind)
        {
            case LabelKind.Player:
                switch (state)
                {
                    case PlayerShadowState.Attached: return "CONTROLLED";
                    case PlayerShadowState.Reuniting: return "REUNITING";
                    default: return "WAITING — SHADOW DETACHED";
                }

            case LabelKind.Shadow:
                switch (state)
                {
                    case PlayerShadowState.Detached: return "CONTROLLED";
                    case PlayerShadowState.Trapped:
                        return stateMachine != null && stateMachine.CanPerformAction(InputAction.Recover)
                            ? "TRAPPED — TAP RECOVER"
                            : "TRAPPED";
                    case PlayerShadowState.Reuniting: return "RETURNING TO PLAYER";
                    default: return "ATTACHED";
                }

            case LabelKind.StealableDoor:
                if (stealable == null) return staticStatus;
                if (stealable.IsGhost) return "GHOST — PASSABLE";
                if (stealable.IsFrozen) return "FROZEN";
                return "SOLID";

            case LabelKind.StealableDrawbridge:
                if (stealable == null) return staticStatus;
                if (stealable.IsGhost)
                {
                    return targetCollider == null || targetCollider.enabled ? "LOWERED — WALKABLE" : "LOWERED";
                }
                return "RAISED";

            case LabelKind.StealablePlatform:
                if (stealable == null) return staticStatus;
                if (stealable.IsFrozen) return "FROZEN — SOLID";
                if (stealable.IsGhost) return "GHOST";
                return !Application.isPlaying || movementSpeed > 0.05f ? "MOVING" : "SOLID";

            case LabelKind.DangerLight:
                return "DESTROYS SHADOW";

            case LabelKind.TrapLight:
                return state == PlayerShadowState.Trapped ? "SHADOW TRAPPED" : "TRAPS SHADOW";

            case LabelKind.ExitZone:
                return "COMPLETE LEVEL";

            default:
                return staticStatus;
        }
    }

    private string BuildText(string status)
    {
        string bg = ColorUtility.ToHtmlStringRGBA(backgroundColor);
        string nameHex = ColorUtility.ToHtmlStringRGBA(nameColor);
        string statusHex = ColorUtility.ToHtmlStringRGBA(statusColor);

        string text = $"<mark=#{bg} padding=\"14,14,6,6\"><b><color=#{nameHex}>{displayName}</color></b>";
        if (!string.IsNullOrEmpty(status))
        {
            text += $"\n<size=70%><color=#{statusHex}>{status}</color></size>";
        }
        return text + "</mark>";
    }

    private void UpdateControlAvoidance(Camera cam)
    {
        IsOverlappingControls = false;

        if (Application.isPlaying && cam != null)
        {
            ResolveControlRects();
            Rect labelRect = GetScreenRect(cam);

            if (controlRects != null && labelRect.width > 0f)
            {
                foreach (var rt in controlRects)
                {
                    if (rt == null || !rt.gameObject.activeInHierarchy) continue;
                    rt.GetWorldCorners(Corners); // Screen Space - Overlay: world corners == screen pixels
                    Rect controlRect = Rect.MinMaxRect(
                        Corners[0].x - controlMarginPixels, Corners[0].y - controlMarginPixels,
                        Corners[2].x + controlMarginPixels, Corners[2].y + controlMarginPixels);

                    if (controlRect.Overlaps(labelRect))
                    {
                        IsOverlappingControls = true;
                        break;
                    }
                }
            }
        }

        float targetAlpha = IsOverlappingControls ? overlappedAlpha : 1f;
        currentAlpha = Application.isPlaying ? Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * 6f) : 1f;
        if (!Mathf.Approximately(tmp.alpha, currentAlpha)) tmp.alpha = currentAlpha;
    }

    private Rect GetScreenRect(Camera cam)
    {
        Bounds b = tmp.bounds; // local-space text bounds
        if (b.size == Vector3.zero) return Rect.zero;

        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        for (int i = 0; i < 4; i++)
        {
            Vector3 local = new Vector3(i % 2 == 0 ? b.min.x : b.max.x, i < 2 ? b.min.y : b.max.y, 0f);
            Vector3 sp = cam.WorldToScreenPoint(transform.TransformPoint(local));
            if (sp.z < 0f) return Rect.zero;
            min = Vector2.Min(min, sp);
            max = Vector2.Max(max, sp);
        }
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    private static void ResolveControlRects()
    {
        if (controlRectsFrame >= 0 && controlRects != null && Time.frameCount - controlRectsFrame < 120) return;
        controlRectsFrame = Time.frameCount;

        var list = new System.Collections.Generic.List<RectTransform>();
        var joystick = FindAnyObjectByType<VirtualJoystick>();
        if (joystick != null) list.Add((RectTransform)joystick.transform);
        var button = FindAnyObjectByType<ContextualActionButton>();
        if (button != null) list.Add((RectTransform)button.transform);
        var hud = GameObject.Find("M1_HUD");
        if (hud != null && hud.transform is RectTransform hudRect) list.Add(hudRect);
        controlRects = list.ToArray();
    }
}
