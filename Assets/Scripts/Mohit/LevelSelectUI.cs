using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using StealMan.Core;

namespace Mohit.MetaSystems
{
    public class LevelSelectUI : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Transform buttonContainer;   // Parent with a Grid/Vertical Layout Group
        [SerializeField] private GameObject levelButtonPrefab; // A single Button prefab with a TMP text child

        [Header("Colors")]
        [SerializeField] private Color unlockedColor = Color.white;
        [SerializeField] private Color lockedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        [SerializeField] private Color completedColor = new Color(0.4f, 1f, 0.4f, 1f);

        private void Start()
        {
            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("LevelSelectUI: SaveManager instance not found.");
            }

            PopulateLevelButtons();
        }

        private void OnEnable()
        {
            if (buttonContainer != null && buttonContainer.childCount > 0)
            {
                PopulateLevelButtons();
            }
        }

        public void PopulateLevelButtons()
        {
            if (buttonContainer == null || levelButtonPrefab == null)
                return;

            // Clear any existing buttons first
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }

            int total = SaveManager.TotalLevels;
            for (int level = 1; level <= total; level++)
            {
                GameObject buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
                SetupButton(buttonObj, level);
            }
        }

        private void SetupButton(GameObject buttonObj, int levelNumber)
        {
            Button button = buttonObj.GetComponent<Button>();
            TMP_Text label = buttonObj.GetComponentInChildren<TMP_Text>();

            bool isUnlocked = SaveManager.Instance != null && SaveManager.Instance.IsLevelUnlocked(levelNumber);
            bool isCompleted = SaveManager.Instance != null && SaveManager.Instance.IsLevelCompleted(levelNumber);

            if (label != null)
            {
                label.text = levelNumber.ToString("D2"); // "01", "02", etc.
                label.color = isCompleted ? completedColor : (isUnlocked ? unlockedColor : lockedColor);
            }

            button.interactable = isUnlocked;

            if (isUnlocked)
            {
                int capturedLevel = levelNumber; // avoid closure bug in loops
                button.onClick.AddListener(() => OnLevelButtonPressed(capturedLevel));
            }
        }

        // MOH-012: Connect LevelSelectUI to GameManager and LevelManager.LoadLevel API
        private void OnLevelButtonPressed(int levelNumber)
        {
            if (SaveManager.Instance != null && !SaveManager.Instance.IsLevelUnlocked(levelNumber))
            {
                Debug.LogWarning($"LevelSelectUI: Level {levelNumber} is locked.");
                return;
            }

            Debug.Log($"LevelSelectUI: Launching level {levelNumber}...");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadLevel(levelNumber);
            }
            else
            {
                LevelManager levelManager = LevelManager.Instance ?? FindAnyObjectByType<LevelManager>();
                if (levelManager != null)
                {
                    levelManager.LoadLevel(levelNumber);
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

        // Hook this to a "Back" button
        public void OnBackPressed()
        {
            if (SceneFader.Instance != null)
                SceneFader.Instance.LoadScene("MainMenu");
            else
                SceneManager.LoadScene("MainMenu");
        }
    }
}