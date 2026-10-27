using UnityEngine;
using StealMan.Core;

/// <summary>
/// Detects the Player entering the level exit
/// and triggers validated level completion via GameManager.
/// </summary>
public class ExitTrigger : MonoBehaviour
{
    [SerializeField] private int levelId = 1;
    private LevelManager levelManager;
    private bool hasTriggered = false;

    public int LevelId { get => levelId; set => levelId = value; }

    private void OnEnable()
    {
        hasTriggered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        // Strict rule: Only the Player body completes the level, NOT the Shadow!
        if (!IsPlayer(other))
        {
            return;
        }

        hasTriggered = true;
        Debug.Log($"ExitTrigger: Player reached exit for Level {levelId}!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel(levelId);
            return;
        }

        // Fallback to direct LevelManager if GameManager is not present
        if (levelManager == null)
        {
            levelManager = LevelManager.Instance ?? FindAnyObjectByType<LevelManager>();
        }

        if (levelManager != null)
        {
            levelManager.LoadNextLevel();
        }
        else
        {
            Debug.LogError("ExitTrigger could not find GameManager or LevelManager.", this);
        }
    }

    private bool IsPlayer(Collider other)
    {
        // Explicitly reject Shadow entity
        if (other.name.ToLower().Contains("shadow") || 
            other.GetComponentInParent<IShadowController>() != null ||
            other.CompareTag("Shadow"))
        {
            return false;
        }

        MonoBehaviour[] behaviours = other.GetComponentsInParent<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IPlayerController || behaviour is IPlayerAgent || behaviour is PlayerController)
            {
                return true;
            }
        }

        return other.CompareTag("Player");
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}