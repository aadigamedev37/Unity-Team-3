
//production ready script



using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IPlayerAgent, IPlayerController
{
    //Header for initializing speed, gravity values and movement reference
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform movementReference;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float jumpTimeout = 0.15f;
    private float jumpTimeoutDelta = 0f;
    private bool jumpRequested = false;

    private CharacterController characterController; //reference to the CharacterController
    private Vector3 verticalVelocity; //stores the player's vertical falling velocity
    private bool isFrozen; //checks whether the player is currently frozen

    private void Awake()
    {
        characterController = GetComponent<CharacterController>(); //initialize the CharacterController here

        //Fallback to the player's own transform if no movement reference is assigned
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (movementReference == null)
        {
            var mainCam = Camera.main;
            movementReference = mainCam != null ? mainCam.transform : transform;
        }
    }

    //Player-side movement function.
    //The Input System will provide the Vector2 input from outside this class.
    public void HandleMovement(Vector2 input)
    {
        if (isFrozen)
            return;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        //Get the forward and right directions from the movement reference
        if (movementReference == null)
        {
            var mainCam = Camera.main;
            movementReference = mainCam != null ? mainCam.transform : transform;
        }

        Vector3 forward = movementReference.forward;
        Vector3 right = movementReference.right;

        //Ignore vertical rotation so movement stays on the ground plane
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        //To Convert the 2D input into a 3D movement direction
        Vector3 moveDirection =
            right * input.x +
            forward * input.y;

        //Apply the movement speed
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

        Vector3 movement = moveDirection * moveSpeed;

        if (jumpTimeoutDelta > 0f)
        {
            jumpTimeoutDelta -= Time.deltaTime;
        }

        // Robust ground check: CC isGrounded combined with downward raycast
        bool grounded = characterController.isGrounded || Physics.Raycast(transform.position, Vector3.down, 1.15f, ~0, QueryTriggerInteraction.Ignore);

        if (grounded)
        {
            if (verticalVelocity.y < 0f)
            {
                verticalVelocity.y = -2f;
            }

            if (jumpRequested && jumpTimeoutDelta <= 0f)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpTimeoutDelta = jumpTimeout;
                jumpRequested = false;
            }
        }
        else
        {
            jumpRequested = false;
        }

        //Apply gravity over time
        verticalVelocity.y += gravity * Time.deltaTime;

        //Combine horizontal movement and vertical velocity
        Vector3 finalMovement =
            movement + verticalVelocity;

        //Move the CharacterController once using the combined movement
        characterController.Move(finalMovement * Time.deltaTime);
    }

    public void Jump()
    {
        if (isFrozen)
            return;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        bool grounded = (characterController != null && characterController.isGrounded) ||
                        Physics.Raycast(transform.position, Vector3.down, 1.15f, ~0, QueryTriggerInteraction.Ignore);

        if (grounded)
        {
            jumpRequested = true;
        }
    }

    //Function used to freeze the player
    public void Freeze()
    {
        isFrozen = true; //set frozen state
        verticalVelocity = Vector3.zero; //clear downward velocity
    }

    //Function used to unfreeze the player
    public void Unfreeze()
    {
        isFrozen = false; //allow movement again
    }

    //Reset the player to the specified spawn position and rotation
    public void ResetToSpawn(Vector3 position, Quaternion rotation)
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (characterController != null)
            characterController.enabled = false; //disable CharacterController so the player can be repositioned safely

        transform.SetPositionAndRotation(position, rotation); //set the player's position and rotation

        verticalVelocity = Vector3.zero; //reset vertical velocity
        isFrozen = false; //reset frozen state

        characterController.enabled = true; //enable the CharacterController again
    }

    public void Reset()
    {
        ResetToSpawn(transform.position, transform.rotation);
    }
}



//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//test script (only for testing)
/*using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IPlayerAgent
{
    //Header for initializing speed mul. and gravity values
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform movementReference;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float jumpTimeout = 0.15f;
    private float jumpTimeoutDelta = 0f;
    private bool jumpRequested = false;

    private CharacterController characterController;  //reference to character controller
    private Vector3 verticalVelocity;       //falling velocity vertical axis and a boolean to check and freeze the player state
    private bool isFrozen;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>(); //initialized the char controller here
    }

    //frozen state for the player all updates will be skipped for this frame
    private void Update()
    {
        if (isFrozen)
            return;

        HandleMovement();
        HandleGravity();      //applying a downward acceleration here and ground checks
    }

    // Temporary keyboard input used only for isolated PlayerTestScene testing.
    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 input = Vector2.ClampMagnitude(
            new Vector2(horizontal, vertical), 1f);

        Vector3 forward = movementReference.forward;
        Vector3 right = movementReference.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection =
            right * input.x +
            forward * input.y;

        characterController.Move(
            moveDirection * moveSpeed * Time.deltaTime);
    }

    private void HandleGravity() //this fnction handles the gravity
    {
        if (characterController.isGrounded && verticalVelocity.y < 0f)     //if the char is grounded(i.e. ground check is ==true and vertical velocity(y) is less than 0f)
        {
            verticalVelocity.y = -2f; //applied a vertical velocity
        }

        verticalVelocity.y += gravity * Time.deltaTime; //gravity can be initialized a value and that multiplied time.deltatime(if u dont know its used to make, game logic and movement not to depend on the frame rates of diff. devices)

        characterController.Move(verticalVelocity * Time.deltaTime); // so its like i referred to the character controller and used a move fnc and the parameters are  (vertical velocity whose value we have now multiplied by time.deltatime)
    }

    public void Freeze() //the fnc to freeze
    {
        isFrozen = true;      //if true
        verticalVelocity = Vector3.zero; //clear the downwrd speed
    }

    public void Unfreeze() //to unfreeze
    {
        isFrozen = false;
    }

    public void ResetToSpawn(Vector3 position, Quaternion rotation)
    {
        characterController.enabled = false;      //disable collision to teleport

        transform.SetPositionAndRotation(position, rotation); //set the pos and rotatiom

        verticalVelocity = Vector3.zero; //velocity and states are reset here
        isFrozen = false;

        characterController.enabled = true; //enabled the collision i.e. the char controller
    }
}*/