using UnityEngine;
using System;

public enum InputTarget
{
    Player,
    Shadow
}

public enum InputAction
{
    Detach,
    Recall,
    Steal,
    Recover
}

public class InputSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private PlayerShadowStateMachine stateMachine;

    [Header("Input")]
    [SerializeField] private InputTarget activeTarget = InputTarget.Player;

    [Header("Movement References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private MonoBehaviour shadowControllerTarget;

    [Header("Cursor Settings (Editor / Mobile Testing)")]
    [Tooltip("If true and a virtual joystick is present (mobile UI mode), ensures cursor remains unlocked and visible in the Editor.")]
    [SerializeField] private bool unlockCursorForMobileUI = true;

    private IShadowController shadowController;

    public Vector2 MoveInput { get; private set; }
    public InputTarget ActiveTarget => activeTarget;

    public event Action<InputAction> OnAction;

    private void Awake()
    {
        MoveInput = Vector2.zero;
        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
        }
        ResolveShadowController();
    }

    private void Start()
    {
#if UNITY_EDITOR
        ConfigureCursorForEditorTesting();
#endif
        // Enforce initial Attached state: Player active, Shadow deactivated and hidden
        HandleStateChanged(stateMachine != null ? stateMachine.CurrentState : PlayerShadowState.Attached);
    }

#if UNITY_EDITOR
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            ConfigureCursorForEditorTesting();
        }
    }

    private void ConfigureCursorForEditorTesting()
    {
        if (unlockCursorForMobileUI && virtualJoystick != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            
            var starterInputs = FindObjectsByType<StarterAssets.StarterAssetsInputs>();
            foreach (var input in starterInputs)
            {
                if (input != null)
                {
                    input.cursorLocked = false;
                    input.cursorInputForLook = false;
                }
            }
        }
    }
#endif

    private void OnValidate()
    {
        ResolveShadowController();
    }

    private void ResolveShadowController()
    {
        if (shadowControllerTarget != null)
        {
            shadowController = shadowControllerTarget as IShadowController;
            if (shadowController == null)
            {
                Debug.LogWarning("InputSystem: shadowControllerTarget does not implement IShadowController.", this);
            }
        }
        else if (shadowController == null)
        {
            var sma = FindAnyObjectByType<ShadowMovementAdapter>();
            if (sma != null)
            {
                shadowController = sma;
                shadowControllerTarget = sma;
            }
        }
    }

    public void SetShadowController(IShadowController controller)
    {
        shadowController = controller;
        shadowControllerTarget = controller as MonoBehaviour;
    }

    public void SetPlayerController(PlayerController controller)
    {
        playerController = controller;
    }

    public void SetStateMachine(PlayerShadowStateMachine sm)
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }

        stateMachine = sm;

        if (stateMachine != null)
        {
            stateMachine.OnStateChanged += HandleStateChanged;
        }
    }

    private void Update()
    {
        Vector2 input = Vector2.zero;
        if (virtualJoystick != null && virtualJoystick.Direction != Vector2.zero)
        {
            input = virtualJoystick.Direction;
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        else
        {
            float h = 0f;
            float v = 0f;
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v -= 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
            }
            if (h == 0f && v == 0f)
            {
                try
                {
                    h = Input.GetAxisRaw("Horizontal");
                    v = Input.GetAxisRaw("Vertical");
                }
                catch { }
            }
            input = Vector2.ClampMagnitude(new Vector2(h, v), 1f);
        }

        // Action hotkeys: Q (Hung's standard shadow toggle), Space, E
        bool actionPressed = false;
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            actionPressed = kb.qKey.wasPressedThisFrame || kb.eKey.wasPressedThisFrame || kb.spaceKey.wasPressedThisFrame;
        }
        if (!actionPressed)
        {
            try
            {
                actionPressed = Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space);
            }
            catch { }
        }

        if (actionPressed)
        {
            TriggerContextualAction();
        }
#endif

        MoveInput = input;

        if (activeTarget == InputTarget.Player && playerController != null)
        {
            playerController.HandleMovement(MoveInput);
        }
        else if (activeTarget == InputTarget.Shadow && shadowController != null)
        {
            shadowController.HandleMovement(MoveInput);
        }
    }

    public void TriggerContextualAction()
    {
        var actionButton = FindAnyObjectByType<ContextualActionButton>();
        if (actionButton != null)
        {
            actionButton.TriggerCurrentAction();
        }
        else if (stateMachine != null)
        {
            switch (stateMachine.CurrentState)
            {
                case PlayerShadowState.Attached:
                    TriggerDetach();
                    break;
                case PlayerShadowState.Detached:
                    TriggerRecall();
                    break;
                case PlayerShadowState.Trapped:
                    TriggerRecover();
                    break;
            }
        }
    }

    private int lastActionFrame = -1;
    private InputAction lastAction;

    public void TriggerAction(InputAction action)
    {
        // Prevent accidental double-trigger in the same frame (e.g. duplicate click or Detach immediately followed by Recall)
        if (Application.isPlaying && Time.frameCount == lastActionFrame && (action == lastAction || (lastAction == InputAction.Detach && action == InputAction.Recall)))
        {
            return;
        }

        if (stateMachine == null)
        {
            Debug.LogWarning("InputSystem: StateMachine reference is missing.");
            return;
        }

        bool actionSuccessful = false;

        switch (action)
        {
            case InputAction.Detach:
                actionSuccessful = stateMachine.Detach();
                break;

            case InputAction.Recall:
                actionSuccessful = stateMachine.Recall();
                break;

            case InputAction.Steal:
                actionSuccessful = stateMachine.Steal();
                break;

            case InputAction.Recover:
                actionSuccessful = stateMachine.Recover();
                break;

            default:
                Debug.LogWarning("Unhandled action: " + action);
                break;
        }

        if (actionSuccessful)
        {
            lastActionFrame = Time.frameCount;
            lastAction = action;
            OnAction?.Invoke(action);
        }
    }

    public void TriggerDetach()
    {
        TriggerAction(InputAction.Detach);
    }

    public void TriggerRecall()
    {
        TriggerAction(InputAction.Recall);
    }

    public void TriggerSteal()
    {
        TriggerAction(InputAction.Steal);
    }

    public void TriggerRecover()
    {
        TriggerAction(InputAction.Recover);
    }

    public void SetActiveTarget(InputTarget target)
    {
        activeTarget = target;
    }

    private void HandleStateChanged(PlayerShadowState newState)
    {
        switch (newState)
        {
            case PlayerShadowState.Attached:
                activeTarget = InputTarget.Player;

                if (playerController != null)
                {
                    playerController.Unfreeze();
                }

                if (shadowController != null)
                {
                    shadowController.Deactivate();
                }
                break;

            case PlayerShadowState.Detached:
                activeTarget = InputTarget.Shadow;

                if (playerController != null)
                {
                    playerController.Freeze();
                }

                if (shadowController != null)
                {
                    shadowController.Activate();
                }
                break;

            case PlayerShadowState.Reuniting:
                activeTarget = InputTarget.Shadow;

                if (playerController != null)
                {
                    playerController.Freeze();
                }

                if (shadowController != null)
                {
                    shadowController.Stop();
                }
                break;

            case PlayerShadowState.Trapped:
                activeTarget = InputTarget.Shadow;

                if (playerController != null)
                {
                    playerController.Freeze();
                }

                if (shadowController != null)
                {
                    shadowController.Stop();
                }
                break;
        }
    }

    private void OnEnable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
            stateMachine.OnStateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }
    }
}