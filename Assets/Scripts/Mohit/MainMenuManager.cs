using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mohit.MetaSystems
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Scene Names")]
        [SerializeField] private string levelSelectSceneName = "LevelSelect";

        [Header("UI References")]
        [SerializeField] private GameObject mainMenuPanel;

        private void Start()
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogError("MainMenuManager: SaveManager instance not found. " +
                                "Make sure a SaveManager exists in the scene or persists from a previous scene.");
            }
        }

        // Hook this to your "Play" button
        public void OnPlayPressed()
        {
            int highestUnlocked = SaveManager.Instance.GetHighestUnlockedLevel();
            LoadLevel(highestUnlocked);
        }

        // Hook this to your "Level Select" button
        public void OnLevelSelectPressed()
        {
            SceneManager.LoadScene(levelSelectSceneName);
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

        // Placeholder until Harsh's LevelManager.LoadLevel API exists (Week 2 dependency)
        private void LoadLevel(int levelNumber)
        {
            Debug.Log($"MainMenuManager: Would load level {levelNumber} " +
                      $"(waiting on Harsh's LevelManager.LoadLevel API to wire this up for real).");

            // TODO (Week 2): Replace with actual call, e.g.
            // LevelManager.Instance.LoadLevel(levelNumber);
        }
    }
}