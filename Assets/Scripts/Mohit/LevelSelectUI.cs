using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
                Debug.LogError("LevelSelectUI: SaveManager instance not found.");
                return;
            }

            PopulateLevelButtons();
        }

        private void PopulateLevelButtons()
        {
            // Clear any existing buttons first (safe if this is called again later)
            foreach (Transform child in buttonContainer)
            {
                Destroy(child.gameObject);
            }

            for (int level = 1; level <= SaveManager.TotalLevels; level++)
            {
                GameObject buttonObj = Instantiate(levelButtonPrefab, buttonContainer);
                SetupButton(buttonObj, level);
            }
        }

        private void SetupButton(GameObject buttonObj, int levelNumber)
        {
            Button button = buttonObj.GetComponent<Button>();
            TMP_Text label = buttonObj.GetComponentInChildren<TMP_Text>();

            bool isUnlocked = SaveManager.Instance.IsLevelUnlocked(levelNumber);
            bool isCompleted = SaveManager.Instance.IsLevelCompleted(levelNumber);

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

        // In LevelSelectUI.cs — replace OnLevelButtonPressed with:
        private void OnLevelButtonPressed(int levelNumber)
        {
            LevelManager levelManager = FindAnyObjectByType<LevelManager>();

            if (levelManager != null)
            {
                levelManager.LoadLevel(levelNumber);
            }
            else
            {
                Debug.LogError("LevelSelectUI: No LevelManager found in the current scene. " +
                                "Check with Harsh/Aditya on how LevelManager persists across scenes.");
            }
        }

        // Hook this to a "Back" button if you add one
        public void OnBackPressed()
        {
            if (SceneFader.Instance != null)
                SceneFader.Instance.LoadScene("MainMenu");
            else
                SceneManager.LoadScene("MainMenu");
        }
    }
}