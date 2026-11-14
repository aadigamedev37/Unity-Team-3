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
    private Vector2? inputOverride = null;
    private StarterAssets.StarterAssetsInputs starterPlayerInputs;
    private StarterAssets.StarterAssetsInputs starterShadowInputs;

    public void SetInputOverride(Vector2? overrideInput)
    {
        inputOverride = overrideInput;
    }

    public Vector2 MoveInput { get; private set; }
    public InputTarget ActiveTarget => activeTarget;

    public event Action<InputAction> OnAction;

    private int lastActionFrame = -1;
    private InputAction lastAction;

    private void Awake()
    {
        MoveInput = Vector2.zero;
        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
        }
        if (stateMachine == null)
        {
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>(FindObjectsInactive.Include);
        }
        ResolveShadowController();
    }

    private void Start()
    {
        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
        }
        if (stateMachine == null)
        {
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>(FindObjectsInactive.Include);
            if (stateMachine != null)
            {
                stateMachine.OnStateChanged -= HandleStateChanged;
                stateMachine.OnStateChanged += HandleStateChanged;
            }
        }
        ResolveShadowController();
#if UNITY_EDITOR
        ConfigureCursorForEditorTesting();
#endif
        // Enforce initial Attached state: Player active, Shadow deactivated and hidden
        HandleStateChanged(stateMachine != null ? stateMachine.CurrentState : PlayerShadowState.Attached);
    }

    private void OnEnable()
    {
        if (stateMachine == null)
        {
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>(FindObjectsInactive.Include);
        }
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
        if (unlockCursorForMobileUI && virtualJoystick != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            var starterInputs = FindObjectsByType<StarterAssets.StarterAssetsInputs>(FindObjectsSortMode.None);
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

    private void ResolveMovementReferences()
    {
        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
        }

        if (starterPlayerInputs == null)
        {
            var pObj = GameObject.Find("PlayerArmature") ?? GameObject.Find("Player") ?? GameObject.FindWithTag("Player");
            if (pObj != null)
            {
                starterPlayerInputs = pObj.GetComponent<StarterAssets.StarterAssetsInputs>() ?? pObj.GetComponentInChildren<StarterAssets.StarterAssetsInputs>(true);
            }
        }

        if (starterShadowInputs == null)
        {
            var sObj = GameObject.Find("Shadow") ?? GameObject.Find("AuthoritativeShadow");
            if (sObj != null)
            {
                starterShadowInputs = sObj.GetComponent<StarterAssets.StarterAssetsInputs>() ?? sObj.GetComponentInChildren<StarterAssets.StarterAssetsInputs>(true);
            }
        }
    }

    private void ResolveShadowController()
    {
        ResolveMovementReferences();
        if (shadowControllerTarget != null)
        {
            shadowController = shadowControllerTarget as IShadowController;
            if (shadowController == null)
            {
                shadowController = shadowControllerTarget.GetComponent<IShadowController>();
            }
            if (shadowController == null)
            {
                Debug.LogWarning("InputSystem: shadowControllerTarget does not implement IShadowController.", this);
            }
        }
        else if (shadowController == null)
        {
            var sma = FindAnyObjectByType<ShadowMovementAdapter>(FindObjectsInactive.Include);
            if (sma != null)
            {
                shadowController = sma;
                shadowControllerTarget = sma;
            }
            else
            {
                var shadowObj = GameObject.Find("Shadow") ?? GameObject.Find("AuthoritativeShadow");
                if (shadowObj != null)
                {
                    var isc = shadowObj.GetComponent<IShadowController>() ?? shadowObj.GetComponentInChildren<IShadowController>(true);
                    if (isc != null)
                    {
                        shadowController = isc;
                        shadowControllerTarget = isc as MonoBehaviour;
                    }
                }
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
            HandleStateChanged(stateMachine.CurrentState);
        }
    }

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
        if (inputOverride.HasValue)
        {
            input = inputOverride.Value;
        }
        else if (virtualJoystick != null && virtualJoystick.Direction != Vector2.zero)
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
            input = new Vector2(h, v);
        }
#endif
        MoveInput = Vector2.ClampMagnitude(input, 1f);
    }

    private void HandleKeyboardActions()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // Action hotkey: E or Q handles Detach, Steal, Recall
        bool actionPressed = false;
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            actionPressed = kb.eKey.wasPressedThisFrame || kb.qKey.wasPressedThisFrame;
        }
        if (!actionPressed)
        {
            try
            {
                actionPressed = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q);
            }
            catch { }
        }

        if (actionPressed)
        {
            TriggerContextualAction();
        }

        // Jump hotkey: Space Bar (Triggers Jump for active character)
        bool jumpPressed = false;
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            jumpPressed = kb.spaceKey.wasPressedThisFrame;
        }
        if (!jumpPressed)
        {
            try
            {
                jumpPressed = Input.GetKeyDown(KeyCode.Space);
            }
            catch { }
        }

        if (jumpPressed)
        {
            TriggerJump();
        }

        // Pause hotkey: Escape or P
        bool pausePressed = false;
        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            var kb = UnityEngine.InputSystem.Keyboard.current;
            pausePressed = kb.escapeKey.wasPressedThisFrame || kb.pKey.wasPressedThisFrame;
        }
        if (!pausePressed)
        {
            try
            {
                pausePressed = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
            }
            catch { }
        }

        if (pausePressed)
        {
            if (Mohit.MetaSystems.PauseMenuManager.Instance != null)
            {
                Mohit.MetaSystems.PauseMenuManager.Instance.TogglePause();
            }
            else if (StealMan.Core.GameManager.Instance != null)
            {
                StealMan.Core.GameManager.Instance.TogglePause();
            }
        }
