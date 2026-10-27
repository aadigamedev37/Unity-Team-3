using UnityEngine;

/// <summary>
/// Handles restarting the currently active level.
/// Keeps restart flow centralized so other systems do not
/// need to manage scene reloads themselves.
/// </summary>
public class RestartSystem : MonoBehaviour
{
    private LevelManager levelManager;
    private bool isRestarting = false;

    private void OnDisable()
    {
        isRestarting = false;
    }

    /// <summary>
    /// Restarts the level currently managed by LevelManager.
    /// </summary>
    public bool RestartCurrentLevel()
    {
        if (isRestarting)
            return false;

        isRestarting = true;

        if (levelManager == null)
        {
            levelManager = FindAnyObjectByType<LevelManager>();
        }

        if (levelManager != null && levelManager.CurrentLevel != null)
        {
            return levelManager.LoadLevel(levelManager.CurrentLevel.LevelId);
        }

        // Fallback for standalone/integration scenes without full LevelConfig
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.buildIndex >= 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(activeScene.buildIndex);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(activeScene.name);
        }

        return true;
    }

    public void ResetRestartState()
    {
        isRestarting = false;
    }
}