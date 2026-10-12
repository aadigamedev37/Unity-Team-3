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

    private IShadowController shadowController;

    public Vector2 MoveInput { get; private set; }
    public InputTarget ActiveTarget => activeTarget;

    public event Action<InputAction> OnAction;

    private void Awake()
    {
        MoveInput = Vector2.zero;
        ResolveShadowController();
    }

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

        if (stateMachine != null && isActiveAndEnabled)
        {
            stateMachine.OnStateChanged += HandleStateChanged;
        }
    }

    private void Update()
    {
        if (virtualJoystick != null)
        {
            MoveInput = virtualJoystick.Direction;
        }
        else
        {
            MoveInput = Vector2.zero;
        }

        if (activeTarget == InputTarget.Player && playerController != null)
        {
            playerController.HandleMovement(MoveInput);
        }
        else if (activeTarget == InputTarget.Shadow && shadowController != null)
        {
            shadowController.HandleMovement(MoveInput);
        }
    }

    public void TriggerAction(InputAction action)
    {
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