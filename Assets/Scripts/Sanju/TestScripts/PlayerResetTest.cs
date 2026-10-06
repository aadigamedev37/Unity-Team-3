using UnityEngine;

public class PlayerResetTest : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerController.ResetToSpawn(
                spawnPoint.position,
                spawnPoint.rotation
            );

            Debug.Log("TEST: Player Reset");
        }
    }
}