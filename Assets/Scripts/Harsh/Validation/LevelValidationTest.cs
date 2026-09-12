using UnityEngine;

/// <summary>
/// Validates the required structure and assembly rules
/// for levels LVL-001 through LVL-007.
/// </summary>
public class LevelValidationTest : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField]
    private int levelId;

    [SerializeField]
    private string expectedLevelName;

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

    private void Start()
    {
        ValidateLevel();
    }

    private void ValidateLevel()
    {
        bool valid = true;

        valid &= ValidateReference(levelRoot, "Level Root");
        valid &= ValidateReference(environment, "Environment");
        valid &= ValidateReference(hazards, "Hazards");
        valid &= ValidateReference(stealables, "Stealables");
        valid &= ValidateReference(spawn, "Spawn");
        valid &= ValidateReference(exit, "Exit");

        switch (levelId)
        {
            case 1:
                valid &= ValidateLevelOne();
                break;

            case 2:
                valid &= ValidateLevelTwo();
                break;

            case 3:
                valid &= ValidateLevelThree();
                break;

            case 4:
                valid &= ValidateLevelFour();
                break;

            case 5:
                valid &= ValidateLevelFive();
                break;

            case 6:
                valid &= ValidateLevelSix();
                break;

            case 7:
                valid &= ValidateLevelSeven();
                break;

            default:
                Debug.LogError(
                    $"Unsupported validation level ID: {levelId}.",
                    this);

                valid = false;
                break;
        }

        if (valid)
        {
            Debug.Log(
                $"VALIDATION PASS: LVL-{levelId:000} validation completed.",
                this);
        }
        else
        {
            Debug.LogError(
                $"VALIDATION FAIL: LVL-{levelId:000} validation failed.",
                this);
        }
    }

    private bool ValidateLevelOne()
    {
        return ValidateChildren(
            environment,
            "Environment");
    }

    private bool ValidateLevelTwo()
    {
        return ValidateChildren(
            environment,
            "Environment");
    }

    private bool ValidateLevelThree()
    {
        return ValidateChildren(
            stealables,
            "Stealables");
    }

    private bool ValidateLevelFour()
    {
        return ValidateChildren(
            environment,
            "Environment") &&
            ValidateChildren(
                stealables,
                "Stealables");
    }

    private bool ValidateLevelFive()
    {
        return ValidateChildren(
            hazards,
            "Hazards") &&
            ValidateChildren(
                stealables,
                "Stealables");
    }

    private bool ValidateLevelSix()
    {
        return ValidateMinimumChildren(
            stealables,
            "Stealables",
            2);
    }

    private bool ValidateLevelSeven()
    {
        return ValidateChildren(
            environment,
            "Environment");
    }

    private bool ValidateReference(
        Transform reference,
        string referenceName)
    {
        if (reference != null)
        {
            return true;
        }

        Debug.LogError(
            $"LVL-{levelId:000}: Missing {referenceName} reference.",
            this);

        return false;
    }

    private bool ValidateChildren(
        Transform parent,
        string parentName)
    {
        return ValidateMinimumChildren(
            parent,
            parentName,
            1);
    }

    private bool ValidateMinimumChildren(
        Transform parent,
        string parentName,
        int minimum)
    {
        if (parent == null)
        {
            return false;
        }

        if (parent.childCount >= minimum)
        {
            return true;
        }

        Debug.LogError(
            $"LVL-{levelId:000}: {parentName} requires at least {minimum} configured object(s).",
            this);

        return false;
    }
}