
using System.Collections;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShadowController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private ThirdPersonController playerController;

    [Header("Shadow")]
    [SerializeField] private Transform shadow;
    [SerializeField] private ThirdPersonController shadowController;
    [SerializeField] private GameObject skeleton;
    [Header("Light")]
    [SerializeField] private Light mainLight;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundOffset = 0.02f;

    [Header("Camera")]
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private Transform playerCameraTarget;
    [SerializeField] private Transform shadowCameraTarget;

    [Header("Input")]
    [SerializeField] private InputActionReference shadowToggleAction;

    [Header("Shadow Emergence")]
    [SerializeField] private float emergenceDuration = 0.8f;
    [SerializeField] private float emergenceHeight = 0.1f;

    [SerializeField]
    private AnimationCurve emergenceCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("State")]
    [SerializeField] public bool shadowControlled = false;

    private Renderer[] shadowRenderers;
    private Coroutine emergenceCoroutine;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        // Find all visual renderers belonging to the shadow.
        shadowRenderers =
            GetComponentsInChildren<Renderer>(true);
    }


    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        if (shadowToggleAction != null)
        {
            shadowToggleAction.action.Enable();
        }
    }


    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        if (shadowToggleAction != null)
        {
            shadowToggleAction.action.Disable();
        }
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Start in normal player mode.
        shadowControlled = false;


        // Hide shadow visually.
        // DO NOT deactivate the Shadow GameObject.
        SetShadowVisible(false);


        // Player is controllable at the beginning.
        if (playerController != null)
        {
            playerController.enabled = true;
        }


        // Shadow is NOT controllable at the beginning.
        if (shadowController != null)
        {
            shadowController.enabled = false;
        }


        // Camera follows player.
        if (cam != null &&
            playerCameraTarget != null)
        {
            cam.Follow = playerCameraTarget;
            cam.LookAt = playerCameraTarget;
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (shadowToggleAction == null)
            return;


        if (shadowToggleAction.action.WasPressedThisFrame())
        {
            if (shadowControlled)
            {
                ExitShadowMode();
            }
            else
            {
                EnterShadowMode();
            }
        }
    }


    // =========================================================
    // ENTER SHADOW MODE
    // =========================================================

    public void EnterShadowMode()
    {
        if (player == null ||
            shadow == null ||
            mainLight == null)
        {
            Debug.LogWarning(
                "ShadowController: Missing references."
            );

            return;
        }


        // Calculate where the real shadow should be.
        Vector3 shadowPosition =
            CalculateShadowPosition();


        if (shadowPosition == Vector3.zero)
        {
            Debug.LogWarning(
                "ShadowController: Could not find shadow position."
            );

            return;
        }


        // -----------------------------------------------------
        // CHANGE STATE
        // -----------------------------------------------------

        shadowControlled = true;


        // -----------------------------------------------------
        // STOP PLAYER
        // -----------------------------------------------------

        if (playerController != null)
        {
            playerController.enabled = false;
        }


        // -----------------------------------------------------
        // PREPARE SHADOW
        // -----------------------------------------------------

        if (shadowController != null)
        {
            shadowController.enabled = false;
        }


        shadow.position = shadowPosition;

        shadow.rotation = player.rotation;


        // Show shadow.
        SetShadowVisible(true);


        // -----------------------------------------------------
        // CAMERA
        // -----------------------------------------------------

        if (cam != null &&
            shadowCameraTarget != null)
        {
            shadowCameraTarget.position =
                shadow.position;

            cam.Follow = shadowCameraTarget;
            cam.LookAt = shadowCameraTarget;
        }


        // -----------------------------------------------------
        // EMERGENCE
        // -----------------------------------------------------

        if (emergenceCoroutine != null)
        {
            StopCoroutine(emergenceCoroutine);
        }


        emergenceCoroutine =
            StartCoroutine(
                ShadowEmergence(shadowPosition)
            );
    }


    // =========================================================
    // EXIT SHADOW MODE
    // =========================================================

    public void ExitShadowMode()
    {
        // Change state first.
        shadowControlled = false;


        // Stop emergence if still running.
        if (emergenceCoroutine != null)
        {
            StopCoroutine(emergenceCoroutine);
            emergenceCoroutine = null;
        }


        // -----------------------------------------------------
        // STOP SHADOW
        // -----------------------------------------------------

        if (shadowController != null)
        {
            shadowController.enabled = false;
        }


        // -----------------------------------------------------
        // CAMERA BACK TO PLAYER
        // -----------------------------------------------------

        if (cam != null &&
            playerCameraTarget != null)
        {
            cam.Follow = playerCameraTarget;
            cam.LookAt = playerCameraTarget;
        }


        // -----------------------------------------------------
        // PLAYER BACK TO CONTROL
        // -----------------------------------------------------

        if (playerController != null)
        {
            playerController.enabled = true;
        }


        // -----------------------------------------------------
        // HIDE SHADOW
        // -----------------------------------------------------

        SetShadowVisible(false);
    }


    // =========================================================
    // SHOW / HIDE SHADOW
    // =========================================================

    private void SetShadowVisible(bool visible)
    {
        if (shadowRenderers == null)
            return;


        foreach (Renderer renderer in shadowRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = visible;
            }
        }
    }


    // =========================================================
    // CALCULATE SHADOW POSITION
    // =========================================================

    private Vector3 CalculateShadowPosition()
    {
        // Direction the light travels.
        Vector3 shadowDirection =
            -mainLight.transform.forward;


        // Start ray at player.
        Ray ray =
            new Ray(
                player.position,
                shadowDirection
            );


        // -----------------------------------------------------
        // REAL GROUND
        // -----------------------------------------------------

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            1000f,
            groundLayer))
        {
            return hit.point +
                   Vector3.up * groundOffset;
        }


        // -----------------------------------------------------
        // FALLBACK
        // -----------------------------------------------------

        Plane groundPlane =
            new Plane(
                Vector3.up,
                new Vector3(
                    0f,
                    player.position.y,
                    0f
                )
            );


        if (groundPlane.Raycast(
            ray,
            out float distance))
        {
            return ray.GetPoint(distance) +
                   Vector3.up * groundOffset;
        }


        return Vector3.zero;
    }


    // =========================================================
    // SHADOW EMERGENCE
    // =========================================================

    private IEnumerator ShadowEmergence(
        Vector3 groundPosition)
    {
        Vector3 startPosition =
            groundPosition -
            Vector3.up * emergenceHeight;


        Vector3 endPosition =
            groundPosition;


        shadow.position =
            startPosition;


        // Shadow starts lying down.
        Quaternion startRotation =
            Quaternion.Euler(
                90f,
                player.eulerAngles.y,
                0f
            );


        // Shadow ends standing.
        Quaternion endRotation =
            Quaternion.Euler(
                0f,
                player.eulerAngles.y,
                0f
            );


        shadow.rotation =
            startRotation;


        float time = 0f;


        while (time < emergenceDuration)
        {
            time += Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    time / emergenceDuration
                );


            float curvedT =
                emergenceCurve.Evaluate(t);


            shadow.position =
                Vector3.Lerp(
                    startPosition,
                    endPosition,
                    curvedT
                );


            shadow.rotation =
                Quaternion.Slerp(
                    startRotation,
                    endRotation,
                    curvedT
                );


            yield return null;
        }


        // Final position.
        shadow.position =
            endPosition;


        shadow.rotation =
            endRotation;


        emergenceCoroutine = null;


        // Now shadow can move.
        if (shadowControlled &&
            shadowController != null)
        {
            shadowController.enabled = true;
        }
    }


    // =========================================================
    // DEBUG
    // =========================================================

    private void OnDrawGizmos()
    {
        if (player == null ||
            mainLight == null)
            return;


        Vector3 direction =
            -mainLight.transform.forward;


        Gizmos.DrawRay(
            player.position,
            direction * 5f
        );


        Vector3 shadowPosition =
            CalculateShadowPosition();


        if (shadowPosition != Vector3.zero)
        {
            Gizmos.DrawSphere(
                shadowPosition,
                0.15f
            );


            Gizmos.DrawLine(
                player.position,
                shadowPosition
            );
        }
    }
}
