using UnityEngine;

/// <summary>
/// Tutorial controller for Level 02 (The Sandbox).
/// Guides the player through the Attached -> Detached -> Reuniting loop.
/// Pulses the ContextualActionButton to prompt Detach when first moving,
/// and pulses Recall while detached to encourage testing reunion.
/// </summary>
public class L02_TutorialController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private ContextualActionButton actionButton;
    [SerializeField] private InputSystem inputSystem;

    private bool hasMoved = false;

    private void Awake()
    {
        if (stateMachine == null) stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();
        if (actionButton == null) actionButton = FindAnyObjectByType<ContextualActionButton>();
        if (inputSystem == null) inputSystem = FindAnyObjectByType<InputSystem>();
    }

    private void OnEnable()
    {
        if (stateMachine != null)
            stateMachine.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        if (stateMachine != null)
            stateMachine.OnStateChanged -= HandleStateChanged;
    }

    private void Update()
    {
        if (!hasMoved && inputSystem != null && inputSystem.MoveInput.sqrMagnitude > 0.05f)
        {
            hasMoved = true;
            if (actionButton != null)
            {
                actionButton.SetPulsing(true);
            }
        }
    }

    private void HandleStateChanged(PlayerShadowState state)
    {
        if (actionButton == null) return;

        if (state == PlayerShadowState.Detached)
        {
            actionButton.SetPulsing(true);
        }
        else if (state == PlayerShadowState.Attached)
        {
            actionButton.SetPulsing(false);
        }
    }
}
