using UnityEngine;
using TMPro;

/// <summary>
/// Implements the exact tutorial sequence for Level 03 (Ghost Gate):
/// 1. Informs player that a heavy door blocks the path.
/// 2. Prompts player to detach shadow.
/// 3. Prompts shadow to approach door and tap STEAL.
/// 4. Prompts shadow to RECALL.
/// 5. Informs player they can now walk through the intangible Ghost Door to the exit!
/// </summary>
public class L03_GhostGateTutorial : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private PlayerController player;
    [SerializeField] private StealableDoor stealableDoor;
    [SerializeField] private Transform exitPad;

    [Header("UI References")]
    [SerializeField] private GameObject tutorialCard;
    [SerializeField] private TextMeshProUGUI tutorialTitle;
    [SerializeField] private TextMeshProUGUI tutorialInstruction;

    private enum Step { ApproachDoor, DetachPrompt, ShadowApproach, StealPrompt, RecallPrompt, PassThroughDoor, NearExit, Completed }
    private Step currentStep = Step.ApproachDoor;

    private void Awake()
    {
        if (stateMachine == null) stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();
        if (player == null) player = FindAnyObjectByType<PlayerController>();
        if (stealableDoor == null) stealableDoor = FindAnyObjectByType<StealableDoor>();

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
        SetStep(Step.ApproachDoor);
    }

    private void Update()
    {
        if (currentStep == Step.Completed) return;

        if (currentStep == Step.ApproachDoor && player != null && stealableDoor != null)
        {
            float dist = Vector3.Distance(player.transform.position, stealableDoor.transform.position);
            if (dist < 4.0f)
            {
                SetStep(Step.DetachPrompt);
            }
        }
        else if ((currentStep == Step.ShadowApproach || currentStep == Step.DetachPrompt) && stealableDoor != null && stealableDoor.IsGhost)
        {
            SetStep(Step.RecallPrompt);
        }
        else if (currentStep == Step.PassThroughDoor && player != null && exitPad != null)
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
                if (currentStep == Step.ApproachDoor || currentStep == Step.DetachPrompt)
                {
                    SetStep(Step.ShadowApproach);
                }
                break;

            case PlayerShadowState.Attached:
                if (stealableDoor != null && stealableDoor.IsGhost)
                {
                    SetStep(Step.PassThroughDoor);
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
            case Step.ApproachDoor:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "OBSTACLE DETECTED";
                if (tutorialInstruction != null) tutorialInstruction.text = "A heavy security gate blocks the corridor ahead";
                break;

            case Step.DetachPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "INFILTRATION";
                if (tutorialInstruction != null) tutorialInstruction.text = "Tap DETACH to release your Shadow";
                break;

            case Step.ShadowApproach:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "STEAL MECHANIC";
                if (tutorialInstruction != null) tutorialInstruction.text = "Move your Shadow to the Door, then tap STEAL";
                break;

            case Step.StealPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "STEAL AVAILABLE";
                if (tutorialInstruction != null) tutorialInstruction.text = "Tap STEAL to phase the door into a Ghost state";
                break;

            case Step.RecallPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "GHOST GATE ACTIVE";
                if (tutorialInstruction != null) tutorialInstruction.text = "Door is intangible! Tap RECALL to return Shadow";
                break;

            case Step.PassThroughDoor:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "CLEAR PATH";
                if (tutorialInstruction != null) tutorialInstruction.text = "Walk through the Ghost Gate and reach the Exit Pad";
                break;

            case Step.NearExit:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "EXIT REACHED";
                if (tutorialInstruction != null) tutorialInstruction.text = "Step onto the exit pad to complete Level 3";
                break;

            case Step.Completed:
                tutorialCard.SetActive(false);
                break;
        }
    }
}
