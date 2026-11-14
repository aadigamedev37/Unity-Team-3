using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Level Configuration")]

    // List of all level configurations available to the LevelManager.
    // Each LevelConfig contains the data needed to identify and load a level.
    [SerializeField]
    private List<LevelConfig> levels = new();

    // ID of the level that should be treated as the starting level.
    [SerializeField]
    private int startingLevelId = 1;

    // The level currently being managed.
    // Other systems can read this value, but only LevelManager can change it.
    public LevelConfig CurrentLevel { get; private set; }

    // Indicates whether a level transition is currently in progress.
    // Other systems can check this, but only LevelManager can change it.
    public bool IsLoading { get; private set; }

    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<LevelManager>();
            }
            return instance;
        }
        set => instance = value;
    }

    public IReadOnlyList<LevelConfig> Levels
    {
        get
        {
            EnsureAllLevelsConfigured();
            return levels;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (instance == null)
        {
            GameObject lmObj = new GameObject("LevelManager");
            lmObj.AddComponent<LevelManager>();
        }
    }

    /// <summary>
    /// Initializes the LevelManager when the object is created.
    /// Validates the configured levels and determines the starting level.
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this && Application.isPlaying)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }

        EnsureAllLevelsConfigured();
        ValidateLevelConfiguration();
        RefreshCurrentLevelFromActiveScene();
        if (CurrentLevel == null)
        {
            CurrentLevel = GetLevelById(startingLevelId);
        }

        if (CurrentLevel == null)
        {
            Debug.LogError(
                $"LevelManager could not find starting level with ID {startingLevelId}.",
                this);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshCurrentLevelFromActiveScene();
    }

    public void RefreshCurrentLevelFromActiveScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        foreach (LevelConfig level in Levels)
        {
            if (level != null && level.SceneName == sceneName)
            {
                CurrentLevel = level;
                return;
            }
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void EnsureAllLevelsConfigured()
    {
        if (levels == null) levels = new List<LevelConfig>();

#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:LevelConfig");
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            LevelConfig cfg = UnityEditor.AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
            if (cfg != null && !levels.Contains(cfg))
            {
                levels.Add(cfg);
            }
        }
#endif

        for (int i = 1; i <= 15; i++)
        {
            if (GetLevelById(i) == null)
            {
                LevelConfig runtimeConfig = ScriptableObject.CreateInstance<LevelConfig>();
                string sceneName = i == 1 ? "L01_Integration" : (i == 2 ? "L02_Integration" : (i == 3 ? "L03_Integration" : $"L{i:02d}_Blockout"));
                runtimeConfig.name = $"RuntimeLevel_{i:02d}";
                typeof(LevelConfig).GetField("levelId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(runtimeConfig, i);
                typeof(LevelConfig).GetField("displayName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(runtimeConfig, $"Level {i:02d}");
                typeof(LevelConfig).GetField("sceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(runtimeConfig, sceneName);
                levels.Add(runtimeConfig);
            }
        }
    }

    /// <summary>
    /// Finds a LevelConfig using its unique level ID.
    /// Returns null if no matching level is found.
    /// </summary>
    /// <param name="levelId">ID of the level to find.</param>
    /// <returns>The matching LevelConfig, or null if it doesn't exist.</returns>
    public LevelConfig GetLevelById(int levelId)
    {
        foreach (LevelConfig level in levels)
        {
            // Ignore empty references in the level list.
            if (level == null)
            {
                continue;
            }

            // Return the configuration when its ID matches the requested ID.
            if (level.LevelId == levelId)
            {
                return level;
            }
        }

        // No level with the requested ID was found.
        return null;
    }


    /// <summary>
    /// Starts loading a level using its configured level ID.
    /// The LevelConfig provides the scene name, keeping scene information
    /// separate from the LevelManager's logic.
    /// </summary>
    /// <param name="levelId">ID of the level to load.</param>
    /// <returns>True if the loading request was started successfully.</returns>
    public bool LoadLevel(int levelId)
    {
        // Prevent another transition from starting while one is already running.
        if (IsLoading)
        {
            Debug.LogWarning(
                "A level transition is already in progress.",
                this);

            return false;
        }

        // Find the configuration for the requested level.
        LevelConfig level = GetLevelById(levelId);

        // The requested level has not been configured.
        if (level == null)
        {
            Debug.LogError(
                $"Cannot load level {levelId}: no matching LevelConfig was found.",
                this);

            return false;
        }

        // A scene name is required for Unity to load the level.
        if (string.IsNullOrWhiteSpace(level.SceneName))
        {
            Debug.LogError(
                $"Cannot load level {levelId}: SceneName is empty.",
                this);

            return false;
        }

        // Start the asynchronous scene-loading process.
        StartCoroutine(LoadLevelRoutine(level));

        return true;
    }


    /// <summary>
    /// Loads the next configured level after the current level.
    /// </summary>
    /// <returns>True if the next-level loading request was started.</returns>
    public bool LoadNextLevel()
    {
        // We cannot determine the next level without knowing the current level.
        if (CurrentLevel == null)
        {
            Debug.LogError(
                "Cannot load next level because CurrentLevel is null.",
                this);

            return false;
        }

        // Find the configured level with the next highest ID.
        LevelConfig nextLevel = GetNextLevel(CurrentLevel);

        // There is no level after the current one.
        if (nextLevel == null)
        {
            Debug.LogWarning(
                $"There is no level after level {CurrentLevel.LevelId}.",
                this);

            return false;
        }

        return LoadLevel(nextLevel.LevelId);
    }


    /// <summary>
    /// Finds the next level based on the lowest level ID greater than
    /// the current level's ID.
    /// 
    /// The list does not need to be in numerical order for this to work.
    /// </summary>
    private LevelConfig GetNextLevel(LevelConfig currentLevel)
    {
        LevelConfig nextLevel = null;

        foreach (LevelConfig level in levels)
        {
            // Ignore empty references.
            if (level == null)
            {
                continue;
            }

            // Ignore the current level and any levels before it.
            if (level.LevelId <= currentLevel.LevelId)
            {
                continue;
            }

            // Keep the lowest ID found after the current level.
            if (nextLevel == null || level.LevelId < nextLevel.LevelId)
            {
                nextLevel = level;
            }
        }

        return nextLevel;
    }


    /// <summary>
    /// Handles the actual asynchronous scene-loading operation.
    /// </summary>
    /// <param name="level">The level configuration to load.</param>
    private IEnumerator LoadLevelRoutine(LevelConfig level)
    {
        // Mark the LevelManager as busy with a transition.
        IsLoading = true;

        // Ask Unity to load the scene without blocking the game thread.
        AsyncOperation operation =
            SceneManager.LoadSceneAsync(level.SceneName);

        // Loading failed to start.
        if (operation == null)
        {
            Debug.LogError(
                $"Failed to start loading scene '{level.SceneName}'.",
                this);

            IsLoading = false;
            yield break;
        }

        // Wait here until Unity finishes loading the scene.
        yield return operation;

        // The scene has successfully finished loading,
        // so this becomes the new current level.
        CurrentLevel = level;

        // The transition is complete.
        IsLoading = false;
    }


    /// <summary>
    /// Checks the configured levels for common configuration problems,
    /// such as missing references, duplicate IDs, or missing scene names.
    /// </summary>
    private void ValidateLevelConfiguration()
    {
        // HashSet stores IDs that have already been encountered.
        // This allows us to detect duplicate level IDs.
        HashSet<int> levelIds = new();

        foreach (LevelConfig level in levels)
        {
            // A null reference means a LevelConfig has not been assigned.
            if (level == null)
            {
                Debug.LogError(
                    "LevelManager contains a null LevelConfig reference.",
                    this);

                continue;
            }

            // Add the ID to the set.
            // Add() returns false if the ID already exists.
            if (!levelIds.Add(level.LevelId))
            {
                Debug.LogError(
                    $"Duplicate Level ID detected: {level.LevelId}.",
                    this);
            }

            // Every level needs a scene name to be loadable.
            if (string.IsNullOrWhiteSpace(level.SceneName))
            {
                Debug.LogError(
                    $"Level {level.LevelId} has an empty SceneName.",
                    this);
            }
        }
    }
}