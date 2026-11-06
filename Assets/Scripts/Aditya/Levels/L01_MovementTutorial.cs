using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Guides new players through fundamental movement in Level 01.
/// Features:
/// 1. Prompts the player to move using Virtual Joystick or WASD.
/// 2. Pulses the VirtualJoystick on mobile until movement occurs.
/// 3. Dynamically updates instructions when movement begins.
/// 4. Prompts the player when approaching the exit portal.
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
            var exitZone = GameObject.Find("ExitZone") ?? GameObject.Find("ENV_ExitZone_ExitPortal_03");
            if (exitZone != null) exitPortal = exitZone.transform;
        }

        if (virtualJoystick != null)
        {
            originalJoystickScale = virtualJoystick.transform.localScale;
        }
    }

    private void Start()
    {
        if (player != null)
        {
            startPos = player.transform.position;
        }

        SetTutorialStep(Step.MovePrompt);
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
            if (distToExit < 4.5f)
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
                if (tutorialTitle != null) tutorialTitle.text = "MOVEMENT TUTORIAL";
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
                tutorialCard.SetActive(false);
                break;
        }
    }
}
