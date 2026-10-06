using UnityEngine;

/// <summary>
/// Handles restarting the currently active level.
/// Keeps restart flow centralized so other systems do not
/// need to manage scene reloads themselves.
/// </summary>
public class RestartSystem : MonoBehaviour
{
    private LevelManager levelManager;

    /// <summary>
    /// Restarts the level currently managed by LevelManager.
    /// </summary>
    public bool RestartCurrentLevel()
    {
        if (levelManager == null)
        {
            levelManager = FindFirstObjectByType<LevelManager>();
        }

        if (levelManager == null)
        {
            Debug.LogError(
                "RestartSystem could not find a LevelManager.",
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

        return levelManager.LoadLevel(levelManager.CurrentLevel.LevelId);
    }
}