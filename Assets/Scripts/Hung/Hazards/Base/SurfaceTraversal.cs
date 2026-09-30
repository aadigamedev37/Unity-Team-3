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

    // Rotation that should be applied to Skeleton while on wall.
    private Quaternion wallSkeletonRotation;

    // Keeps wall rotation active while Animator is enabled.
    private bool wallRotationActive;

    private void Awake()
    {
        if (skeleton != null)
        {
            originalSkeletonLocalRotation = skeleton.localRotation;
        }

        if (skeletonAnimator == null && skeleton != null)
        {
            skeletonAnimator = skeleton.GetComponentInChildren<Animator>();
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
    // This allows:
    // Isdetach = true
    // AND
    // correct wall-facing rotation.
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

        cameraTarget.rotation =
            Quaternion.LookRotation(
                -lookDirection,
                Vector3.up
            );
    }

    // =========================================================
    // FIND WALL
    // =========================================================

    private void CheckForWall()
    {
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
            return;

        // Ignore floors and mostly-horizontal surfaces.
        float angle = Vector3.Angle(hit.normal, Vector3.up);

        if (angle < 80f)
            return;

        EnterWall(hit);
    }

    // =========================================================
    // ENTER WALL
    // =========================================================

    private void EnterWall(RaycastHit hit)
    {
        if (currentState == TraversalState.Wall)
            return;

        currentState = TraversalState.Wall;

        wallNormal = hit.normal.normalized;
        wallPoint = hit.point;

        /*
         * Save camera rotation before changing it.
         */
        if (cameraTarget != null)
        {
            savedCameraTargetRotation =
                cameraTarget.rotation;
        }

        /*
         * Disable normal movement.
         */
        if (shadowController != null)
        {
            shadowController.enabled = false;
        }

        /*
         * Rotate Shadow's Skeleton so its back is
         * against the wall.
         */
        RotateSkeletonForWall(wallNormal);

        /*
         * Put Shadow against the wall.
         */
        transform.position =
            hit.point + hit.normal * wallOffset;

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
         *
         * Shadow's FRONT faces the wall normal.
         * Therefore Shadow's BACK is against the wall.
         */

        Vector3 facingDirection = normal;

        facingDirection.y = 0f;

        if (facingDirection.sqrMagnitude < 0.001f)
            return;

        facingDirection.Normalize();

        wallSkeletonRotation =
            Quaternion.LookRotation(
                facingDirection,
                Vector3.up
            );

        /*
         * Keep wall rotation active.
         *
         * LateUpdate() will reapply this after Animator.
         */
        wallRotationActive = true;

        /*
         * IMPORTANT:
         *
         * Animator must stay ENABLED so it can process
         * the Isdetach transition.
         */
        if (skeletonAnimator != null)
        {
            skeletonAnimator.enabled = true;
            skeletonAnimator.SetBool("Isdetach", true);
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
        if (shadowInputs == null)
            return;

        Vector2 input = shadowInputs.move;

        if (input.sqrMagnitude < 0.001f)
            return;

        Vector3 wallRight =
            Vector3.Cross(Vector3.up, wallNormal).normalized;

        Vector3 desiredMovement =
            -wallRight * input.x +
            Vector3.up * input.y;

        // Explicitly project movement onto the wall surface.
        Vector3 projectedMovement =
            Vector3.ProjectOnPlane(desiredMovement, wallNormal);

        if (projectedMovement.sqrMagnitude < 0.001f)
            return;

        projectedMovement.Normalize();

        projectedMovement *= wallMoveSpeed * Time.deltaTime;

        TryMoveOnWall(projectedMovement);
    }

    // =========================================================
    // TRY MOVE ON REAL WALL
    // =========================================================

    private void TryMoveOnWall(Vector3 movement)
    {
        if (movement.sqrMagnitude < 0.000001f)
            return;

        Vector3 currentPosition = transform.position;

        Vector3 targetPosition =
            currentPosition + movement;

        /*
         * Find the wall at the TARGET position.
         *
         * We raycast toward the current wall direction.
         * If there is a wall there, Shadow is allowed to move.
         *
         * If there is NO wall there, the movement is rejected.
         *
         * This is what creates the real wall boundary.
         */

        RaycastHit hit;

        Vector3 rayOrigin =
            targetPosition + wallNormal * wallCheckDistance;

        Vector3 rayDirection =
            -wallNormal;

        float rayLength =
            wallCheckDistance * 2f;

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
            // There is no wall at the target position.
            // Stop at the edge.
            return;
        }

        // Make sure the detected surface is actually a wall.
        float angle =
            Vector3.Angle(hit.normal, Vector3.up);

        if (angle < 80f)
        {
            return;
        }

        /*
         * The target position is valid.
         *
         * Put Shadow exactly at the wall surface,
         * maintaining wallOffset.
         */

        Vector3 newPosition =
            hit.point + hit.normal * wallOffset;

        transform.position = newPosition;

        // Update wall information.
        wallNormal = hit.normal.normalized;
        wallPoint = hit.point;

        // Update Skeleton direction in case the wall curves
        // or changes direction.
        RotateSkeletonForWall(wallNormal);
    }

    // =========================================================
    // KEEP SHADOW ATTACHED TO WALL
    // =========================================================

    private void KeepShadowOnWall()
    {
        RaycastHit hit;

        /*
         * Start slightly away from Shadow and raycast
         * toward the wall.
         */

        Vector3 rayOrigin =
            transform.position + wallNormal * wallCheckDistance;

        Vector3 rayDirection =
            -wallNormal;

        float rayLength =
            wallCheckDistance * 2f;

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
            /*
             * Shadow has completely left the wall.
             */
            LeaveWall();
            return;
        }

        // Check that it is still a wall.
        float angle =
            Vector3.Angle(hit.normal, Vector3.up);

        Debug.Log("Wall angle: " + angle);

        if (angle < 80f)
        {
            LeaveWall();
            return;
        }

        // Update wall information.
        wallNormal = hit.normal.normalized;
        wallPoint = hit.point;

        /*
         * Correct Shadow's distance from the wall.
         *
         * We preserve the movement along the wall,
         * but correct the perpendicular distance.
         */

        Vector3 surfacePosition =
            hit.point + hit.normal * wallOffset;

        transform.position = surfacePosition;

        // Make Skeleton follow wall direction.
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

        /*
         * Stop LateUpdate from forcing the wall rotation.
         */
        wallRotationActive = false;

        // Restore original Skeleton rotation.
        if (skeleton != null)
        {
            skeleton.localRotation =
                originalSkeletonLocalRotation;
        }

        /*
         * Turn Animator back on and return to normal state.
         */
        if (skeletonAnimator != null)
        {
            skeletonAnimator.enabled = true;
            skeletonAnimator.SetBool("Isdetach", false);
        }

        /*
         * Restore camera.
         */
        if (cameraTarget != null)
        {
            cameraTarget.rotation =
                savedCameraTargetRotation;
        }

        // Re-enable Starter Assets movement.
        if (shadowController != null)
        {
            shadowController.enabled = true;
        }

        wallNormal = Vector3.zero;
        wallPoint = Vector3.zero;
    }

    // =========================================================
    // FLOOR DETECTION
    // =========================================================

    private void CheckForFloor()
    {
        RaycastHit hit;

        Vector3 rayOrigin =
            transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out hit,
            0.6f,
            surfaceLayer,
            QueryTriggerInteraction.Ignore))
        {
            float floorAngle =
                Vector3.Angle(hit.normal, Vector3.up);

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

        /*
         * Check above Shadow.
         * We move the ray toward the wall's upper edge.
         */

        Vector3 rayOrigin =
            transform.position
            + Vector3.up * 1.0f
            - wallNormal * 0.5f;

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

        float angle =
            Vector3.Angle(hit.normal, Vector3.up);

        /*
         * Only a horizontal surface is considered
         * the top of the wall.
         */

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

        /*
         * The important point:
         *
         * We use the FEET position, not the Shadow root.
         */

        float feetY = footProbe.position.y;
        float topY = wallTopHit.point.y;

        /*
         * Feet have not reached the top yet.
         *
         * Shadow stays in Wall state and can continue climbing.
         */

        if (feetY < topY - topReachTolerance)
            return;

        /*
         * =====================================================
         * FEET HAVE REACHED THE TOP
         * =====================================================
         */

        /*
         * Calculate how much we need to move Shadow vertically
         * so the feet sit exactly on the top surface.
         */

        float verticalCorrection =
            topY - footProbe.position.y;

        /*
         * Move Shadow slightly onto the top surface.
         *
         * -wallNormal = direction onto the top of the wall.
         */

        Vector3 newPosition =
            transform.position;

        newPosition +=
            -wallNormal * topForwardOffset;

        newPosition.y += verticalCorrection;

        transform.position = newPosition;

        /*
         * Now the feet are actually on the top.
         */

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

            Gizmos.DrawWireSphere(
                surfaceProbe.position,
                detectionRadius
            );

            Gizmos.DrawLine(
                surfaceProbe.position,
                surfaceProbe.position +
                surfaceProbe.forward * detectionDistance
            );
        }

        if (currentState == TraversalState.Wall)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                transform.position,
                transform.position + wallNormal
            );

            Gizmos.color = Color.blue;

            Gizmos.DrawLine(
                transform.position + wallNormal * wallCheckDistance,
                transform.position
            );
        }
    }
}