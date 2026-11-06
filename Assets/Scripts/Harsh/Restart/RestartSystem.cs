
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles restarting the currently active level.
/// Uses LevelManager for managed levels and supports
/// standalone test scenes when no LevelManager exists.
/// </summary>
public class RestartSystem : MonoBehaviour
{
    private LevelManager levelManager;
    private bool isRestarting;

    private void OnEnable()
    {
        isRestarting = false;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    /// <summary>
    /// Restarts the currently active level.
    /// </summary>
    public bool RestartCurrentLevel()
    {
        if (isRestarting)
        {
            return false;
        }

        levelManager = LevelManager.Instance;

        if (levelManager != null)
        {
            if (levelManager.IsLoading)
            {
                Debug.LogWarning(
                    "Cannot restart while a level transition is in progress.",
                    this);
                return false;
            }

            if (levelManager.CurrentLevel == null)
            {
                Debug.LogError(
                    "Cannot restart because LevelManager has no current level.",
                    this);
                return false;
            }

            isRestarting = true;

            bool requestAccepted = levelManager.LoadLevel(
                levelManager.CurrentLevel.LevelId);

            if (!requestAccepted)
            {
                isRestarting = false;
            }

            return requestAccepted;
        }

        // Fallback only for standalone scenes without a LevelManager.
        Scene activeScene = SceneManager.GetActiveScene();

        if (!activeScene.IsValid() || !activeScene.isLoaded)
        {
            Debug.LogError(
                "Cannot restart because there is no valid active scene.",
                this);
            return false;
        }

        isRestarting = true;

        AsyncOperation operation = activeScene.buildIndex >= 0
            ? SceneManager.LoadSceneAsync(activeScene.buildIndex)
            : SceneManager.LoadSceneAsync(activeScene.name);

        if (operation == null)
        {
            isRestarting = false;

            Debug.LogError(
                $"Could not start reloading scene '{activeScene.name}'.",
                this);
            return false;
        }

        return true;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isRestarting = false;
        levelManager = LevelManager.Instance;
    }

    public void ResetRestartState()
    {
        isRestarting = false;
    }
}
