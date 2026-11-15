using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using StealMan.Core;
using Mohit.MetaSystems;

/// <summary>
/// Detects the Player entering the level exit portal,
/// enforces tutorial restrictions, displays cinematic level completion directives,
/// and executes seamless cinematic screen-fade transitions to subsequent levels.
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int levelId = 1;
    [SerializeField] private bool isLocked = false;

    private LevelManager levelManager;
    private bool hasTriggered = false;

    public int LevelId { get => levelId; set => levelId = value; }
    public bool IsLocked { get => isLocked; set => isLocked = value; }
    public System.Action OnLockedExitAttempted;

    // Singleton fader canvas for seamless cinematic cross-scene fade
    private static GameObject faderCanvasInstance;
    private static CanvasGroup faderGroup;

    private void Awake()
    {
        EnsureFaderCanvas();
    }

    private void OnEnable()
    {
        hasTriggered = false;
    }

    private void Start()
    {
        // Fade in from black if coming from previous scene
        if (faderGroup != null && faderGroup.alpha > 0.05f)
        {
            StartCoroutine(FadeRoutine(1f, 0f, 0.6f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        // Strict rule: Only the Player body completes the level, NOT the Shadow!
        if (!IsPlayer(other))
            return;

        if (isLocked)
        {
            Debug.Log($"ExitTrigger: Exit is locked for Level {levelId} (tutorial requirement pending).");
            OnLockedExitAttempted?.Invoke();
            return;
        }

        hasTriggered = true;
        Debug.Log($"ExitTrigger: Player reached exit for Level {levelId}!");

        // 1. Notify tutorials and gameplay systems
        GameEvents.RaiseLevelComplete(levelId);

        // Notify active tutorial controllers
        var l01 = FindAnyObjectByType<L01_MovementTutorial>();
        if (l01 != null) l01.OnLevelCompleted();

        var l02 = FindAnyObjectByType<L02_SandboxTutorial>();
        if (l02 != null) l02.SendMessage("HandleLevelComplete", levelId, SendMessageOptions.DontRequireReceiver);

        var l03 = FindAnyObjectByType<L03_GhostGateTutorial>();
        if (l03 != null) l03.SendMessage("HandleLevelComplete", levelId, SendMessageOptions.DontRequireReceiver);

        // 2. Freeze player movement during cinematic exit
        var playerCtrl = other.GetComponentInParent<PlayerController>();
        if (playerCtrl != null)
        {
            playerCtrl.enabled = false;
        }

        // 3. Persist progression via SaveManager if active
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CompleteLevel(levelId);
        }

        // 4. Execute cinematic fade and transition
        StartCoroutine(CinematicExitRoutine());
    }

    private IEnumerator CinematicExitRoutine()
    {
        EnsureFaderCanvas();

        // Brief cinematic pause to savor completion (0.4s)
        yield return new WaitForSeconds(0.4f);

        // Smooth cinematic fade to black (0.7s)
        yield return StartCoroutine(FadeRoutine(0f, 1f, 0.7f));

        // Determine destination scene
        string targetScene = levelId switch
        {
            1 => "L02_Integration",
            2 => "L03_Integration",
            _ => "LevelSelect"
        };

        Debug.Log($"ExitTrigger: Seamlessly transitioning to '{targetScene}'...");

        // If SceneFader is available, delegate to it
        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.LoadScene(targetScene);
            yield break;
        }

        // Otherwise load via SceneManager asynchronously while screen is black
        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        if (op != null)
        {
            yield return op;
        }

        // Fade in from black in the new scene
        yield return StartCoroutine(FadeRoutine(1f, 0f, 0.6f));
    }

    private static void EnsureFaderCanvas()
    {
        if (faderCanvasInstance != null) return;

        faderCanvasInstance = new GameObject("CinematicFaderCanvas");
        DontDestroyOnLoad(faderCanvasInstance);

        Canvas canvas = faderCanvasInstance.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        faderCanvasInstance.AddComponent<CanvasScaler>();

        faderGroup = faderCanvasInstance.AddComponent<CanvasGroup>();
        faderGroup.alpha = 0f;
        faderGroup.blocksRaycasts = false;

        GameObject blackImg = new GameObject("BlackOverlay");
        blackImg.transform.SetParent(faderCanvasInstance.transform, false);

        Image img = blackImg.AddComponent<Image>();
        img.color = Color.black;

        RectTransform rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    private static IEnumerator FadeRoutine(float fromAlpha, float toAlpha, float duration)
    {
        if (faderGroup == null) yield break;

        faderGroup.blocksRaycasts = toAlpha > 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // Smooth step curve for movie-like ease in/out
            faderGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        faderGroup.alpha = toAlpha;
        faderGroup.blocksRaycasts = toAlpha > 0.5f;
    }

    private bool IsPlayer(Collider other)
    {
        // Explicitly reject Shadow entity
        if (other.name.ToLower().Contains("shadow") || 
            other.GetComponentInParent<IShadowController>() != null)
        {
            return false;
        }

        MonoBehaviour[] behaviours = other.GetComponentsInParent<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IPlayerController || behaviour is IPlayerAgent || behaviour is PlayerController)
            {
                return true;
            }
        }

        return other.CompareTag("Player");
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
