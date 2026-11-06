using UnityEngine;
using StarterAssets;

public class SurfaceTraversal : MonoBehaviour
{
    public enum TraversalState
    {
        Floor,
        Wall
    }

    [Header("State")]
    public TraversalState currentState = TraversalState.Floor;

    [Header("Wall Detection")]
    [SerializeField] private Transform surfaceProbe;
    [SerializeField] private LayerMask surfaceLayer;
    [SerializeField] private float detectionDistance = 1.5f;
    [SerializeField] private float detectionRadius = 0.25f;

    [Header("Wall Movement")]
    [SerializeField] private float wallMoveSpeed = 3f;

    [Tooltip("Distance Shadow stays away from the wall surface.")]
    [SerializeField] private float wallOffset = 0.15f;

    [Tooltip("How far we check around the new position to find the wall.")]
    [SerializeField] private float wallCheckDistance = 1f;

    [Header("Character")]
    [SerializeField] private ThirdPersonController shadowController;
    [SerializeField] private StarterAssetsInputs shadowInputs;

    [Header("Skeleton")]
    [Tooltip("Assign the Skeleton / visual parent. Do NOT assign Shadow root.")]
    [SerializeField] private Transform skeleton;

    [Tooltip("Animator inside Skeleton. Automatically found if empty.")]
    [SerializeField] private Animator skeletonAnimator;

    [Header("Top / Feet")]
    [SerializeField] private Transform footProbe;
    [SerializeField] private float topReachTolerance = 0.05f;
    [SerializeField] private float topForwardOffset = 0.6f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTarget;

    private Quaternion savedCameraTargetRotation;

    // Original Skeleton rotation
    private Quaternion originalSkeletonLocalRotation;

    // Current wall information
    private Vector3 wallNormal;
    private Vector3 wallPoint;
    private float wallEnterTimestamp;

    // Rotation that should be applied to Skeleton while on wall.
    private Quaternion wallSkeletonRotation;

    // Keeps wall rotation active while Animator is enabled.
    private bool wallRotationActive;

    public LayerMask SurfaceLayer { get => surfaceLayer; set => surfaceLayer = value; }
    public Vector3 WallNormal => wallNormal;

    private void Awake()
    {
        EnsureReferences();
    }

    private void OnEnable()
    {
        EnsureReferences();
    }

    public void EnsureReferences()
    {
        if (skeleton == null)
        {
            Transform sk = transform.Find("body/Skeleton") ?? transform.Find("Skeleton");
            if (sk != null) skeleton = sk;
            else skeleton = transform;
        }

        if (skeleton != null)
        {
            originalSkeletonLocalRotation = skeleton.localRotation;
        }

        if (skeletonAnimator == null)
        {
            skeletonAnimator = GetComponentInChildren<Animator>();
        }

        if (shadowInputs == null)
        {
            shadowInputs = GetComponent<StarterAssetsInputs>();
        }

        if (shadowController == null)
        {
            shadowController = GetComponent<ThirdPersonController>();
        }

        if (cameraTarget == null)
        {
            cameraTarget = transform.Find("ShadowCameraRoot");
        }

        if (surfaceProbe == null)
        {
            Transform sp = transform.Find("SurfaceProbe");
            if (sp != null)
            {
                surfaceProbe = sp;
            }
            else
            {
                var go = new GameObject("SurfaceProbe");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = new Vector3(0f, 0.6f, 0f);
                surfaceProbe = go.transform;
            }
        }

        if (footProbe == null)
        {
            Transform fp = transform.Find("Foot") ?? transform.Find("FootProbe");
            if (fp != null)
            {
                footProbe = fp;
            }
            else
            {
                var go = new GameObject("FootProbe");
                go.transform.SetParent(transform, false);
                go.transform.localPosition = Vector3.zero;
                footProbe = go.transform;
            }
        }

        if (surfaceLayer.value == 0)
        {
            int shadowSurfaceLayer = LayerMask.NameToLayer("ShadowSurface");
            if (shadowSurfaceLayer >= 0)
            {
                surfaceLayer = (1 << shadowSurfaceLayer) | (1 << 0);
            }
            else
            {
                surfaceLayer = ~0;
            }
        }
    }

    public void SetInput(Vector2 input)
    {
        if (shadowInputs != null)
        {
            shadowInputs.move = input;
        }
    }

