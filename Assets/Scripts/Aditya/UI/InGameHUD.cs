using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StealMan.Core;
using Mohit.MetaSystems;

namespace StealMan.UI
{
    /// <summary>
    /// Coordinates the in-game mobile HUD, including level information, objective,
    /// pause button and menu overlay, level completion screen, failure screen,
    /// and trapped state recovery countdown.
    /// </summary>
    public class InGameHUD : MonoBehaviour
    {
        [Header("Level Information")]
        [SerializeField] private TMP_Text levelIndicatorText;
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField] private string defaultObjective = "Steal the Door & Reach the Exit";

        [Header("Pause Controls & Overlay")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button pauseResumeButton;
        [SerializeField] private Button pauseRestartButton;
        [SerializeField] private Button pauseMainMenuButton;

        [Header("Level Completion Overlay")]
        [SerializeField] private GameObject completionPanel;
        [SerializeField] private TMP_Text completionTitleText;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button levelSelectButton;
        [SerializeField] private Button completionMainMenuButton;

        [Header("Level Failure Overlay")]
        [SerializeField] private GameObject failurePanel;
        [SerializeField] private TMP_Text failureTitleText;
        [SerializeField] private TMP_Text failureReasonText;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button failureMainMenuButton;

        [Header("Hazard & Trap Feedback")]
        [SerializeField] private GameObject trapBannerPanel;
        [SerializeField] private TMP_Text trapBannerText;

        [Header("Dependencies (Auto-found if unassigned)")]
        [SerializeField] private PlayerShadowStateMachine stateMachine;
        [SerializeField] private RecoverHandler recoverHandler;
        [SerializeField] private ContextualActionButton contextualActionButton;

        private void Awake()
        {
            if (stateMachine == null)
                stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();

            if (recoverHandler == null)
                recoverHandler = FindAnyObjectByType<RecoverHandler>();

            if (contextualActionButton == null)
                contextualActionButton = FindAnyObjectByType<ContextualActionButton>();

            // Setup button listeners
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveListener(OnPauseButtonClicked);
                pauseButton.onClick.AddListener(OnPauseButtonClicked);
            }

            if (pauseResumeButton != null)
                pauseResumeButton.onClick.AddListener(OnResumeClicked);

            if (pauseRestartButton != null)
                pauseRestartButton.onClick.AddListener(OnRestartClicked);

            if (pauseMainMenuButton != null)
                pauseMainMenuButton.onClick.AddListener(OnMainMenuClicked);

            if (nextLevelButton != null)
                nextLevelButton.onClick.AddListener(OnNextLevelClicked);

            if (levelSelectButton != null)
                levelSelectButton.onClick.AddListener(OnLevelSelectClicked);

            if (completionMainMenuButton != null)
                completionMainMenuButton.onClick.AddListener(OnMainMenuClicked);

            if (retryButton != null)
                retryButton.onClick.AddListener(OnRestartClicked);

            if (failureMainMenuButton != null)
                failureMainMenuButton.onClick.AddListener(OnMainMenuClicked);

            // Hide overlays initially
            if (pausePanel != null) pausePanel.SetActive(false);
            if (completionPanel != null) completionPanel.SetActive(false);
            if (failurePanel != null) failurePanel.SetActive(false);
            if (trapBannerPanel != null) trapBannerPanel.SetActive(false);
        }

        private void Start()
        {
            UpdateLevelDisplay();
        }

        private void OnEnable()
        {
            SubscribeToGameManager();

            if (stateMachine != null)
            {
                stateMachine.OnStateChanged -= HandleStateChanged;
                stateMachine.OnStateChanged += HandleStateChanged;
            }

            UpdateLevelDisplay();
        }

        private void OnDisable()
        {
            UnsubscribeFromGameManager();

            if (stateMachine != null)
            {
                stateMachine.OnStateChanged -= HandleStateChanged;
            }
        }

