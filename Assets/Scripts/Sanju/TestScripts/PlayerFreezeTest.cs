using UnityEngine;

public class PlayerFreezeTest : MonoBehaviour
{
    private PlayerController playerController;
    private IPlayerAgent playerAgent;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerAgent = playerController;
    }

    private void Update()
    {
        // Temporary keyboard input for isolated PlayerTestScene testing only.
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 input = Vector2.ClampMagnitude(
            new Vector2(horizontal, vertical), 1f);

        // Send the test input into the production PlayerController.
        playerController.HandleMovement(input);

        // Press F to freeze the player.
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerAgent.Freeze();
            Debug.Log("TEST: Player Frozen");
        }

        // Press U to unfreeze the player.
        if (Input.GetKeyDown(KeyCode.U))
        {
            playerAgent.Unfreeze();
            Debug.Log("TEST: Player Unfrozen");
        }
    }
}