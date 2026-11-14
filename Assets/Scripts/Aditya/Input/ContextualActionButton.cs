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

    [Header("Icon Configuration")]
    [SerializeField] private Image buttonIcon;
    [SerializeField] private Sprite detachIcon;
    [SerializeField] private Sprite recallIcon;
    [SerializeField] private Sprite stealIcon;
    [SerializeField] private Sprite recoverIcon;
    [SerializeField] private bool showIconsOnly = false;

    [Header("Pulsing Effect")]
    [SerializeField] private bool enablePulsing = false;
    [SerializeField] private float pulseFrequency = 4.5f;
    [SerializeField] private float pulseScale = 0.08f;

    private Vector3 initialScale = Vector3.one;
    public bool EnablePulsing { get => enablePulsing; set => SetPulsing(value); }

    public void SetPulsing(bool pulsing)
    {
        enablePulsing = pulsing;
        if (!pulsing)
        {
            transform.localScale = initialScale;
        }
    }

    private void Update()
    {
        if (enablePulsing && button != null && button.interactable)
        {
            float wave = Mathf.Sin(Time.time * pulseFrequency);
            float scaleMul = 1f + wave * pulseScale;
            transform.localScale = initialScale * scaleMul;
        }
        else if (transform.localScale != initialScale)
        {
            transform.localScale = initialScale;
        }
    }

    private Button button;
    private bool isStealTargetInRange = false;

    public InputAction CurrentAction { get; private set; } = InputAction.Detach;
    public bool IsStealTargetInRange => isStealTargetInRange;

    private void Awake()
    {
        initialScale = transform.localScale;
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
        }

        UpdateContextualState();
    }

    public void UpdateContextualState()
    {
        if (stateMachine == null)
            return;

        if (button == null)
        {
            initialScale = transform.localScale;
        button = GetComponent<Button>();
        }

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

        UpdateVisuals(state, labelText);
    }

    private void UpdateVisuals(PlayerShadowState state, string labelText)
    {
        Sprite icon = null;
        switch (CurrentAction)
        {
            case InputAction.Detach: icon = detachIcon; break;
            case InputAction.Recall: icon = recallIcon; break;
            case InputAction.Steal: icon = stealIcon; break;
            case InputAction.Recover: icon = recoverIcon; break;
        }

        if (buttonIcon != null)
        {
            buttonIcon.sprite = icon;
            buttonIcon.enabled = icon != null;
        }

        if (showIconsOnly && icon != null)
        {
            SetLabelText("");
        }
        else
        {
            SetLabelText(labelText);
        }
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

    private int lastClickFrame = -1;

    public void TriggerCurrentAction()
    {
        HandleButtonClick();
    }

    private void HandleButtonClick()
    {
        if (Time.frameCount == lastClickFrame)
            return;
        lastClickFrame = Time.frameCount;

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