    private void Update()
    {
        if (currentState == TraversalState.Floor)
        {
            CheckForWall();
        }
        else if (currentState == TraversalState.Wall)
        {
            // Existing floor detection.
            CheckForFloor();

            if (currentState == TraversalState.Wall)
            {
                // Separate top-of-wall detection.
                CheckForWallTop();

                // Only leave wall when Shadow actually reaches it.
                CheckIfReachedWallTop();
            }

            if (currentState == TraversalState.Wall)
            {
                UpdateWallMovement();
                KeepShadowOnWall();
            }
        }
    }

    // =========================================================
    // LATE UPDATE
    // =========================================================
    // Animator updates before LateUpdate.
    //
    // Therefore we apply the wall Skeleton rotation again here
    // AFTER the Animator has updated.
    //
    // This allows correct wall-facing rotation.
    // =========================================================

    private void LateUpdate()
    {
        if (currentState != TraversalState.Wall)
            return;

        if (!wallRotationActive)
            return;

        // Animator can overwrite Skeleton rotation.
        // Apply our wall rotation after Animator has updated.
        if (skeleton != null)
        {
            skeleton.rotation = wallSkeletonRotation;
        }

        // Keep the camera in the wall view.
        SetCameraForWall(wallNormal);
    }

    // =========================================================
    // CAMERA
    // =========================================================

    private void SetCameraForWall(Vector3 wallNormal)
    {
        if (cameraTarget == null)
            return;

        /*
         * Camera should look in the same direction as
         * the wall normal.
         *
         * Wall normal = direction OUT from the wall.
         *
         * CameraTarget faces toward the wall so the camera
         * sees Shadow from the front.
         */

        Vector3 lookDirection = wallNormal;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude < 0.001f)
            return;

        lookDirection.Normalize();

