using UnityEngine;
using TMPro;

/// <summary>
/// Implements the exact tutorial sequence for Level 02 (The Sandbox):
/// 1. Enforces level restrictions: hides Jump button, enables Shadow Detach/Recall.
/// 2. GATES the exit: locks ExitTrigger until the player detaches, moves shadow, and recalls.
/// 3. Prevents premature exit completion and directs player with movie-like cinematic directives.
/// 4. Unlocks exit and illuminates portal green/cyan upon tutorial mastery.
/// </summary>
public class L02_SandboxTutorial : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private PlayerController player;
    [SerializeField] private Transform exitPad;
    [SerializeField] private ExitTrigger exitTrigger;
    [SerializeField] private Light exitLight;

    [Header("UI References")]
    [SerializeField] private GameObject tutorialCard;
    [SerializeField] private TextMeshProUGUI tutorialTitle;
    [SerializeField] private TextMeshProUGUI tutorialInstruction;

    private enum Step { DetachPrompt, ShadowActive, RecallPrompt, ProceedToExit, NearExit, Completed }
    private Step currentStep = Step.DetachPrompt;

    private float shadowMoveTimer = 0f;
    private bool tutorialCompleted = false;

    private void Awake()
    {
        if (stateMachine == null) stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();
        if (player == null) player = FindAnyObjectByType<PlayerController>();

        if (exitTrigger == null) exitTrigger = FindAnyObjectByType<ExitTrigger>();
        if (exitPad == null && exitTrigger != null) exitPad = exitTrigger.transform;

        if (exitLight == null)
        {
            var el = GameObject.Find("ExitLight");
            if (el != null) exitLight = el.GetComponent<Light>();
        }

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

        if (exitTrigger != null)
        {
            exitTrigger.OnLockedExitAttempted += HandleLockedExitAttempt;
        }
    }

    private void OnDisable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }
        GameEvents.OnLevelComplete -= HandleLevelComplete;

        if (exitTrigger != null)
        {
            exitTrigger.OnLockedExitAttempted -= HandleLockedExitAttempt;
        }
    }

    private void Start()
    {
        ApplyLevelRestrictions();

        // Lock exit until tutorial requirements are fulfilled
        if (exitTrigger != null)
        {
            exitTrigger.IsLocked = true;
        }

        if (exitLight != null)
        {
            exitLight.color = new Color(1f, 0.2f, 0.2f); // Red = Locked
        }

        SetStep(Step.DetachPrompt);
    }

    private void ApplyLevelRestrictions()
    {
        var canvas = GameObject.Find("GameplayHUD_Mobile") ?? GameObject.Find("Canvas");
        if (canvas != null)
        {
            // Hide Jump button for Level 2
            var jumpBtn = canvas.transform.Find("JumpButton");
            if (jumpBtn != null) jumpBtn.gameObject.SetActive(false);

            // Ensure Contextual Action Button (Shadow Walk / Detach) is enabled
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

        if (currentStep == Step.ShadowActive)
        {
            shadowMoveTimer += Time.deltaTime;
            if (shadowMoveTimer > 2.5f)
            {
                SetStep(Step.RecallPrompt);
            }
        }
        else if (currentStep == Step.ProceedToExit && player != null && exitPad != null)
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
                if (currentStep == Step.DetachPrompt)
                {
                    shadowMoveTimer = 0f;
                    SetStep(Step.ShadowActive);
                }
                break;

            case PlayerShadowState.Attached:
                if (currentStep == Step.ShadowActive || currentStep == Step.RecallPrompt)
                {
                    tutorialCompleted = true;
                    // Unlock the exit
                    if (exitTrigger != null)
                    {
                        exitTrigger.IsLocked = false;
                    }
                    if (exitLight != null)
                    {
                        exitLight.color = new Color(0.2f, 0.9f, 1f); // Vibrant Cyan = Unlocked
                    }
                    SetStep(Step.ProceedToExit);
                }
                break;
        }
    }

    private void HandleLockedExitAttempt()
    {
        if (tutorialCard == null) return;
        tutorialCard.SetActive(true);
        if (tutorialTitle != null) tutorialTitle.text = "EXIT LOCKED";
        if (tutorialInstruction != null) tutorialInstruction.text = "You must detach and recall your shadow to unlock the exit.";
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
            case Step.DetachPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "SECTOR 02: THE SANDBOX";
                if (tutorialInstruction != null) tutorialInstruction.text = "Tap DETACH or press 'E' to separate your Shadow";
                break;

            case Step.ShadowActive:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "SHADOW DETACHED";
                if (tutorialInstruction != null) tutorialInstruction.text = "Guide your Shadow independently around the room";
                break;

            case Step.RecallPrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "RECALL MECHANIC";
                if (tutorialInstruction != null) tutorialInstruction.text = "Tap RECALL or press 'E' to snap your Shadow back to you";
                break;

            case Step.ProceedToExit:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "TRAINING COMPLETE";
                if (tutorialInstruction != null) tutorialInstruction.text = "Exit unlocked! Proceed to the portal to advance";
                break;

            case Step.NearExit:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "EXIT REACHED";
                if (tutorialInstruction != null) tutorialInstruction.text = "Step into the portal to proceed to Sector 03";
                break;

            case Step.Completed:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "SECTOR 02 COMPLETE";
                if (tutorialInstruction != null) tutorialInstruction.text = "Advancing to Sector 03...";
                break;
        }
    }
}
