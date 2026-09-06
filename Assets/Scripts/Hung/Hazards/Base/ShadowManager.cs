using System.Collections;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShadowManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public ThirdPersonController playerController;
    private Animator playerAnimator;
    

    [Header("Shadow")]
    public GameObject shadow;
    public Transform shadowTransform;
    public ThirdPersonController shadowController;



    [Header("Shadow Skeleton")]
    public Transform shadowSkeleton;

    [Header("Camera")]
    public CinemachineCamera camera;
    public Transform playerCameraTarget;
    public Transform shadowCameraTarget;

    [Header("Settings")]
    public float shadowDistance = 2f;
    public float spawnTime = 1f;
    public float rotations = 2f;

    [Header("Input")]
    public InputActionReference toggleAction;

    public bool shadowMode = false;
    private Coroutine spawnCoroutine;

    private Quaternion skeletonStartRotation;
  
    //private StarterAssetsInputs playerInputs;
    //private StarterAssetsInputs shadowInputs;

    [Header("Shadow Light")]
    public Light shadowLight;
 
    public LayerMask groundLayer;
    public Transform playerHead;
    private void Start()
    {
        shadowMode = false;

        if (shadow != null)
            shadow.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;

        

        if (shadowSkeleton != null)
            skeletonStartRotation = shadowSkeleton.localRotation;
        playerAnimator = player.GetComponent<Animator>();

        //playerInputs = player.GetComponent<StarterAssetsInputs>();
        //shadowInputs = shadow.GetComponent<StarterAssetsInputs>();
        playerAnimator.speed = 1f;
        SetPlayerCamera();
    }

    private void OnEnable()
    {
        if (toggleAction != null)
            toggleAction.action.Enable();
    }

    private void OnDisable()
    {
        if (toggleAction != null)
            toggleAction.action.Disable();
    }

    private void Update()
    {
        if (toggleAction == null)
            return;

        if (toggleAction.action.WasPressedThisFrame())
        {
            if (shadowMode)
                ExitShadow();
            else
                SpawnShadow();
        }
        //shadowLightObject.transform.position = GetShadowHeadPosition() ;
    }

    private void SpawnShadow()
    {
        
        //ClearPlayerInput();
        player.GetComponent<PlayerInput>().enabled = false;
        shadow.GetComponent<PlayerInput>().enabled = true;
        shadowMode = true;
        // Stop player
        if (playerController != null)
            playerController.enabled = false;

        if (playerAnimator != null)
            playerAnimator.SetBool("Isdetach", true);

        Vector3 position =
     player.position -
     player.forward * shadowDistance;

        position.y = player.position.y;

        shadowTransform.position = position;

        // Make shadow face same direction as player
        shadowTransform.rotation = player.rotation;

        // Reset skeleton rotation
        if (shadowSkeleton != null)
            shadowSkeleton.localRotation = skeletonStartRotation;

        // Show shadow
        shadow.SetActive(true);

        // Shadow cannot move during animation
        if (shadowController != null)
            shadowController.enabled = false;

        

        // Start rolling animation
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(ShadowSpawnAnimation());
        SetShadowCamera();
    }

    private IEnumerator ShadowSpawnAnimation()
    {
        float time = 0f;

        // Final position
        Vector3 finalPosition = shadowTransform.position;

        // Start below ground
        Vector3 startPosition = finalPosition;
        startPosition.y -= 1.8f;

        shadowSkeleton.position = startPosition;

        while (time < spawnTime)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / spawnTime);

            // Smooth movement upward
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            // Move shadow from underground to final position
            shadowSkeleton.position = Vector3.Lerp(
                startPosition,
                finalPosition,
                smooth
            );

            // Roll skeleton around Y
            float yRotation = Mathf.Lerp(
                rotations * 360f,
                0f,
                smooth
            );

            if (shadowSkeleton != null)
            {
                shadowSkeleton.localRotation =
                    skeletonStartRotation *
                    Quaternion.Euler(0f, yRotation, 0f);
            }

            yield return null;
        }

        // Make sure everything finishes exactly
        shadowTransform.position = finalPosition;

        if (shadowSkeleton != null)
            shadowSkeleton.localRotation = skeletonStartRotation;

        spawnCoroutine = null;

        // Shadow can move now
        if (shadowMode && shadowController != null)
            shadowController.enabled = true;
    }

    public void ExitShadow()
    {
        shadowMode = false;
        playerAnimator.SetBool("Isdetach", false);
        //ClearShadowInput();
        shadow.GetComponent<PlayerInput>().enabled = false;
        player.GetComponent<PlayerInput>().enabled = true;
        
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        // Stop shadow
        //if (shadowController != null)
        //    shadowController.enabled = false;

        // Reset skeleton
        if (shadowSkeleton != null)
            shadowSkeleton.localRotation = skeletonStartRotation;

        // Hide shadow
        if (shadow != null)
            shadow.SetActive(false);

        // Start player again
        if (playerAnimator != null)
            playerAnimator.speed = 1f;

        if (playerController != null)
            playerController.enabled = true;

        SetPlayerCamera();
    }

    private void SetPlayerCamera()
    {
        if (camera == null || playerCameraTarget == null)
            return;

        camera.Follow = playerCameraTarget;
        camera.LookAt = playerCameraTarget;
    }

    private void SetShadowCamera()
    {
        if (camera == null || shadowCameraTarget == null)
            return;

        //shadowCameraTarget.position = shadowTransform.position;

        camera.Follow = shadowCameraTarget;
        camera.LookAt = shadowCameraTarget;
    }

    private Vector3 GetShadowHeadPosition()
    {
        if (playerHead == null || shadowLight == null)
            return player.position;

        Vector3 shadowDirection =
            -shadowLight.transform.forward;

        Ray ray = new Ray(
            playerHead.position,
            shadowDirection
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            100f,
            groundLayer))
        {
            return hit.point;
        }

        return player.position + Vector3.one;
    }

}