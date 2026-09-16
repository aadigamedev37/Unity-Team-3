using UnityEngine;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    private void Start()
    {
        if (levelManager == null)
        {
            Debug.LogError(
                "BootstrapLoader requires a LevelManager.",
                this);
            return;
        }

        levelManager.LoadLevel(1);
    }
}   