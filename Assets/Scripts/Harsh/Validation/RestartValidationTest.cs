using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Measures the time taken to reload the current level
/// through RestartSystem.
/// </summary>
public class RestartValidationTest : MonoBehaviour
{
    [SerializeField]
    private RestartSystem restartSystem;

    private float restartStartTime;
    private bool restartStarted;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void Start()
    {
        if (restartStarted)
        {
            return;
        }

        if (restartSystem == null)
        {
            Debug.LogError(
                "RestartValidationTest requires a RestartSystem.",
                this);

            return;
        }

        restartStarted = true;
        restartStartTime = Time.realtimeSinceStartup;

        if (!restartSystem.RestartCurrentLevel())
        {
            Debug.LogError("Restart validation could not start.");
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        float elapsed =
            Time.realtimeSinceStartup - restartStartTime;

        Debug.Log(
            $"Restart validation: {elapsed:F3}s");

        if (elapsed < 0.5f)
        {
            Debug.Log(
                $"VALIDATION PASS: Restart completed in {elapsed:F3}s.");
        }
        else
        {
            Debug.LogError(
                $"VALIDATION FAIL: Restart took {elapsed:F3}s.");
        }

        Destroy(gameObject);
    }
}