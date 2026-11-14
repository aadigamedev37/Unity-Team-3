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
        [SerializeField] private TMP_Text totalStarsText;
        [SerializeField] private Color lockedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        [SerializeField] private Color completedColor = new Color(0.4f, 1f, 0.4f, 1f);
        [SerializeField] private Button[] chapterTabs;       // Tab_Ch1, Tab_Ch2, Tab_Ch3
        [SerializeField] private GameObject[] chapterRows;   // Row_BreakIn, Row_Lockdown, Row_Source
        [SerializeField] private string[] chapterNames = { "BREAK-IN", "LOCKDOWN", "THE SOURCE" };
        [SerializeField] private Sprite tabSelectedSprite;
        [SerializeField] private Sprite tabNormalSprite;
        [SerializeField] private Sprite[] chapterThumbnails; // 3 pictures, one per chapter
        private int selectedChapter;

        private void Start()
        {

            if (SaveManager.Instance == null)
            {
                Debug.LogWarning("LevelSelectUI: SaveManager instance not found.");
            }
            PopulateLevelButtons();
        }

        void OnEnable()
        {
            // SaveManager.Instance.CompleteLevel(1);  // temproary testing line, remove later
            // SaveManager.Instance.SaveScore(1, 1950, 3, 22f, 0); // temproary testing line, remove later
            PopulateLevelButtons();
            SetupTabs();
            int highest = SaveManager.Instance.GetHighestUnlockedLevel();
            SelectChapter(Mathf.Clamp((highest - 1) / 5, 0, 2));
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
            if (totalStarsText != null && SaveManager.Instance != null)
                totalStarsText.text = SaveManager.Instance.GetTotalStars() + " / " + SaveManager.Instance.GetMaxPossibleStars();
        }

        private void SetupButton(GameObject buttonObj, int levelNumber)
        {
            Button button = buttonObj.GetComponent<Button>();
            LevelCardView view = buttonObj.GetComponent<LevelCardView>();

            bool isUnlocked = SaveManager.Instance != null && SaveManager.Instance.IsLevelUnlocked(levelNumber);
            bool isCompleted = SaveManager.Instance != null && SaveManager.Instance.IsLevelCompleted(levelNumber);

            if (view != null)
            {
                LevelScoreData s = SaveManager.Instance != null ? SaveManager.Instance.GetScoreData(levelNumber) : null;
                int stars = s != null ? s.bestStars : 0;
                float best = s != null ? s.bestTime : 0f;
                view.Bind(levelNumber, isUnlocked, isCompleted, stars, best);
                view.SetThumbnail(chapterThumbnails[Mathf.Clamp((levelNumber - 1) / 5, 0, 2)], !isUnlocked);
                int highestNow = SaveManager.Instance.GetHighestUnlockedLevel();
                view.SetCurrent(isUnlocked, isUnlocked && !isCompleted && levelNumber == highestNow);
            }

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

        bool IsChapterUnlocked(int chapter)
        {
            return SaveManager.Instance.GetHighestUnlockedLevel() >= chapter * 5 + 1;
        }

        void SetupTabs()
        {
            for (int i = 0; i < chapterTabs.Length; i++)
            {
                int index = i; // copy, so each button keeps its own number
                chapterTabs[i].onClick.RemoveAllListeners();
                chapterTabs[i].onClick.AddListener(() =>
                {
                    if (IsChapterUnlocked(index)) SelectChapter(index);
                });

                // the lock icon shows only while the chapter is locked
                Transform icon = chapterTabs[i].transform.Find("Icon");
                if (icon != null) icon.gameObject.SetActive(!IsChapterUnlocked(i));
            }

            // the sidebar names the chapters, so hide the old row labels
            foreach (GameObject row in chapterRows)
            {
                Transform label = row.transform.Find("Label");
                if (label != null) label.gameObject.SetActive(false);
            }
        }

        void SelectChapter(int index)
        {
            selectedChapter = index;
            for (int i = 0; i < chapterRows.Length; i++)
                chapterRows[i].SetActive(i == index);

            for (int i = 0; i < chapterTabs.Length; i++)
            {
                bool selected = (i == index);

                Transform visual = chapterTabs[i].transform.Find("Visual");
                if (visual != null)
                {
                    Image img = visual.GetComponent<Image>();
                    if (img != null)
                    {
                        img.sprite = selected ? tabSelectedSprite : tabNormalSprite;
                        img.color = Color.white;
                    }
                }

                // two-line label, built here so every tab is identical
                TMP_Text label = chapterTabs[i].GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    string small = selected ? "#FFFFFF" : "#9A9AA6";
                    label.text = "<size=60%><color=" + small + ">CH.0" + (i + 1) + "</color></size>\n" + chapterNames[i];
                    label.color = selected ? Color.white : new Color(0.88f, 0.87f, 0.85f);
                }
            }
        }
    }
}