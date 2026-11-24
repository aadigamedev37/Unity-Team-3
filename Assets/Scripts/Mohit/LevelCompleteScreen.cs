using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mohit.MetaSystems
{
    public class LevelCompleteScreen : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject panel;
        [SerializeField] private CanvasGroup group;

        [Header("Texts")]
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text precisionText;
        [SerializeField] private TMP_Text stealsText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text bestText;
        [SerializeField] private GameObject newBestTag;

        [Header("Stars (3 Images, gold when earned)")]
        [SerializeField] private Image[] stars = new Image[3];

        [Header("Buttons")]
        [SerializeField] private Button skipButton;    // transparent full-card button behind the other three
        [SerializeField] private Button retryButton;
        [SerializeField] private Button levelsButton;
        [SerializeField] private Button nextButton;

        [Header("Scene names")]
        [SerializeField] private string levelSelectScene = "LevelSelect";

        public System.Action OnRetry;   // set by gameplay glue; falls back to reloading the scene
        public System.Action OnNext;    // set by gameplay glue; hidden if never set

        private Coroutine routine;
        private bool skip;
        private ScoreResult shown;

        private void Awake()
        {
            if (panel != null) panel.SetActive(false);
            if (skipButton != null)   skipButton.onClick.AddListener(() => skip = true);
            if (retryButton != null)  retryButton.onClick.AddListener(Retry);
            if (levelsButton != null) levelsButton.onClick.AddListener(Levels);
            if (nextButton != null)   nextButton.onClick.AddListener(Next);
        }

        public void Show(int levelId, string levelName, float time, int deaths, int steals,
                         int requiredSteals, ScoreResult result, int bestScore, bool isNewBest)
        {
            shown = result;
            if (panel != null) panel.SetActive(true);
            if (nextButton != null) nextButton.gameObject.SetActive(OnNext != null && levelId < SaveManager.TotalLevels);
            if (newBestTag != null) newBestTag.SetActive(false);

            levelText.text = $"LEVEL {levelId:D2} · {levelName.ToUpper()}";
            timeText.text = $"{time:F1} s";
            precisionText.text = $"{result.PrecisionPercent:F0}%";
            stealsText.text = $"{steals} / {requiredSteals}";
            bestText.text = isNewBest ? "" : $"BEST {bestScore}";
            scoreText.text = "0";
            foreach (var s in stars) if (s != null) s.color = UIColors.CrimsonDeep;

            if (routine != null) StopCoroutine(routine);
            skip = false;
            routine = StartCoroutine(Reveal(isNewBest));
        }

        private IEnumerator Reveal(bool isNewBest)
        {
            group.alpha = 0f;
            yield return Fade(0.15f);

            yield return Wait(0.65f);

            // Score count-up
            float t = 0f;
            while (t < 0.8f && !skip)
            {
                t += Time.unscaledDeltaTime;
                float k = 1f - Mathf.Pow(1f - Mathf.Clamp01(t / 0.8f), 3f);
                scoreText.text = Mathf.RoundToInt(shown.Score * k).ToString();
                yield return null;
            }
            scoreText.text = shown.Score.ToString();
            if (newBestTag != null) newBestTag.SetActive(isNewBest);

            // Stars
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] == null) continue;
                if (i < shown.Stars)
                {
                    stars[i].color = UIColors.Gold;
                    if (!skip) yield return Pop(stars[i].rectTransform);
                }
                yield return Wait(0.2f);
            }
            routine = null;
        }

        private IEnumerator Fade(float seconds)
        {
            float t = 0f;
            while (t < seconds && !skip)
            {
                t += Time.unscaledDeltaTime;
                group.alpha = Mathf.Clamp01(t / seconds);
                yield return null;
            }
            group.alpha = 1f;
        }

        private IEnumerator Wait(float seconds)
        {
            float t = 0f;
            while (t < seconds && !skip) { t += Time.unscaledDeltaTime; yield return null; }
        }

        private IEnumerator Pop(RectTransform rt)
        {
            float t = 0f;
            while (t < 0.18f && !skip)
            {
                t += Time.unscaledDeltaTime;
                float k = Mathf.Sin(Mathf.Clamp01(t / 0.18f) * Mathf.PI);   // 0 -> 1 -> 0
                rt.localScale = Vector3.one * (1f + 0.25f * k);
                yield return null;
            }
            rt.localScale = Vector3.one;
        }

        private void Retry()
        {
            Time.timeScale = 1f;
            if (OnRetry != null) OnRetry();
            else UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        private void Next()
        {
            Time.timeScale = 1f;
            OnNext?.Invoke();
        }

        private void Levels()
        {
            Time.timeScale = 1f;
            ScoringSystemBridge.ResetDeaths();
            if (SceneFader.Instance != null) SceneFader.Instance.LoadScene(levelSelectScene);
            else UnityEngine.SceneManagement.SceneManager.LoadScene(levelSelectScene);
        }
    }
}