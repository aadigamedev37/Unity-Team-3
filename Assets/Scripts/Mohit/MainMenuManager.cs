using UnityEngine;
using UnityEngine.SceneManagement;
using StealMan.Core;

namespace Mohit.MetaSystems
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string levelSelectSceneName = "LevelSelect";

        [Header("UI References")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private SettingsUI settingsUI;

        private void Start()
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("MainMenuManager: SaveManager instance not found.");
            }

            if (settingsUI == null)
            {
                settingsUI = FindAnyObjectByType<SettingsUI>();
            }
        }

        // Hook this to your "Play" button
        public void OnPlayPressed()
        {
            int highestUnlocked = SaveManager.Instance != null ? SaveManager.Instance.GetHighestUnlockedLevel() : 1;
            LoadLevel(highestUnlocked);
        }

        // Hook this to your "Level Select" button
        public void OnLevelSelectPressed()
        {
            if (SceneFader.Instance != null)
                SceneFader.Instance.LoadScene(levelSelectSceneName);
            else
                SceneManager.LoadScene(levelSelectSceneName);
        }

        // Hook this to your "Settings" button
        public void OnSettingsPressed()
        {
            if (settingsUI != null)
            {
                settingsUI.Open();
            }
            else
            {
                Debug.LogWarning("MainMenuManager: SettingsUI reference not found.");
            }
        }

        // Hook this to your "Quit" button
        public void OnQuitPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // Loads the requested level via GameManager / LevelManager (MOH-012)
        private void LoadLevel(int levelNumber)
        {
            Debug.Log($"MainMenuManager: Loading level {levelNumber}...");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadLevel(levelNumber);
            }
            else if (LevelManager.Instance != null)
            {
                LevelManager.Instance.LoadLevel(levelNumber);
            }
            else
            {
                string sceneName = GameManager.GetSceneNameForLevel(levelNumber);
                if (SceneFader.Instance != null)
                    SceneFader.Instance.LoadScene(sceneName);
                else
                    SceneManager.LoadScene(sceneName);
            }
        }
    }
}