using UnityEngine;
using TMPro;

/// <summary>
/// Implements the exact tutorial sequence for Level 02 (The Sandbox):
/// 1. Prompts player to tap DETACH to release the shadow.
/// 2. Explains independent shadow movement.
/// 3. Prompts player to tap RECALL to return the shadow.
/// 4. Directs player to the exit pad once the shadow is reattached.
/// </summary>
public class L02_SandboxTutorial : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform exitPad;

    [Header("UI References")]
    [SerializeField] private GameObject tutorialCard;
    [SerializeField] private TextMeshProUGUI tutorialTitle;
    [SerializeField] private TextMeshProUGUI tutorialInstruction;

    private enum Step { DetachPrompt, ShadowActive, RecallPrompt, ProceedToExit, NearExit, Completed }
    private Step currentStep = Step.DetachPrompt;

    private float shadowMoveTimer = 0f;

    private void Awake()
    {
        if (stateMachine == null) stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();
        if (player == null) player = FindAnyObjectByType<PlayerController>();

        if (exitPad == null)
        {
            var pad = GameObject.Find("ExitPad") ?? GameObject.Find("ExitZone");
            if (pad != null) exitPad = pad.transform;
        }

        if (tutorialCard == null)
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                var card = canvas.transform.Find("TutorialCard");
                if (card != null)
                {
                    tutorialCard = card.gameObject;
                    tutorialTitle = card.Find("Title")?.GetComponent<TextMeshProUGUI>();
                    tutorialInstruction = card.Find("Instruction")?.GetComponent<TextMeshProUGUI>();
                }
            }
        }
    }

    private void OnEnable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged += HandleStateChanged;
        }
        GameEvents.OnLevelComplete += HandleLevelComplete;
    }

    private void OnDisable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }
        GameEvents.OnLevelComplete -= HandleLevelComplete;
    }

    private void Start()
    {
        SetStep(Step.DetachPrompt);
    }

    private void Update()
    {
        if (currentStep == Step.Completed) return;

        if (currentStep == Step.ShadowActive)
        {
            shadowMoveTimer += Time.deltaTime;
            if (shadowMoveTimer > 2.0f)
            {
                SetStep(Step.RecallPrompt);
            }
        }
        else if (currentStep == Step.ProceedToExit && player != null && exitPad != null)
        {
            float dist = Vector3.Distance(player.transform.position, exitPad.position);
            if (dist < 3.5f)
            {
                SetStep(Step.NearExit);
            }
        }
    }

    private void HandleStateChanged(PlayerShadowState newState)
    {
        switch (newState)
        {
            case PlayerShadowState.Detached:
                if (currentStep == Step.DetachPrompt)
                {
                    shadowMoveTimer = 0f;
                    SetStep(Step.ShadowActive);
                }
                break;

            case PlayerShadowState.Attached:
                if (currentStep == Step.ShadowActive || currentStep == Step.RecallPrompt)
                {
                    SetStep(Step.ProceedToExit);
                }
                break;
        }
    }

    private void HandleLevelComplete(int levelId)
    {
        SetStep(Step.Completed);
    }

    private void SetStep(Step step)
    {
        currentStep = step;

        if (tutorialCard == null) return;

        switch (step)
        {
            case Step.DetachPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "SHADOW MECHANIC";
                if (tutorialInstruction != null) tutorialInstruction.text = "Tap DETACH to release your Shadow";
                break;

            case Step.ShadowActive:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "SHADOW DETACHED";
                if (tutorialInstruction != null) tutorialInstruction.text = "Move your Shadow independently around the room";
                break;

            case Step.RecallPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "RECALL MECHANIC";
                if (tutorialInstruction != null) tutorialInstruction.text = "Tap RECALL to snap your Shadow back to you";
                break;

            case Step.ProceedToExit:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "OBJECTIVE";
                if (tutorialInstruction != null) tutorialInstruction.text = "Proceed to the illuminated Exit Pad to complete Level 2";
                break;

            case Step.NearExit:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "EXIT REACHED";
                if (tutorialInstruction != null) tutorialInstruction.text = "Step onto the exit pad to complete Level 2";
                break;

            case Step.Completed:
                tutorialCard.SetActive(false);
                break;
        }
    }
}
