using UnityEngine;

public class LevelSystemTest : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    private void Start()
    {
        Debug.Log("LevelSystemTest: Requesting Level 1.");

        levelManager.LoadLevel(1);
    }
}