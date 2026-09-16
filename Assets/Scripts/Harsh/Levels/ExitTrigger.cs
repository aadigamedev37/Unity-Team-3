using UnityEngine;

/// <summary>
/// Detects the Player entering the level exit
/// and requests the next configured level.
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    private LevelManager levelManager;

    private void OnTriggerEnter(Collider other)
    {
        if (levelManager == null)
        {
            levelManager = FindFirstObjectByType<LevelManager>();
        }

        if (levelManager == null)
        {
            Debug.LogError(
                "ExitTrigger could not find a LevelManager.",
                this);
            return;
        }

        if (!IsPlayer(other))
        {
            return;
        }

        levelManager.LoadNextLevel();
    }

    private bool IsPlayer(Collider other)
    {
        MonoBehaviour[] behaviours =
            other.GetComponentsInParent<MonoBehaviour>();

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IPlayerController)
            {
                return true;
            }
        }

        return false;
    }
}