        public void SubscribeToGameManager(GameManager gm = null)
        {
            var mgr = gm ?? GameManager.Instance;
            if (mgr != null)
            {
                mgr.OnPaused -= HandlePauseStateChanged;
                mgr.OnPaused += HandlePauseStateChanged;
                mgr.OnLevelComplete -= HandleLevelCompleted;
                mgr.OnLevelComplete += HandleLevelCompleted;
                mgr.OnLevelFailed -= HandleLevelFailed;
                mgr.OnLevelFailed += HandleLevelFailed;
            }
        }

        public void UnsubscribeFromGameManager(GameManager gm = null)
        {
            var mgr = gm ?? GameManager.Instance;
            if (mgr != null)
            {
                mgr.OnPaused -= HandlePauseStateChanged;
                mgr.OnLevelComplete -= HandleLevelCompleted;
                mgr.OnLevelFailed -= HandleLevelFailed;
            }
        }

        private void Update()
        {
            // Update trapped recovery countdown & tap indicator
            if (stateMachine != null && stateMachine.CurrentState == PlayerShadowState.Trapped)
            {
                if (trapBannerPanel != null && !trapBannerPanel.activeSelf)
                    trapBannerPanel.SetActive(true);

                if (trapBannerText != null && recoverHandler != null)
                {
                    trapBannerText.text = $"<b><color=#FF4444>SHADOW TRAPPED!</color></b>\n<size=80%>Tap <b>RECOVER</b> {recoverHandler.RemainingTaps} time(s)!  [{recoverHandler.TimeRemaining:F1}s]</size>";
                }
            }
            else
            {
                if (trapBannerPanel != null && trapBannerPanel.activeSelf)
                    trapBannerPanel.SetActive(false);
            }
        }

        public void UpdateLevelDisplay()
        {
            int lvlId = GameManager.Instance != null ? GameManager.Instance.CurrentLevelId : 1;
            string displayName = $"LEVEL {lvlId:D2}";

            if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null)
            {
                displayName = LevelManager.Instance.CurrentLevel.DisplayName.ToUpper();
            }

            if (levelIndicatorText != null)
            {
                levelIndicatorText.text = $"<b>{displayName}</b>";
            }

            if (objectiveText != null)
            {
                objectiveText.text = $"<b>Objective:</b> {defaultObjective}";
            }
        }

        private void HandlePauseStateChanged(bool paused)
        {
            if (pausePanel != null)
            {
                pausePanel.SetActive(paused);
            }
        }

        private void HandleLevelCompleted(int levelId)
        {
            Debug.Log($"[InGameHUD] Displaying Level {levelId} Completion Screen.");
            if (completionPanel != null)
            {
                completionPanel.SetActive(true);
                if (completionTitleText != null)
                {
                    completionTitleText.text = $"<b><color=#4CAF50>LEVEL {levelId:D2} COMPLETE!</color></b>";
                }
            }
        }

        private void HandleLevelFailed(string reason)
        {
            Debug.Log($"[InGameHUD] Displaying Failure Screen: {reason}");
            if (failurePanel != null)
            {
                failurePanel.SetActive(true);
                if (failureReasonText != null)
                {
                    failureReasonText.text = reason;
                }
            }
        }

        private void HandleStateChanged(PlayerShadowState newState)
        {
            if (newState != PlayerShadowState.Trapped && trapBannerPanel != null)
            {
                trapBannerPanel.SetActive(false);
            }
        }

        #region UI Button Handlers

        public void OnPauseButtonClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TogglePause();
            else
                Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
        }

        public void OnResumeClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.Resume();
            else
            {
                Time.timeScale = 1f;
                if (pausePanel != null) pausePanel.SetActive(false);
            }
        }

        public void OnRestartClicked()
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
                GameManager.Instance.RestartCurrentLevel();
            else
            {
                var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
            }
        }

        public void OnNextLevelClicked()
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
                GameManager.Instance.LoadNextLevel();
            else
                OnLevelSelectClicked();
        }

        public void OnLevelSelectClicked()
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
                GameManager.Instance.LoadLevelSelect();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelect");
        }

        public void OnMainMenuClicked()
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
                GameManager.Instance.LoadMainMenu();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        #endregion
    }
}
