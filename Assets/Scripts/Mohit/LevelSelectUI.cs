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
        [SerializeField] private Transform[] phaseContainers = new Transform[3];   // the three Cards objects
        [SerializeField] private GameObject levelButtonPrefab;                    // the LevelCard prefab
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
            PopulateLevelButtons();
        }

        public void PopulateLevelButtons()
        {
            if (phaseContainers == null || levelButtonPrefab == null)
                return;

            foreach (Transform container in phaseContainers)
            {
                if (container == null) continue;
                foreach (Transform child in container)
                    Destroy(child.gameObject);
            }

            int total = SaveManager.TotalLevels;
            for (int level = 1; level <= total; level++)
            {
                int phase = Mathf.Clamp((level - 1) / 5, 0, phaseContainers.Length - 1);
                if (phaseContainers[phase] == null) continue;

                GameObject buttonObj = Instantiate(levelButtonPrefab, phaseContainers[phase]);
                SetupButton(buttonObj, level);
            }
        }

        private void SetupButton(GameObject buttonObj, int levelNumber)
        {
            Button button = buttonObj.GetComponent<Button>();
            LevelCardView view = buttonObj.GetComponent<LevelCardView>();

            bool isUnlocked = SaveManager.Instance != null && SaveManager.Instance.IsLevelUnlocked(levelNumber);
            bool isCompleted = SaveManager.Instance != null && SaveManager.Instance.IsLevelCompleted(levelNumber);

            if (view != null)
                view.Bind(levelNumber, isUnlocked, isCompleted, 0, 0f);   // stars and time arrive in step 5

            button.interactable = isUnlocked;

            if (isUnlocked)
            {
                int capturedLevel = levelNumber;   // avoid the closure bug in loops
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