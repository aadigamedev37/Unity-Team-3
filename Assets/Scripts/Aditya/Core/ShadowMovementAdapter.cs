using UnityEngine;



using StarterAssets;







/// <summary>



/// Universal adapter component for the Shadow entity.



/// Implements IShadowController, decoupling InputSystem from specific



/// shadow controller or physics internals (CharacterController, StarterAssets, SurfaceTraversal).



/// </summary>



public class ShadowMovementAdapter : MonoBehaviour, IShadowController



{



    [Header("Movement Settings")]



    [SerializeField] private float moveSpeed = 4f;



    [SerializeField] private Transform movementReference;







    [Header("Components (Auto-detected if unassigned)")]



    [SerializeField] private CharacterController characterController;



    [SerializeField] private Animator animator;



    [SerializeField] private Renderer[] shadowRenderers;



    [SerializeField] private SurfaceTraversal surfaceTraversal;



    [SerializeField] private StarterAssetsInputs shadowInputs;







    [Header("Rotation Settings")]



    [SerializeField] private float rotationSmoothTime = 0.12f;







    [Header("Physics & Gravity")]



    [SerializeField] private float gravity = -20f;



    private float verticalVelocity = 0f;



    [Header("Jump Settings")]

    [SerializeField] private float jumpHeight = 1.2f;

    private bool jumpRequested = false;

    private float jumpTimeoutDelta = 0f;







    [Header("Spawn Settings (Auto-detected if unassigned)")]



    [SerializeField] private Transform spawnAnchor;







    private bool isActive = false;



    private Vector3 spawnPosition;



    private Quaternion spawnRotation;



    private float rotationVelocity;







    public bool IsActive => isActive;



    public SurfaceTraversal Traversal => surfaceTraversal;







    private void Awake()



    {



        if (characterController == null)



        {



            characterController = GetComponent<CharacterController>();



        }







        if (animator == null)



        {



            animator = GetComponentInChildren<Animator>();



        }







        if (surfaceTraversal == null)



        {



            surfaceTraversal = GetComponent<SurfaceTraversal>();



        }







        if (shadowInputs == null)



        {



            shadowInputs = GetComponent<StarterAssetsInputs>();



        }







        shadowRenderers = GetComponentsInChildren<Renderer>(true);



        SetRenderersVisible(false);







        if (spawnAnchor == null)



        {



            var player = FindAnyObjectByType<PlayerController>();



            if (player != null)



            {



                Transform anchor = player.transform.Find("ShadowAnchor");



                spawnAnchor = anchor != null ? anchor : player.transform;



            }



        }







        // Deconflict: Disable ThirdPersonController and PlayerInput on Shadow so they do not fight this adapter



        var tpc = GetComponent<StarterAssets.ThirdPersonController>();



        if (tpc != null)



        {



            tpc.enabled = false;



        }







        var pi = GetComponent<UnityEngine.InputSystem.PlayerInput>();



        if (pi != null)



        {



            pi.enabled = false;



        }







        if (movementReference == null)



        {



            Camera mainCam = Camera.main;



            if (mainCam != null)



            {



                movementReference = mainCam.transform;



            }



            else



            {



                movementReference = transform;



            }



        }







        spawnPosition = transform.position;



        spawnRotation = transform.rotation;



    }







    private void Start()



    {



        // At start, the player and shadow begin in the Attached state.



        // Only the player must be present and visible in the beginning.



        var sm = FindAnyObjectByType<PlayerShadowStateMachine>();



        if (sm == null || sm.CurrentState == PlayerShadowState.Attached)



        {



            SnapToAnchor();



            Deactivate();



        }



    }







    private void SnapToAnchor()



    {



        if (spawnAnchor == null)



        {



            var player = FindAnyObjectByType<PlayerController>();



            if (player != null)



            {



                Transform anchor = player.transform.Find("ShadowAnchor");



                spawnAnchor = anchor != null ? anchor : player.transform;



            }



        }







        if (spawnAnchor != null)



        {



            bool ccWasEnabled = characterController != null && characterController.enabled;



            if (ccWasEnabled) characterController.enabled = false;



            transform.SetPositionAndRotation(spawnAnchor.position, spawnAnchor.rotation);



            if (ccWasEnabled) characterController.enabled = true;



        }



    }







    public void Activate()



    {



        isActive = true;



        gameObject.SetActive(true);







        SnapToAnchor();







        if (surfaceTraversal != null && surfaceTraversal.currentState == SurfaceTraversal.TraversalState.Wall)



        {



            surfaceTraversal.ExitWall();



        }







        if (characterController != null)



        {



            characterController.enabled = true;



        }







        SetRenderersVisible(true);







        if (animator != null)



        {



            animator.enabled = true;



            animator.SetBool("Isdetach", false);



            animator.SetBool("Grounded", true);



            animator.SetFloat("Speed", 0f);



            animator.SetFloat("MotionSpeed", 1f);



        }







        Debug.Log("ShadowMovementAdapter: Shadow Activated.");



    }







    public void Deactivate()



    {



        isActive = false;







        if (surfaceTraversal != null && surfaceTraversal.currentState == SurfaceTraversal.TraversalState.Wall)



        {



            surfaceTraversal.ExitWall();



        }







        if (characterController != null)



        {



            characterController.enabled = false;



        }



        SetRenderersVisible(false);







        if (animator != null)



        {



            animator.SetBool("Isdetach", false);



            animator.SetFloat("Speed", 0f);



        }







        if (shadowInputs != null)



        {



            shadowInputs.move = Vector2.zero;



        }







        Debug.Log("ShadowMovementAdapter: Shadow Deactivated.");



    }







