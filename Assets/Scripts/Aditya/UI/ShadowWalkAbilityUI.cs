using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dedicated circular on-screen HUD button for the SHADOW WALK ability.
/// 
/// Displays:
/// - Circular dark charcoal background with spectral cyan border.
/// - Outer radial countdown fill ring (draining clockwise as ability timer counts down).
/// - Remaining duration text (e.g. 14.8s) or action label.
/// - Low-time flashing warning state when remaining time < 3.0s.
/// - Contextual STEAL state when near a stealable object.
/// - Smooth touch and mouse input response.
/// </summary>
public class ShadowWalkAbilityUI : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private ShadowWalkAbility ability;
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private ContextualActionButton contextualAction;

    [Header("Button Elements")]
    [SerializeField] private Button button;
    [SerializeField] private Image buttonBackground;
    [SerializeField] private Image countdownRing;
    [SerializeField] private Image glowBorder;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI actionLabel;

    [Header("Visual Colors")]
    [SerializeField] private Color normalRingColor = new Color(0.12f, 0.85f, 0.95f, 1f); // Vibrant Cyan
    [SerializeField] private Color warningRingColor = new Color(1.0f, 0.45f, 0.15f, 1f); // Warning Amber
    [SerializeField] private Color lowTimeColor = new Color(1.0f, 0.20f, 0.20f, 1f); // Danger Red
    [SerializeField] private Color stealGlowColor = new Color(0.25f, 0.75f, 1.0f, 1f); // Steal Blue

    [Header("Warning Settings")]
    [SerializeField] private float lowTimeThreshold = 3.0f;
    [SerializeField] private float warningPulseSpeed = 8.0f;

    private Vector3 initialScale = Vector3.one;

    private void Awake()
    {
        initialScale = transform.localScale;

        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(HandleButtonClicked);
        }

        if (ability == null)
            ability = FindAnyObjectByType<ShadowWalkAbility>();

        if (stateMachine == null)
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();

        if (contextualAction == null)
            contextualAction = GetComponent<ContextualActionButton>() ?? FindAnyObjectByType<ContextualActionButton>();
    }

    private void OnEnable()
    {
        if (ability != null)
        {
            ability.OnTimerUpdated += HandleTimerUpdated;
            ability.OnAbilityStateChanged += HandleAbilityStateChanged;
        }

        if (stateMachine != null)
        {
            stateMachine.OnStateChanged += HandleStateMachineStateChanged;
        }

        UpdateVisualState();
    }

    private void OnDisable()
    {
        if (ability != null)
        {
            ability.OnTimerUpdated -= HandleTimerUpdated;
            ability.OnAbilityStateChanged -= HandleAbilityStateChanged;
        }

        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateMachineStateChanged;
        }
    }

    private void Update()
    {
        if (ability == null || ability.CurrentState != ShadowWalkAbility.AbilityState.Active)
            return;

        float remaining = ability.RemainingTime;

        // Low time flashing pulse
        if (remaining <= lowTimeThreshold && remaining > 0f)
        {
            float pulse = (Mathf.Sin(Time.time * warningPulseSpeed) + 1f) * 0.5f;
            Color pulseCol = Color.Lerp(warningRingColor, lowTimeColor, pulse);

            if (countdownRing != null)
                countdownRing.color = pulseCol;

            if (glowBorder != null)
                glowBorder.color = pulseCol;
        }
    }

    private void HandleTimerUpdated(float remaining, float max)
    {
        if (countdownRing != null)
        {
            float fill = max > 0.001f ? Mathf.Clamp01(remaining / max) : 0f;
            countdownRing.fillAmount = fill;

            if (remaining > lowTimeThreshold)
            {
                countdownRing.color = normalRingColor;
                if (glowBorder != null) glowBorder.color = normalRingColor;
            }
        }

        if (timerText != null)
        {
            if (contextualAction != null && contextualAction.IsStealTargetInRange)
            {
                timerText.text = "STEAL";
            }
            else
            {
                timerText.text = $"{remaining:F1}s";
            }
        }
    }

    private void HandleAbilityStateChanged(ShadowWalkAbility.AbilityState newState)
    {
        UpdateVisualState();
    }

    private void HandleStateMachineStateChanged(PlayerShadowState newState)
    {
        UpdateVisualState();
    }

    public void UpdateVisualState()
    {
        PlayerShadowState pState = stateMachine != null ? stateMachine.CurrentState : PlayerShadowState.Attached;
        bool isStealInRange = contextualAction != null && contextualAction.IsStealTargetInRange;

        switch (pState)
        {
            case PlayerShadowState.Attached:
                if (countdownRing != null)
                {
                    countdownRing.fillAmount = 1.0f;
                    countdownRing.color = normalRingColor;
                }
                if (timerText != null) timerText.text = "RELEASE";
                if (actionLabel != null) actionLabel.text = "SHADOW WALK";
                if (button != null) button.interactable = true;
                break;

            case PlayerShadowState.Detached:
                if (isStealInRange)
                {
                    if (timerText != null) timerText.text = "STEAL";
                    if (actionLabel != null) actionLabel.text = "INTERACT";
                    if (countdownRing != null) countdownRing.color = stealGlowColor;
                }
                else
                {
                    if (actionLabel != null) actionLabel.text = "RECALL";
                }
                if (button != null) button.interactable = true;
                break;

            case PlayerShadowState.Reuniting:
                if (countdownRing != null) countdownRing.fillAmount = 0f;
                if (timerText != null) timerText.text = "...";
                if (actionLabel != null) actionLabel.text = "RETURNING";
                if (button != null) button.interactable = false;
                break;

            case PlayerShadowState.Trapped:
                if (timerText != null) timerText.text = "TAP!";
                if (actionLabel != null) actionLabel.text = "RECOVER";
                if (button != null) button.interactable = true;
                break;
        }
    }

    public void HandleButtonClicked()
    {
        if (stateMachine == null) return;

        PlayerShadowState pState = stateMachine.CurrentState;
        bool isStealInRange = contextualAction != null && contextualAction.IsStealTargetInRange;

        switch (pState)
        {
            case PlayerShadowState.Attached:
                if (ability != null)
                {
                    ability.ActivateShadowWalk();
                }
                else
                {
                    stateMachine.Detach();
                }
                break;

            case PlayerShadowState.Detached:
                if (isStealInRange)
                {
                    stateMachine.Steal();
                }
                else
                {
                    if (ability != null)
                    {
                        ability.TriggerRecall();
                    }
                    else
                    {
                        stateMachine.Recall();
                    }
                }
                break;

            case PlayerShadowState.Trapped:
                stateMachine.Recover();
                break;
        }
    }
}
