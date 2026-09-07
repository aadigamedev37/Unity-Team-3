using UnityEngine;

/// <summary>
/// Detects an object entering the level exit and requests
/// the next configured level from LevelManager.
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private LevelManager levelManager;

    private void OnTriggerEnter(Collider other)
    {
        if (levelManager == null)
        {
            Debug.LogError("ExitTrigger requires a LevelManager.", this);
            return;
        }

        levelManager.LoadNextLevel();
    }
}