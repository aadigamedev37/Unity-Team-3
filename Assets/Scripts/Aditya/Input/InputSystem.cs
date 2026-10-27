using UnityEngine;

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
    [SerializeField] private InputTarget activeTarget = InputTarget.Player;
<<<<<<< Updated upstream
    [SerializeField] private VirtualJoystick virtualJoystick;
=======
    //[SerializeField] private VirtualJoystick virtualJoystick;
>>>>>>> Stashed changes
    [SerializeField] private PlayerShadowStateMachine stateMachine;

    public Vector2 MoveInput { get; private set; }

    public InputTarget ActiveTarget => activeTarget;

    public event System.Action<InputAction> OnAction;

    private void Awake()
    {
        MoveInput = Vector2.zero;
    }

    private void OnEnable()
    {
        OnAction += HandleAction;
    }

    private void OnDisable()
    {
        OnAction -= HandleAction;
    }

<<<<<<< Updated upstream
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
        Debug.Log("MoveInput: " + MoveInput);
    }
=======
    //private void Update()
    //{
    //    if (virtualJoystick != null)
    //    {
    //        MoveInput = virtualJoystick.Direction;
    //    }
    //    else
    //    {
    //        MoveInput = Vector2.zero;
    //    }

    //}
>>>>>>> Stashed changes

    public void TriggerAction(InputAction action)
    {
        OnAction?.Invoke(action);
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

    private void HandleAction(InputAction action)
    {
        if (stateMachine == null)
            return;

        switch (action)
        {
            case InputAction.Detach:
                stateMachine.Detach();
                break;

            case InputAction.Recall:
                stateMachine.Recall();
                break;
        }
    }
}