
using UnityEngine;
using System;
using StarterAssets;
using UnityEngine.InputSystem;

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

    [Header("Movement Controllers")]
    [SerializeField] private ThirdPersonController playerController;
    [SerializeField] private ThirdPersonController shadowController;

    [Header("Input Settings")]
    [SerializeField] private InputTarget activeTarget = InputTarget.Player;

    [Header("Cursor Settings")]
    [SerializeField] private bool unlockCursorForMobileUI = true;

    public Vector2 MoveInput { get; private set; }

    public InputTarget ActiveTarget => activeTarget;

    public event Action<InputAction> OnAction;

    private int lastActionFrame = -1;
    private InputAction lastAction;

    private void Awake()
    {
        MoveInput = Vector2.zero;
    }

    private void Start()
    {
#if UNITY_EDITOR
        ConfigureCursorForEditorTesting();
#endif

        if (stateMachine != null)
        {
            HandleStateChanged(stateMachine.CurrentState);
        }
        else
        {
            activeTarget = InputTarget.Player;
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

        ClearAllMovementInput();
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
        if (!unlockCursorForMobileUI || virtualJoystick == null)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        var starterInputs =
            FindObjectsByType<StarterAssetsInputs>(
                FindObjectsSortMode.None);

        foreach (var input in starterInputs)
        {
            if (input != null)
            {
                input.cursorLocked = false;
                input.cursorInputForLook = false;
            }
        }
    }
#endif

    private void Update()
    {
        ReadMovementInput();
        HandleKeyboardActions();
        SendMovementInput();
    }

    // ----------------------------------
    // MOVEMENT INPUT
    // ----------------------------------

    private void ReadMovementInput()
    {
        Vector2 input = Vector2.zero;

        // Virtual joystick
        if (virtualJoystick != null)
        {
            input = virtualJoystick.Direction;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        // Keyboard testing
        if (input == Vector2.zero)
        {
            if (Keyboard.current != null)
            {
                var kb = Keyboard.current;

                float h = 0f;
                float v = 0f;

                if (kb.wKey.isPressed || kb.upArrowKey.isPressed)
                    v += 1f;

                if (kb.sKey.isPressed || kb.downArrowKey.isPressed)
                    v -= 1f;

                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)
                    h -= 1f;

                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed)
                    h += 1f;

                input = new Vector2(h, v);
            }
        }
#endif

        MoveInput = Vector2.ClampMagnitude(input, 1f);
    }

    private void SendMovementInput()
    {
        switch (activeTarget)
        {
            case InputTarget.Player:

                if (playerController != null &&
                    playerController.isActiveAndEnabled)
                {
                    playerController.HandleMovement(MoveInput);
                }

                if (shadowController != null)
                {
                    shadowController.ClearMovementInput();
                }

                break;

            case InputTarget.Shadow:

                if (shadowController != null &&
                    shadowController.isActiveAndEnabled)
                {
                    shadowController.HandleMovement(MoveInput);
                }

                if (playerController != null)
                {
                    playerController.ClearMovementInput();
                }

                break;
        }
    }

    private void ClearAllMovementInput()
    {
        MoveInput = Vector2.zero;

        if (playerController != null)
            playerController.ClearMovementInput();

        if (shadowController != null)
            shadowController.ClearMovementInput();
    }

    // ----------------------------------
    // KEYBOARD ACTIONS
    // ----------------------------------

    private void HandleKeyboardActions()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Keyboard.current == null)
            return;

        var kb = Keyboard.current;

        bool actionPressed =
            kb.qKey.wasPressedThisFrame ||
            kb.eKey.wasPressedThisFrame;
            //kb.spaceKey.wasPressedThisFrame;

        if (actionPressed)
        {
            TriggerContextualAction();
        }
#endif
    }

    // ----------------------------------
    // CONTEXTUAL ACTIONS
    // ----------------------------------

    public void TriggerContextualAction()
    {
        var actionButton =
            FindAnyObjectByType<ContextualActionButton>();

        if (actionButton != null)
        {
            actionButton.TriggerCurrentAction();
            return;
        }

        if (stateMachine == null)
            return;

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

    public void TriggerAction(InputAction action)
    {
        // Prevent duplicate actions in the same frame
        if (Application.isPlaying &&
            Time.frameCount == lastActionFrame &&
            (action == lastAction ||
             (lastAction == InputAction.Detach &&
              action == InputAction.Recall)))
        {
            return;
        }

        if (stateMachine == null)
        {
            Debug.LogWarning(
                "InputSystem: StateMachine reference is missing.");

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

    // ----------------------------------
    // STATE MACHINE
    // ----------------------------------

    public void SetStateMachine(PlayerShadowStateMachine sm)
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }

        stateMachine = sm;

        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
            stateMachine.OnStateChanged += HandleStateChanged;

            HandleStateChanged(stateMachine.CurrentState);
        }
    }

    public void SetPlayerController(ThirdPersonController controller)
    {
        playerController = controller;
    }

    public void SetShadowController(ThirdPersonController controller)
    {
        shadowController = controller;
    }

    public void SetActiveTarget(InputTarget target)
    {
        activeTarget = target;
        ClearAllMovementInput();
    }

    private void HandleStateChanged(PlayerShadowState newState)
    {
        switch (newState)
        {
            case PlayerShadowState.Attached:

                activeTarget = InputTarget.Player;
                break;

            case PlayerShadowState.Detached:

                activeTarget = InputTarget.Shadow;
                break;

            case PlayerShadowState.Reuniting:

                activeTarget = InputTarget.Shadow;
                break;

            case PlayerShadowState.Trapped:

                activeTarget = InputTarget.Shadow;
                break;
        }

        ClearAllMovementInput();

        Debug.Log(
            "Input target changed to: " + activeTarget);
    }
}
