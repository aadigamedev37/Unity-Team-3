using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mohit.MetaSystems;

namespace StealMan.Core
{
    /// <summary>
    /// Central game manager orchestrating application flow, pause state, level completion,
    /// and cross-scene coordination between gameplay and UI/Meta systems.
    /// Provides the authoritative events: OnPaused (MOH-009) and OnLevelComplete (MOH-010).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<GameManager>();
                }
                return _instance;
            }
            set => _instance = value;
        }

        [Header("Runtime State")]
        [SerializeField] private int currentLevelId = 1;
        [SerializeField] private bool isPaused = false;

        // Events required by UI and Meta systems
        public event Action<bool> OnPaused;              // MOH-009: true when paused, false when resumed
        public event Action<int> OnLevelComplete;        // MOH-010: fires with completed level ID
        public event Action<string> OnLevelFailed;       // Fires when shadow destroyed or recovery fails

        public bool IsPaused => isPaused;
        public int CurrentLevelId => currentLevelId;

        private bool isLevelCompleting = false;
        private bool isLevelFailing = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (Instance == null)
            {
                GameObject gmObj = new GameObject("GameManager");
                gmObj.AddComponent<GameManager>();
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                if (Application.isPlaying)
                    Destroy(gameObject);
                else
                    DestroyImmediate(gameObject);
                return;
            }

            _instance = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }

            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                SceneManager.sceneLoaded -= HandleSceneLoaded;
                _instance = null;
            }
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Safety: always ensure timeScale is reset to 1 on scene transitions
            Time.timeScale = 1f;
            isPaused = false;
            isLevelCompleting = false;
            isLevelFailing = false;

            // Sync current level ID from LevelManager if available
            if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null)
            {
                currentLevelId = LevelManager.Instance.CurrentLevel.LevelId;
            }
        }

        public void SetCurrentLevelId(int levelId)
        {
            currentLevelId = Mathf.Max(1, levelId);
        }

        #region Pause / Resume Flow (MOH-009)

        public void TogglePause()
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        public void Pause()
        {
            if (isPaused) return;

            isPaused = true;
            Time.timeScale = 0f;
            Debug.Log("[GameManager] Game Paused.");
            OnPaused?.Invoke(true);
        }

        public void Resume()
        {
            if (!isPaused) return;

            isPaused = false;
            Time.timeScale = 1f;
            Debug.Log("[GameManager] Game Resumed.");
            OnPaused?.Invoke(false);
        }

        /// <summary>
        /// Resets session completion and failure state flags (useful for test fixtures and level resets).
        /// </summary>
        public void ResetSessionState()
        {
            isPaused = false;
            isLevelCompleting = false;
            isLevelFailing = false;
        }

        #endregion

        #region Level Completion & Progression Flow (MOH-010)

        /// <summary>
        /// Validated level completion entry point.
        /// Guards against duplicate callbacks, persists progression, and notifies UI.
        /// </summary>
        public void CompleteLevel(int levelId)
        {
            if (isLevelCompleting)
            {
                Debug.LogWarning($"[GameManager] Ignoring duplicate CompleteLevel({levelId}) call.");
                return;
            }

            isLevelCompleting = true;
            currentLevelId = levelId;
            Debug.Log($"[GameManager] Level {levelId} Completed successfully!");

            // 1. Update persistent progression via SaveManager
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CompleteLevel(levelId);
            }
            else
            {
                Debug.LogWarning("[GameManager] SaveManager.Instance is null during level completion!");
            }

            // 2. Fire the authoritative level completion event
            GameEvents.RaiseLevelComplete(levelId);
            OnLevelComplete?.Invoke(levelId);

            // 3. Smooth transition to next level or level select
            StartCoroutine(AdvanceToNextLevelRoutine(levelId));
        }

        private System.Collections.IEnumerator AdvanceToNextLevelRoutine(int completedLevelId)
        {
            yield return new WaitForSeconds(1.2f);
            isLevelCompleting = false;

            if (completedLevelId < 10)
            {
                int nextLevelId = completedLevelId + 1;
                Debug.Log($"[GameManager] Advancing to Level {nextLevelId}...");
                LoadLevel(nextLevelId);
            }
            else
            {
                Debug.Log("[GameManager] Level 10 completed! Returning to Level Select.");
                LoadLevelSelect();
            }
        }

        /// <summary>
        /// Handles level failure (e.g. shadow destroyed or recovery failed).
        /// </summary>
        public void FailLevel(string reason)
        {
            if (isLevelFailing || isLevelCompleting)
                return;

            isLevelFailing = true;
            Debug.LogWarning($"[GameManager] Level failed: {reason}");
            OnLevelFailed?.Invoke(reason);
        }

        #endregion

        #region Level Navigation (MOH-012)

        public void RestartCurrentLevel()
        {
            Time.timeScale = 1f;
            isPaused = false;
            isLevelCompleting = false;
            isLevelFailing = false;

            RestartSystem restartSystem = FindAnyObjectByType<RestartSystem>();
            if (restartSystem != null)
            {
                restartSystem.RestartCurrentLevel();
                return;
            }

            if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevel != null)
            {
                LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentLevel.LevelId);
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }

        public void LoadNextLevel()
        {
            Time.timeScale = 1f;
            isPaused = false;
            isLevelCompleting = false;
            isLevelFailing = false;

            if (LevelManager.Instance != null)
            {
                bool started = LevelManager.Instance.LoadNextLevel();
                if (!started)
                {
                    Debug.LogWarning("[GameManager] LevelManager could not load next level, loading LevelSelect.");
                    LoadLevelSelect();
                }
            }
            else
            {
                int nextLevelId = currentLevelId + 1;
                LoadLevel(nextLevelId);
            }
        }

        public void LoadLevel(int levelId)
        {
            Time.timeScale = 1f;
            isPaused = false;
            isLevelCompleting = false;
            isLevelFailing = false;
            currentLevelId = levelId;

            if (LevelManager.Instance != null)
            {
                bool loaded = LevelManager.Instance.LoadLevel(levelId);
                if (loaded) return;
            }

            // Fallback: If LevelManager isn't configured, map level ID to known scenes
            string sceneName = GetSceneNameForLevel(levelId);
            Debug.Log($"[GameManager] Loading scene '{sceneName}' for level {levelId} via SceneManager fallback.");
            if (SceneFader.Instance != null)
                SceneFader.Instance.LoadScene(sceneName);
            else
                SceneManager.LoadScene(sceneName);
        }

        public void LoadLevelSelect()
        {
            Time.timeScale = 1f;
            isPaused = false;
            if (SceneFader.Instance != null)
                SceneFader.Instance.LoadScene("LevelSelect");
            else
                SceneManager.LoadScene("LevelSelect");
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            isPaused = false;
            if (SceneFader.Instance != null)
                SceneFader.Instance.LoadScene("MainMenu");
            else
                SceneManager.LoadScene("MainMenu");
        }

        public static string GetSceneNameForLevel(int levelId)
        {
            switch (levelId)
            {
                case 1: return "L01_Integration";
                case 2: return "L02_Integration";
                case 3: return "L03_Integration";
                case 4: return "L04_Integration";
                case 5: return "L05_Integration";
                case 6: return "L06_Blockout";
                case 7: return "L07_Blockout";
                case 8: return "L08_Blockout";
                case 9: return "L09_Blockout";
                case 10: return "L10_Blockout";
                default: return $"L{levelId:D2}_Blockout";
            }
        }

        #endregion
    }
}