    public void Stop()



    {



        if (characterController != null && characterController.enabled)



        {



            characterController.Move(Vector3.zero);



        }







        if (shadowInputs != null)



        {



            shadowInputs.move = Vector2.zero;



        }







        if (animator != null)



        {



            animator.SetFloat("Speed", 0f);



            animator.SetBool("Grounded", true);



            animator.SetBool("Isdetach", false);



        }



    }







    public void Reset()



    {



        if (surfaceTraversal != null && surfaceTraversal.currentState == SurfaceTraversal.TraversalState.Wall)



        {



            surfaceTraversal.ExitWall();



        }







        if (characterController != null)



        {



            characterController.enabled = false;



        }







        SnapToAnchor();







        isActive = false;



        SetRenderersVisible(false);







        if (animator != null)



        {



            animator.SetBool("Isdetach", false);



            animator.SetFloat("Speed", 0f);



        }







        if (shadowInputs != null)



        {



            shadowInputs.move = Vector2.zero;



        }



    }







    public void SetSpawnPosition(Vector3 position, Quaternion rotation)



    {



        spawnPosition = position;



        spawnRotation = rotation;



    }







    public void Jump()

    {

        if (!isActive) return;

        if (surfaceTraversal != null && surfaceTraversal.currentState == SurfaceTraversal.TraversalState.Wall) return;

        if (characterController != null && characterController.isGrounded)

        {

            jumpRequested = true;

        }

    }



    public void OnExitedWall()



    {



        verticalVelocity = 0f;



        if (characterController != null)



        {



            characterController.enabled = true;



        }



    }







    public void HandleMovement(Vector2 input)



    {



        if (!isActive)



            return;







        // Forward input to shadowInputs so SurfaceTraversal can access it



        if (shadowInputs != null)



        {



            shadowInputs.move = input;



        }







        // If currently on a wall, SurfaceTraversal handles climbing movement in its Update()



        if (surfaceTraversal != null && surfaceTraversal.currentState == SurfaceTraversal.TraversalState.Wall)



        {



            if (characterController != null && characterController.enabled)



            {



                characterController.enabled = false;



            }



            return;



        }







        float inputMag = input.magnitude;



        if (inputMag < 0.001f)



        {



            // Apply gravity even when stationary if airborne



            if (characterController != null && characterController.enabled)



            {



                if (!characterController.isGrounded)



                {



                    verticalVelocity += gravity * Time.deltaTime;



                    characterController.Move(Vector3.up * (verticalVelocity * Time.deltaTime));



                }



                else if (verticalVelocity < 0f)



                {



                    verticalVelocity = -2f;



                }



            }







            if (animator != null)



            {



                bool isGrounded = characterController != null ? characterController.isGrounded : true;



                animator.SetFloat("Speed", 0f);



                animator.SetFloat("MotionSpeed", 1f);



                animator.SetBool("Grounded", isGrounded);



                animator.SetBool("Isdetach", false);



            }



            return;



        }







        if (characterController != null && !characterController.enabled)



        {



            characterController.enabled = true;



        }







        Vector3 forward = movementReference != null ? movementReference.forward : Vector3.forward;



        Vector3 right = movementReference != null ? movementReference.right : Vector3.right;







        forward.y = 0f;



        right.y = 0f;



        forward.Normalize();



        right.Normalize();







        Vector3 moveDir = (right * input.x + forward * input.y).normalized;



        Vector3 horizontalDelta = moveDir * (moveSpeed * Time.deltaTime);







        // Apply Leash Constraint!



        if (ShadowWalkAbility.Instance != null)



        {



            horizontalDelta = ShadowWalkAbility.Instance.ConstrainHorizontalMovement(transform.position, horizontalDelta);



        }







        // Apply Gravity and Ground Checks



        if (characterController != null && characterController.enabled)



        {



            if (!characterController.isGrounded)



            {



                verticalVelocity += gravity * Time.deltaTime;



            }



            else if (verticalVelocity < 0f)



            {



                verticalVelocity = -2f;



            }







            Vector3 totalDelta = horizontalDelta + Vector3.up * (verticalVelocity * Time.deltaTime);



            characterController.Move(totalDelta);



        }



        else



        {



            transform.position += horizontalDelta;



        }







        if (horizontalDelta.sqrMagnitude > 0.0001f)



        {



            float targetAngle = Mathf.Atan2(horizontalDelta.x, horizontalDelta.z) * Mathf.Rad2Deg;



            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);



            transform.rotation = Quaternion.Euler(0f, angle, 0f);



        }







        if (animator != null)



        {



            bool isGrounded = characterController != null ? characterController.isGrounded : true;



            float actualSpeed = horizontalDelta.magnitude / Time.deltaTime;



            animator.SetFloat("Speed", Mathf.Clamp(actualSpeed, 0f, 6f));



            animator.SetFloat("MotionSpeed", 1f);



            animator.SetBool("Grounded", isGrounded);



            animator.SetBool("Isdetach", false);



        }



    }







    private void SetRenderersVisible(bool visible)



    {



        if (shadowRenderers == null)



            return;







        foreach (var r in shadowRenderers)



        {



            if (r != null)



            {



                r.enabled = visible;



            }



        }



    }



}

