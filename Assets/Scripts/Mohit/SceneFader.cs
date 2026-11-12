using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mohit.MetaSystems
{
    public class SceneFader : MonoBehaviour
    {
        public static SceneFader Instance { get; private set; }

        [SerializeField] private CanvasGroup fadeGroup;
        [SerializeField] private float fadeDuration = 0.4f;

        private bool isFading;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            fadeGroup.alpha = 0f;
            fadeGroup.blocksRaycasts = false;
        }

        public void LoadScene(string sceneName)
        {
            if (isFading) return;
            StartCoroutine(FadeAndLoad(sceneName));
        }

        // Fade to black, run any action (load a level, quit...), then fade back in once a new scene is active.
        public void FadeThen(System.Action action)
        {
            if (isFading) return;
            StartCoroutine(FadeThenRoutine(action));
        }

        private IEnumerator FadeThenRoutine(System.Action action)
        {
            isFading = true;
            yield return Fade(0f, 1f);

            Scene before = SceneManager.GetActiveScene();
            action?.Invoke();

            // If the action loaded a scene, wait for it (max 3 s), then fade back in.
            float wait = 3f;
            while (SceneManager.GetActiveScene() == before && wait > 0f)
            {
                wait -= Time.unscaledDeltaTime;
                yield return null;
            }

            yield return Fade(1f, 0f);
            isFading = false;
        }

        private IEnumerator FadeAndLoad(string sceneName)
        {
            isFading = true;

            yield return Fade(0f, 1f);

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            if (op != null)
                yield return op;

            yield return Fade(1f, 0f);

            isFading = false;
        }

        private IEnumerator Fade(float from, float to)
        {
            fadeGroup.blocksRaycasts = true; // block clicks during the fade

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime; // works even if timeScale is 0
                fadeGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
                yield return null;
            }

            fadeGroup.alpha = to;
            fadeGroup.blocksRaycasts = to > 0.5f;
        }
    }
}