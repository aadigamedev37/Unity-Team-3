using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Guides new players through fundamental movement in Level 01 (The Corridor).
/// Enforces level restrictions: hides Jump and Shadow action buttons, allowing only joystick movement.
/// Delivers movie-like cinematic directives via the HUD Tutorial Card.
/// </summary>
public class L01_MovementTutorial : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private PlayerController player;
    [SerializeField] private VirtualJoystick virtualJoystick;
    [SerializeField] private Transform exitPortal;

    [Header("UI References")]
    [SerializeField] private GameObject tutorialCard;
    [SerializeField] private TextMeshProUGUI tutorialTitle;
    [SerializeField] private TextMeshProUGUI tutorialInstruction;

    private enum Step { MovePrompt, ProceedToExit, NearExit, Completed }
    private Step currentStep = Step.MovePrompt;

    private Vector3 startPos;
    private bool hasMoved = false;
    private float pulseTimer = 0f;
    private Vector3 originalJoystickScale = Vector3.one;

    private void Awake()
    {
        if (inputSystem == null) inputSystem = FindAnyObjectByType<InputSystem>();
        if (player == null) player = FindAnyObjectByType<PlayerController>();
        if (virtualJoystick == null) virtualJoystick = FindAnyObjectByType<VirtualJoystick>();

        if (exitPortal == null)
        {
            var exitZone = GameObject.Find("ExitZone");
            if (exitZone != null) exitPortal = exitZone.transform;
        }

        if (virtualJoystick != null)
        {
            originalJoystickScale = virtualJoystick.transform.localScale;
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

    private void Start()
    {
        // Restriction enforcement for Level 1: Pure movement only (No jump, no shadow action)
        ApplyLevelRestrictions();

        if (player != null)
        {
            startPos = player.transform.position;
        }

        SetTutorialStep(Step.MovePrompt);
    }

    private void ApplyLevelRestrictions()
    {
        var canvas = GameObject.Find("GameplayHUD_Mobile") ?? GameObject.Find("Canvas");
        if (canvas != null)
        {
            // Hide Jump button for Level 1
            var jumpBtn = canvas.transform.Find("JumpButton");
            if (jumpBtn != null) jumpBtn.gameObject.SetActive(false);

            // Hide Contextual Action Button (Shadow Walk / Detach) for Level 1
            var actionBtn = canvas.transform.Find("ContextualActionButton");
            if (actionBtn != null) actionBtn.gameObject.SetActive(false);

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

        // Animate joystick pulsing during the MovePrompt step
        if (currentStep == Step.MovePrompt && virtualJoystick != null && !hasMoved)
        {
            pulseTimer += Time.deltaTime * 3.5f;
            float scaleMultiplier = 1f + Mathf.Sin(pulseTimer) * 0.12f;
            virtualJoystick.transform.localScale = originalJoystickScale * scaleMultiplier;
        }
        else if (virtualJoystick != null && virtualJoystick.transform.localScale != originalJoystickScale)
        {
            virtualJoystick.transform.localScale = Vector3.Lerp(virtualJoystick.transform.localScale, originalJoystickScale, Time.deltaTime * 10f);
        }

        // Check if player has started moving
        if (!hasMoved)
        {
            bool hasInput = inputSystem != null && inputSystem.MoveInput.sqrMagnitude > 0.05f;
            bool hasPositionChanged = player != null && Vector3.Distance(player.transform.position, startPos) > 0.8f;

            if (hasInput || hasPositionChanged)
            {
                hasMoved = true;
                SetTutorialStep(Step.ProceedToExit);
            }
        }
        else if (currentStep == Step.ProceedToExit && player != null && exitPortal != null)
        {
            float distToExit = Vector3.Distance(player.transform.position, exitPortal.position);
            if (distToExit < 5.0f)
            {
                SetTutorialStep(Step.NearExit);
            }
        }
    }

    public void OnLevelCompleted()
    {
        SetTutorialStep(Step.Completed);
    }

    private void SetTutorialStep(Step step)
    {
        currentStep = step;

        if (tutorialCard == null) return;

        switch (step)
        {
            case Step.MovePrompt:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "SECTOR 01: THE CORRIDOR";
                if (tutorialInstruction != null) tutorialInstruction.text = "Drag the Joystick or use W-A-S-D to Move";
                break;

            case Step.ProceedToExit:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "OBJECTIVE";
                if (tutorialInstruction != null) tutorialInstruction.text = "Follow the corridor to the illuminated Exit Portal";
                break;

            case Step.NearExit:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "EXIT REACHED";
                if (tutorialInstruction != null) tutorialInstruction.text = "Step into the portal to complete Level 1";
                break;

            case Step.Completed:
                tutorialCard.SetActive(true);
                if (tutorialTitle != null) tutorialTitle.text = "SECTOR 01 COMPLETE";
                if (tutorialInstruction != null) tutorialInstruction.text = "Initiating warp sequence to Sector 02...";
                break;
        }
    }
}