        cameraTarget.rotation = Quaternion.LookRotation(-lookDirection, Vector3.up);
    }

    // =========================================================
    // FIND WALL
    // =========================================================

    public void CheckForWall()
    {
        if (surfaceProbe == null)
            EnsureReferences();

        if (surfaceProbe == null)
            return;

        RaycastHit hit;

        bool foundWall = Physics.SphereCast(
            surfaceProbe.position,
            detectionRadius,
            surfaceProbe.forward,
            out hit,
            detectionDistance,
            surfaceLayer,
            QueryTriggerInteraction.Ignore
        );

        if (!foundWall)
        {
            foundWall = Physics.Raycast(
                surfaceProbe.position,
                surfaceProbe.forward,
                out hit,
                detectionDistance,
                surfaceLayer,
                QueryTriggerInteraction.Ignore
            );
        }

        if (!foundWall)
            return;

        // Ignore floors and mostly-horizontal surfaces.
        float angle = Vector3.Angle(hit.normal, Vector3.up);

        if (angle < 75f)
            return;

        // Explicit Climbable Surface Validation: ONLY designated surfaces can be climbed!
        if (!IsClimbableSurface(hit.collider))
            return;

        EnterWall(hit);
    }

    public bool IsClimbableSurface(Collider col)
    {
        if (col == null) return false;

        // 1. Check for explicit ShadowClimbableSurface component
        var climbable = col.GetComponentInParent<ShadowClimbableSurface>();
        if (climbable != null)
        {
            return climbable.IsClimbable;
        }

        // 2. Check for dedicated ShadowSurface layer
        int shadowSurfaceLayer = LayerMask.NameToLayer("ShadowSurface");
        if (shadowSurfaceLayer >= 0 && col.gameObject.layer == shadowSurfaceLayer)
        {
            return true;
        }

        return false;
    }

    // =========================================================
    // ENTER WALL
    // =========================================================

    public void EnterWall(RaycastHit hit)
    {
        if (currentState == TraversalState.Wall)
            return;

        currentState = TraversalState.Wall;

        wallNormal = hit.normal.normalized;
        wallPoint = hit.point;
        wallEnterTimestamp = Time.time;

        /*
         * Save camera rotation before changing it.
         */
        if (cameraTarget != null)
        {
            savedCameraTargetRotation = cameraTarget.rotation;
        }

        /*
         * Disable normal CharacterController so it doesn't fight wall coordinates.
         */
        var cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        if (shadowController != null)
        {
            shadowController.enabled = false;
        }

        /*
         * Rotate Shadow's Skeleton so its back is against the wall.
         */
        RotateSkeletonForWall(wallNormal);

        /*
         * Put Shadow against the wall.
         */
        transform.position = hit.point + hit.normal * wallOffset;

        /*
         * Set camera to the standard wall view.
         */
        SetCameraForWall(wallNormal);
    }

    // =========================================================
    // SKELETON ROTATION
    // =========================================================

    private void RotateSkeletonForWall(Vector3 normal)
    {
        if (skeleton == null)
            return;

        /*
         * Wall normal points OUT of the wall.
         * Shadow's FRONT faces the wall normal.
         * Therefore Shadow's BACK is against the wall.
         */
        Vector3 facingDirection = normal;
        facingDirection.y = 0f;

        if (facingDirection.sqrMagnitude < 0.001f)
            return;

        facingDirection.Normalize();

        wallSkeletonRotation = Quaternion.LookRotation(facingDirection, Vector3.up);

        /*
         * Keep wall rotation active.
         * LateUpdate() will reapply this after Animator.
         */
        wallRotationActive = true;

        /*
         * Maintain locomotion blend tree active so walk animation works while climbing.
         */
        if (skeletonAnimator != null)
        {
            skeletonAnimator.enabled = true;
            skeletonAnimator.SetBool("Isdetach", false);
            skeletonAnimator.SetBool("Grounded", true);
        }

        /*
         * Apply immediately.
         */
        skeleton.rotation = wallSkeletonRotation;
    }

    // =========================================================
    // WALL MOVEMENT
    // =========================================================

    private void UpdateWallMovement()
    {
        Vector2 input = shadowInputs != null ? shadowInputs.move : Vector2.zero;

        if (input.sqrMagnitude < 0.001f)
        {
            if (skeletonAnimator != null)
            {
                skeletonAnimator.SetFloat("Speed", 0f);
                skeletonAnimator.SetFloat("MotionSpeed", 1f);
                skeletonAnimator.SetBool("Grounded", true);
                skeletonAnimator.SetBool("Isdetach", false);
            }
            return;
        }

        Vector3 wallRight = Vector3.Cross(Vector3.up, wallNormal).normalized;

        Vector3 desiredMovement = -wallRight * input.x + Vector3.up * input.y;

        // Explicitly project movement onto the wall surface.
        Vector3 projectedMovement = Vector3.ProjectOnPlane(desiredMovement, wallNormal);

        if (projectedMovement.sqrMagnitude < 0.001f)
            return;

        projectedMovement.Normalize();
        projectedMovement *= wallMoveSpeed * Time.deltaTime;

        TryMoveOnWall(projectedMovement);

        if (skeletonAnimator != null)
        {
            skeletonAnimator.SetFloat("Speed", 2f); // Walking animation
            skeletonAnimator.SetFloat("MotionSpeed", 1f);
            skeletonAnimator.SetBool("Grounded", true);
            skeletonAnimator.SetBool("Isdetach", false);
        }
    }

    // =========================================================
    // TRY MOVE ON REAL WALL
    // =========================================================

    private void TryMoveOnWall(Vector3 movement)
    {
        if (movement.sqrMagnitude < 0.000001f)
            return;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentPosition + movement;

        RaycastHit hit;
        Vector3 rayOrigin = targetPosition + wallNormal * wallCheckDistance;
        Vector3 rayDirection = -wallNormal;
        float rayLength = wallCheckDistance * 2f;

        bool foundWall = Physics.Raycast(
            rayOrigin,
            rayDirection,
            out hit,
            rayLength,
            surfaceLayer,
            QueryTriggerInteraction.Ignore
        );

        if (!foundWall)
        {
            // Stop at edge of wall.
            return;
        }

        // Make sure the detected surface is actually a wall.
        float angle = Vector3.Angle(hit.normal, Vector3.up);

        if (angle < 75f)
        {
            return;
        }

        // Explicit Climbable Surface Validation: stop at edge if surface ends
        if (!IsClimbableSurface(hit.collider))
        {
            return;
        }

        Vector3 newPosition = hit.point + hit.normal * wallOffset;

        // Leash constraint validation for wall climbing
        if (ShadowWalkAbility.Instance != null && !ShadowWalkAbility.Instance.ValidateProposedPosition(newPosition, out Vector3 clampedPos))
        {
            return;
        }

        transform.position = newPosition;

        // Update wall information.
        wallNormal = hit.normal.normalized;
        wallPoint = hit.point;

        RotateSkeletonForWall(wallNormal);
    }

    // =========================================================
    // KEEP SHADOW ATTACHED TO WALL
    // =========================================================

    private void KeepShadowOnWall()
    {
        RaycastHit hit;

        Vector3 rayOrigin = transform.position + wallNormal * wallCheckDistance;
        Vector3 rayDirection = -wallNormal;
        float rayLength = wallCheckDistance * 2f;

        bool foundWall = Physics.Raycast(
            rayOrigin,
            rayDirection,
            out hit,
            rayLength,
            surfaceLayer,
            QueryTriggerInteraction.Ignore
        );

        if (!foundWall)
        {
            LeaveWall();
            return;
        }

        // Check that it is still a wall.
        float angle = Vector3.Angle(hit.normal, Vector3.up);

        if (angle < 75f)
        {
            LeaveWall();
            return;
        }

        if (!IsClimbableSurface(hit.collider))
        {
            LeaveWall();
            return;
        }

        wallNormal = hit.normal.normalized;
        wallPoint = hit.point;

        Vector3 surfacePosition = hit.point + hit.normal * wallOffset;
        transform.position = surfacePosition;

        RotateSkeletonForWall(wallNormal);
    }

    // =========================================================
    // LEAVE WALL
    // =========================================================

    public void ExitWall()
    {
        LeaveWall();
    }

    private void LeaveWall()
    {
        if (currentState == TraversalState.Floor)
            return;

        currentState = TraversalState.Floor;

        wallRotationActive = false;

        if (skeleton != null)
        {
            skeleton.localRotation = originalSkeletonLocalRotation;
        }

        if (skeletonAnimator != null)
        {
            skeletonAnimator.enabled = true;
            skeletonAnimator.SetBool("Isdetach", false);
            skeletonAnimator.SetBool("Grounded", true);
        }

        if (cameraTarget != null)
        {
            cameraTarget.rotation = savedCameraTargetRotation;
        }

        var cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = true;
        }

        var adapter = GetComponent<ShadowMovementAdapter>();
        if (adapter != null)
        {
            adapter.OnExitedWall();
        }

        wallNormal = Vector3.zero;
        wallPoint = Vector3.zero;
    }

    // =========================================================
    // FLOOR DETECTION
    // =========================================================

    private void CheckForFloor()
    {
        if (Time.time < wallEnterTimestamp + 0.5f)
            return;

        Vector2 input = shadowInputs != null ? shadowInputs.move : Vector2.zero;
        if (input.y > 0.05f)
            return;

        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out hit,
            0.6f,
            surfaceLayer,
            QueryTriggerInteraction.Ignore))
        {
            float floorAngle = Vector3.Angle(hit.normal, Vector3.up);

            // Horizontal surface = floor
            if (floorAngle < 30f)
            {
                LeaveWall();
            }
        }
    }

    // =========================================================
    // TOP DETECTION
    // =========================================================

    private bool wallTopDetected;
    private RaycastHit wallTopHit;

    private void CheckForWallTop()
    {
        if (currentState != TraversalState.Wall)
            return;

        Vector3 rayOrigin = transform.position + Vector3.up * 1.0f - wallNormal * 0.5f;

        RaycastHit hit;

        bool foundTop = Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out hit,
            1.5f,
            surfaceLayer,
            QueryTriggerInteraction.Ignore
        );

        Debug.DrawRay(
            rayOrigin,
            Vector3.down * 1.5f,
            foundTop ? Color.green : Color.yellow
        );

        if (!foundTop)
        {
            wallTopDetected = false;
            return;
        }

        float angle = Vector3.Angle(hit.normal, Vector3.up);

        if (angle > 30f)
        {
            wallTopDetected = false;
            return;
        }

        wallTopDetected = true;
        wallTopHit = hit;
    }

    // =========================================================
    // REACH TOP WITH FEET
    // =========================================================

    private void CheckIfReachedWallTop()
    {
        if (!wallTopDetected)
            return;

        if (footProbe == null)
            return;

        float feetY = footProbe.position.y;
        float topY = wallTopHit.point.y;

        if (feetY < topY - topReachTolerance)
            return;

        float verticalCorrection = topY - footProbe.position.y;

        Vector3 newPosition = transform.position;
        newPosition += -wallNormal * topForwardOffset;
        newPosition.y += verticalCorrection;

        transform.position = newPosition;

        LeaveWall();

        wallTopDetected = false;
    }

    // =========================================================
    // DEBUG
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        if (surfaceProbe != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(surfaceProbe.position, detectionRadius);
            Gizmos.DrawLine(surfaceProbe.position, surfaceProbe.position + surfaceProbe.forward * detectionDistance);
        }

        if (currentState == TraversalState.Wall)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + wallNormal);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + wallNormal * wallCheckDistance, transform.position);
        }
    }
}
