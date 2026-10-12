using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ContextualActionButton : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private InputSystem inputSystem;

    [Header("UI Display")]
    [SerializeField] private TextMeshProUGUI buttonTextMesh;
    [SerializeField] private Text buttonLegacyText;

    [Header("Configuration")]
    [SerializeField] private string detachLabel = "DETACH";
    [SerializeField] private string recallLabel = "RECALL";
    [SerializeField] private string stealLabel = "STEAL";
    [SerializeField] private string recoverLabel = "RECOVER";
    [SerializeField] private string reunitingLabel = "REUNITING";

    private Button button;
    private bool isStealTargetInRange = false;

    public InputAction CurrentAction { get; private set; } = InputAction.Detach;
    public bool IsStealTargetInRange => isStealTargetInRange;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleButtonClick);
    }

    private void Start()
    {
        UpdateContextualState();
    }

    private void OnEnable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged += HandleStateChanged;
        }

        UpdateContextualState();
    }

    private void OnDisable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }
    }

    public void SetStealTargetInRange(bool inRange)
    {
        isStealTargetInRange = inRange;
        UpdateContextualState();
    }

    private void HandleStateChanged(PlayerShadowState newState)
    {
        UpdateContextualState();
    }

    public void UpdateContextualState()
    {
        if (stateMachine == null)
            return;

        PlayerShadowState state = stateMachine.CurrentState;
        bool interactable = true;
        string labelText = detachLabel;

        switch (state)
        {
            case PlayerShadowState.Attached:
                CurrentAction = InputAction.Detach;
                labelText = detachLabel;
                interactable = true;
                break;

            case PlayerShadowState.Detached:
                if (isStealTargetInRange)
                {
                    CurrentAction = InputAction.Steal;
                    labelText = stealLabel;
                }
                else
                {
                    CurrentAction = InputAction.Recall;
                    labelText = recallLabel;
                }
                interactable = true;
                break;

            case PlayerShadowState.Trapped:
                CurrentAction = InputAction.Recover;
                labelText = recoverLabel;
                interactable = true;
                break;

            case PlayerShadowState.Reuniting:
                interactable = false;
                labelText = reunitingLabel;
                break;
        }

        if (button != null)
        {
            button.interactable = interactable;
        }

        SetLabelText(labelText);
    }

    private void SetLabelText(string text)
    {
        if (buttonTextMesh != null)
        {
            buttonTextMesh.text = text;
        }
        else if (buttonLegacyText != null)
        {
            buttonLegacyText.text = text;
        }
    }

    private void HandleButtonClick()
    {
        if (inputSystem != null)
        {
            inputSystem.TriggerAction(CurrentAction);
        }
        else if (stateMachine != null)
        {
            switch (CurrentAction)
            {
                case InputAction.Detach:
                    stateMachine.Detach();
                    break;
                case InputAction.Recall:
                    stateMachine.Recall();
                    break;
                case InputAction.Steal:
                    stateMachine.Steal();
                    break;
                case InputAction.Recover:
                    stateMachine.Recover();
                    break;
            }
        }
    }
}
