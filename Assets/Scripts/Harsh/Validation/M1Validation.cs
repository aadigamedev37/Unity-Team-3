using UnityEngine;

/// <summary>
/// Validates the assembled M1 technical proof scene.
/// Checks the required LevelRoot hierarchy and system references.
/// </summary>
public class M1TechnicalProofValidation : MonoBehaviour
{
    [Header("Level Structure")]
    [SerializeField]
    private Transform levelRoot;

    [SerializeField]
    private Transform environment;

    [SerializeField]
    private Transform hazards;

    [SerializeField]
    private Transform stealables;

    [SerializeField]
    private Transform spawn;

    [SerializeField]
    private Transform exit;

    [Header("Systems")]
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private RestartSystem restartSystem;

    private void Start()
    {
        ValidateM1Proof();
    }

    private void ValidateM1Proof()
    {
        bool passed = true;

        passed &= ValidateReference(levelRoot, "Level Root");
        passed &= ValidateReference(environment, "Environment");
        passed &= ValidateReference(hazards, "Hazards");
        passed &= ValidateReference(stealables, "Stealables");
        passed &= ValidateReference(spawn, "Spawn");
        passed &= ValidateReference(exit, "Exit");

        passed &= ValidateReference(levelManager, "Level Manager");
        passed &= ValidateReference(restartSystem, "Restart System");

        if (levelManager != null && levelManager.CurrentLevel == null)
        {
            Debug.LogError(
                "M1 VALIDATION FAIL: LevelManager has no current level.",
                this);

            passed = false;
        }

        if (passed)
        {
            Debug.Log(
                "M1 TECHNICAL PROOF: PASS.",
                this);
        }
        else
        {
            Debug.LogError(
                "M1 TECHNICAL PROOF: FAIL.",
                this);
        }
    }

    private bool ValidateReference(Object reference, string referenceName)
    {
        if (reference != null)
        {
            return true;
        }

        Debug.LogError(
            $"M1 VALIDATION FAIL: {referenceName} reference is missing.",
            this);

        return false;
    }
}