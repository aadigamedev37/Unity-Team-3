using UnityEngine;
using TMPro;

/// <summary>
/// Implements the exact tutorial sequence for Level 03 (Ghost Gate):
/// 1. Enforces level restrictions: hides Jump button, enables Shadow Detach/Recall and Steal.
/// 2. Guides the player to approach the solid blast door blocking the hallway.
/// 3. Prompts the player to detach shadow, approach door, and execute STEAL.
/// 4. Explains ghost phasing, prompts shadow recall, and directs player to walk through to the exit portal.
/// </summary>
public class L03_GhostGateTutorial : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private PlayerController player;
    [SerializeField] private StealableDoor stealableDoor;
    [SerializeField] private Transform exitPad;
    [SerializeField] private ExitTrigger exitTrigger;

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

        if (exitTrigger == null) exitTrigger = FindAnyObjectByType<ExitTrigger>();
        if (exitPad == null && exitTrigger != null) exitPad = exitTrigger.transform;

        if (tutorialCard == null)
        {
            var canvas = GameObject.Find("GameplayHUD_Mobile") ?? GameObject.Find("Canvas");
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
        ApplyLevelRestrictions();
        SetStep(Step.ApproachDoor);
    }

    private void ApplyLevelRestrictions()
    {
        var canvas = GameObject.Find("GameplayHUD_Mobile") ?? GameObject.Find("Canvas");
        if (canvas != null)
        {
            // Hide Jump button for Level 3
            var jumpBtn = canvas.transform.Find("JumpButton");
            if (jumpBtn != null) jumpBtn.gameObject.SetActive(false);

            // Ensure Contextual Action Button is enabled
            var actionBtn = canvas.transform.Find("ContextualActionButton");
            if (actionBtn != null) actionBtn.gameObject.SetActive(true);

            // Hide duplicate LevelHeader to avoid overlapping title bars
            var levelHeader = canvas.transform.Find("LevelHeader");
            if (levelHeader != null) levelHeader.gameObject.SetActive(false);

            // Position TutorialCard cleanly at the top
            var card = canvas.transform.Find("TutorialCard") as RectTransform;
            if (card != null)
            {
                card.anchoredPosition = new Vector2(0f, -40f);
                card.sizeDelta = new Vector2(500f, 75f);
            }
        }
    }

    private void Update()
    {
        if (currentStep == Step.Completed) return;

        if (currentStep == Step.ApproachDoor && player != null && stealableDoor != null)
        {
            float dist = Vector3.Distance(player.transform.position, stealableDoor.transform.position);
            if (dist < 5.0f)
            {
                SetStep(Step.DetachPrompt);
            }
        }
        else if (currentStep == Step.ShadowApproach && stealableDoor != null)
        {
            if (stealableDoor.IsGhost)
            {
                SetStep(Step.RecallPrompt);
            }
            else
            {
                var shadow = GameObject.Find("Shadow");
                if (shadow != null)
                {
                    float dist = Vector3.Distance(shadow.transform.position, stealableDoor.transform.position);
                    if (dist < 3.5f)
                    {
                        SetStep(Step.StealPrompt);
                    }
                }
            }
        }
        else if (currentStep == Step.StealPrompt && stealableDoor != null && stealableDoor.IsGhost)
        {
            SetStep(Step.RecallPrompt);
        }
        else if (currentStep == Step.PassThroughDoor && player != null && exitPad != null)
        {
            float dist = Vector3.Distance(player.transform.position, exitPad.position);
            if (dist < 4.5f)
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

    public void HandleLevelComplete(int levelId)
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
                if (tutorialTitle != null) tutorialTitle.text = "SECTOR 03: GHOST GATE";
                if (tutorialInstruction != null) tutorialInstruction.text = "A heavy blast door blocks the corridor ahead";
                break;

            case Step.DetachPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "INFILTRATION REQUIRED";
                if (tutorialInstruction != null) tutorialInstruction.text = "Tap DETACH or press 'E' to send your Shadow forward";
                break;

            case Step.ShadowApproach:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "SHADOW INFILTRATION";
                if (tutorialInstruction != null) tutorialInstruction.text = "Guide your Shadow close to the blast door";
                break;

            case Step.StealPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "STEAL AVAILABLE";
                if (tutorialInstruction != null) tutorialInstruction.text = "Tap STEAL or press 'E' to phase the blast door into a Ghost state";
                break;

            case Step.RecallPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "GHOST GATE ACTIVE";
                if (tutorialInstruction != null) tutorialInstruction.text = "Door is intangible! Tap RECALL or press 'E' to return Shadow";
                break;

            case Step.PassThroughDoor:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "CLEAR PATH";
                if (tutorialInstruction != null) tutorialInstruction.text = "Walk through the phased Ghost Door and reach the Exit Portal";
                break;

            case Step.NearExit:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "EXIT REACHED";
                if (tutorialInstruction != null) tutorialInstruction.text = "Step into the portal to complete Sector 03";
                break;

            case Step.Completed:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "MISSION COMPLETE";
                if (tutorialInstruction != null) tutorialInstruction.text = "All sectors cleared! Returning to mission select...";
                break;
        }
    }
}
