using UnityEngine;

/// <summary>
/// Completes the RECALL / RECOVER flow at runtime.
///
/// PlayerShadowStateMachine.Recall() and Recover() move the state to Reuniting, but nothing in the
/// M1 scene ever called CompleteReunion(), so manual play stayed stuck in Reuniting (action button
/// disabled with "REUNITING"). This component moves the shadow back to the player's ShadowAnchor
/// and then calls CompleteReunion(), which returns control (and the camera) to the player.
///
/// Runs in LateUpdate so it overrides any joystick-driven shadow movement during the reunion.
/// </summary>
public class ShadowReunionController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private Transform shadowRoot;
    [Tooltip("Point the shadow returns to (Player/ShadowAnchor). Falls back to the player's feet.")]
    [SerializeField] private Transform reunionPoint;

    [Header("Reunion Motion")]
    [SerializeField] private float returnSpeed = 10f;
    [SerializeField] private float arrivalDistance = 0.1f;
    [Tooltip("Hard cap so the reunion always completes, even if the path is blocked.")]
    [SerializeField] private float maxReunionDuration = 1.2f;

    private CharacterController shadowCharacterController;
    private bool isReuniting;
    private float reunionTimer;

    public bool IsReuniting => isReuniting;

    private void Awake()
    {
        if (stateMachine == null) stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();

        if (shadowRoot == null)
        {
            var shadow = FindAnyObjectByType<ShadowMovementAdapter>();
            if (shadow != null) shadowRoot = shadow.transform;
        }

        if (reunionPoint == null)
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                Transform anchor = player.transform.Find("ShadowAnchor");
                reunionPoint = anchor != null ? anchor : player.transform;
            }
        }

        if (shadowRoot != null)
        {
            shadowCharacterController = shadowRoot.GetComponent<CharacterController>();
        }
    }

    private void OnEnable()
    {
        if (stateMachine != null) stateMachine.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (stateMachine != null) stateMachine.OnStateChanged -= HandleStateChanged;
        isReuniting = false;
    }

    private void HandleStateChanged(PlayerShadowState newState)
    {
        isReuniting = newState == PlayerShadowState.Reuniting;
        reunionTimer = 0f;
    }

    private void LateUpdate()
    {
        if (!isReuniting || stateMachine == null) return;

        if (stateMachine.CurrentState != PlayerShadowState.Reuniting)
        {
            isReuniting = false;
            return;
        }

        reunionTimer += Time.deltaTime;

        if (shadowRoot == null || reunionPoint == null)
        {
            Finish();
            return;
        }

        Vector3 current = shadowRoot.position;
        Vector3 goal = reunionPoint.position;
        Vector3 next = Vector3.MoveTowards(current, goal, returnSpeed * Time.deltaTime);

        Vector3 flatDir = goal - current;
        flatDir.y = 0f;

        SetShadowPose(next, flatDir.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(flatDir) : shadowRoot.rotation);

        if (Vector3.Distance(next, goal) <= arrivalDistance || reunionTimer >= maxReunionDuration)
        {
            SetShadowPose(goal, shadowRoot.rotation);
            Finish();
        }
    }

    private void SetShadowPose(Vector3 position, Quaternion rotation)
    {
        bool ccWasEnabled = shadowCharacterController != null && shadowCharacterController.enabled;
        if (ccWasEnabled) shadowCharacterController.enabled = false;
        shadowRoot.SetPositionAndRotation(position, rotation);
        if (ccWasEnabled) shadowCharacterController.enabled = true;
    }

    private void Finish()
    {
        isReuniting = false;
        if (stateMachine.CurrentState == PlayerShadowState.Reuniting)
        {
            stateMachine.CompleteReunion();
        }
    }
}
