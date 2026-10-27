using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using StealMan.Core;

namespace Mohit.MetaSystems
{
    public class PauseMenuManager : MonoBehaviour
    {
        public static PauseMenuManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject pausePanel;

        private bool isPaused = false;

        private void Awake()
        {
            Instance = this;          // always point at the manager in the current scene
            Time.timeScale = 1f;      // safety: never start a scene paused

            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
                BindButtons();
            }
        }

        private void OnEnable()
        {
            SubscribeToGameManager();
        }

        private void OnDisable()
        {
            UnsubscribeFromGameManager();
        }

        public void SubscribeToGameManager(GameManager gm = null)
        {
            var mgr = gm ?? GameManager.Instance;
            if (mgr != null)
            {
                mgr.OnPaused -= HandleGameManagerPaused;
                mgr.OnPaused += HandleGameManagerPaused;
            }
        }

        public void UnsubscribeFromGameManager(GameManager gm = null)
        {
            var mgr = gm ?? GameManager.Instance;
            if (mgr != null)
            {
                mgr.OnPaused -= HandleGameManagerPaused;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromGameManager();

            if (Instance == this)
                Instance = null;
        }

        private void HandleGameManagerPaused(bool paused)
        {
            isPaused = paused;
            if (pausePanel != null)
                pausePanel.SetActive(paused);
        }

        private void BindButtons()
        {
            if (pausePanel == null) return;

            Button[] buttons = pausePanel.GetComponentsInChildren<Button>(true);
            foreach (var btn in buttons)
            {
                string bName = btn.gameObject.name.ToLower();
                if (bName.Contains("resume"))
                {
                    btn.onClick.RemoveListener(OnResumePressed);
                    btn.onClick.AddListener(OnResumePressed);
                }
                else if (bName.Contains("restart"))
                {
                    btn.onClick.RemoveListener(OnRestartPressed);
                    btn.onClick.AddListener(OnRestartPressed);
                }
                else if (bName.Contains("mainmenu") || bName.Contains("menu"))
                {
                    btn.onClick.RemoveListener(OnMainMenuPressed);
                    btn.onClick.AddListener(OnMainMenuPressed);
                }
            }
        }

        // MOH-009: Delegate pause actions to GameManager
        public void TogglePause()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TogglePause();
            else
            {
                if (isPaused) Resume();
                else Pause();
            }
        }

        public void Pause()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Pause();
            }
            else
            {
                isPaused = true;
                Time.timeScale = 0f;
                if (pausePanel != null)
                    pausePanel.SetActive(true);
            }
        }

        public void Resume()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Resume();
            }
            else
            {
                isPaused = false;
                Time.timeScale = 1f;
                if (pausePanel != null)
                    pausePanel.SetActive(false);
            }
        }

        // Hook this to Pause panel's "Restart" button
        public void OnRestartPressed()
        {
            Time.timeScale = 1f; // always reset before reloading
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartCurrentLevel();
            }
            else
            {
                Scene currentScene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(currentScene.name);
            }
        }

        // Hook this to Pause panel's "Main Menu" button
        public void OnMainMenuPressed()
        {
            Time.timeScale = 1f; // always reset before leaving
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadMainMenu();
            }
            else
            {
                if (SceneFader.Instance != null)
                    SceneFader.Instance.LoadScene("MainMenu");
                else
                    SceneManager.LoadScene("MainMenu");
            }
        }

        // Hook this to Pause panel's "Resume" button
        public void OnResumePressed()
        {
            Resume();
        }
    }
}