#endif
    }

    private void SendMovementInput()
    {
        ResolveMovementReferences();

        switch (activeTarget)
        {
            case InputTarget.Player:
                if (playerController != null && playerController.isActiveAndEnabled)
                {
                    playerController.HandleMovement(MoveInput);
                }
                if (starterPlayerInputs != null)
                {
                    starterPlayerInputs.MoveInput(MoveInput);
                }
                if (shadowController != null)
                {
                    shadowController.HandleMovement(Vector2.zero);
                }
                if (starterShadowInputs != null)
                {
                    starterShadowInputs.MoveInput(Vector2.zero);
                }
                break;

            case InputTarget.Shadow:
                if (shadowController != null)
                {
                    shadowController.HandleMovement(MoveInput);
                }
                if (starterShadowInputs != null)
                {
                    starterShadowInputs.MoveInput(MoveInput);
                }
                if (playerController != null)
                {
                    playerController.HandleMovement(Vector2.zero);
                }
                if (starterPlayerInputs != null)
                {
                    starterPlayerInputs.MoveInput(Vector2.zero);
                }
                break;
        }
    }

    private void ClearAllMovementInput()
    {
        MoveInput = Vector2.zero;
        if (playerController != null && playerController.isActiveAndEnabled)
            playerController.HandleMovement(Vector2.zero);
        if (starterPlayerInputs != null)
            starterPlayerInputs.MoveInput(Vector2.zero);
        if (shadowController != null)
            shadowController.HandleMovement(Vector2.zero);
        if (starterShadowInputs != null)
            starterShadowInputs.MoveInput(Vector2.zero);
    }

    public void TriggerJump()
    {
        if (activeTarget == InputTarget.Player && playerController != null)
        {
            playerController.Jump();
        }
        else if (activeTarget == InputTarget.Shadow && shadowController != null)
        {
            shadowController.Jump();
        }
    }

    public void TriggerContextualAction()
    {
        if (stateMachine == null)
        {
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>(FindObjectsInactive.Include);
        }

        var actionButton = FindAnyObjectByType<ContextualActionButton>(FindObjectsInactive.Include);
        if (actionButton != null && actionButton.gameObject.activeInHierarchy)
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
                    if (actionButton != null && actionButton.IsStealTargetInRange)
                    {
                        TriggerSteal();
                    }
                    else
                    {
                        TriggerRecall();
                    }
                    break;
                case PlayerShadowState.Trapped:
                    TriggerRecover();
                    break;
            }
        }
        else
        {
            var sm = FindAnyObjectByType<ShadowManager>(FindObjectsInactive.Include);
            if (sm != null)
            {
                if (sm.shadowMode) sm.ExitShadow();
                else sm.SpawnShadow();
            }
        }
    }

    public void TriggerAction(InputAction action)
    {
        // Prevent duplicate actions in the same frame
        if (Application.isPlaying && Time.frameCount == lastActionFrame && (action == lastAction || (lastAction == InputAction.Detach && action == InputAction.Recall)))
        {
            return;
        }

        if (stateMachine == null)
        {
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>(FindObjectsInactive.Include);
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
        var sm = FindAnyObjectByType<ShadowManager>(FindObjectsInactive.Include);
        if (sm != null && !sm.shadowMode)
        {
            sm.SpawnShadow();
        }
    }

    public void TriggerRecall()
    {
        TriggerAction(InputAction.Recall);
        var sm = FindAnyObjectByType<ShadowManager>(FindObjectsInactive.Include);
        if (sm != null && sm.shadowMode)
        {
            sm.ExitShadow();
        }
    }

    public void TriggerSteal()
    {
        var interactor = FindAnyObjectByType<StealInteractor>(FindObjectsInactive.Include);
        if (interactor != null && interactor.HasTarget)
        {
            interactor.ExecuteSteal();
        }
        TriggerAction(InputAction.Steal);
    }

    public void TriggerRecover()
    {
        TriggerAction(InputAction.Recover);
    }

    public void SetActiveTarget(InputTarget target)
    {
        activeTarget = target;
        ClearAllMovementInput();
        Debug.Log("Input target changed to: " + activeTarget);
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
                if (playerController == null)
                {
                    playerController = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
                }
                if (playerController != null)
                {
                    playerController.Freeze();
                }
                if (shadowController == null)
                {
                    ResolveShadowController();
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

        ClearAllMovementInput();
        Debug.Log("Input target changed to: " + activeTarget);
    }
}
