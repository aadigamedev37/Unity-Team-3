
//production ready script



/*using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IPlayerAgent
{
    //Header for initializing speed mul. and gravity values
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform movementReference;

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

        HandleGravity();      //applying a downward acceleration here and ground checks
    }

    private void HandleMovement(Vector2 input)
    {
        if (isFrozen)
            return;

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


//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//test script (only for testing)
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IPlayerAgent
{
    //Header for initializing speed mul. and gravity values
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Transform movementReference;

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
}