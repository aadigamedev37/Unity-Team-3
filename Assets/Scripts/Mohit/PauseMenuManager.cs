using UnityEngine;
using UnityEngine.SceneManagement;

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
                pausePanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // TODO (MOH-009): Replace manual pause trigger with Aditya's GameManager.OnPaused event once confirmed
        public void TogglePause()
        {
            if (isPaused) Resume();
            else Pause();
        }

        public void Pause()
        {
            isPaused = true;
            Time.timeScale = 0f;
            if (pausePanel != null)
                pausePanel.SetActive(true);
        }

        public void Resume()
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        // Hook this to Pause panel's "Restart" button
        public void OnRestartPressed()
        {
            Time.timeScale = 1f; // always reset before reloading
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        // Hook this to Pause panel's "Main Menu" button
        public void OnMainMenuPressed()
        {
            Time.timeScale = 1f; // always reset before leaving

            if (SceneFader.Instance != null)
                SceneFader.Instance.LoadScene("MainMenu");
            else
                SceneManager.LoadScene("MainMenu");
        }

        // Hook this to Pause panel's "Resume" button
        public void OnResumePressed()
        {
            Resume();
        }
    }
}