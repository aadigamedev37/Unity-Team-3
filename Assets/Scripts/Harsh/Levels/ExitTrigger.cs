using UnityEngine;
using StealMan.Core;
using Mohit.MetaSystems;

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
        GameEvents.RaiseLevelComplete(levelId);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel(levelId);
            return;
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CompleteLevel(levelId);
        }

        // Fallback to direct LevelManager if GameManager is not present
        if (levelManager == null)
        {
            levelManager = LevelManager.Instance ?? FindAnyObjectByType<LevelManager>();
        }

        if (levelManager != null)
        {
            StartCoroutine(DirectLoadNextLevelRoutine());
        }
        else
        {
            Debug.LogError("ExitTrigger could not find GameManager or LevelManager.", this);
        }
    }

    private System.Collections.IEnumerator DirectLoadNextLevelRoutine()
    {
        yield return new WaitForSeconds(1.2f);
        if (levelId < 3 && levelManager != null)
        {
            levelManager.LoadLevel(levelId + 1);
        }
    }

    private bool IsPlayer(Collider other)
    {
        // Explicitly reject Shadow entity
        if (other.name.ToLower().Contains("shadow") || 
            other.GetComponentInParent<IShadowController>() != null)
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