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

    [Header("Shadow Light")]
    public Light shadowLight;

    public LayerMask groundLayer;
    public Transform playerHead;

    private void Awake()
    {
        ResolveReferences();
    }

    public void ResolveReferences()
    {
        if (player == null)
        {
            var pCtrl = FindAnyObjectByType<PlayerController>(FindObjectsInactive.Include);
            if (pCtrl != null)
            {
                player = pCtrl.transform;
            }
            else
            {
                var tpcList = FindObjectsByType<ThirdPersonController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var t in tpcList)
                {
                    if (t != null && !t.name.ToLower().Contains("shadow") && (t.name.Contains("Player") || t.CompareTag("Player")))
                    {
                        player = t.transform;
                        playerController = t;
                        break;
                    }
                }
                if (player == null)
                {
                    var pObj = GameObject.Find("PlayerArmature") ?? GameObject.Find("Player") ?? GameObject.FindWithTag("Player");
                    if (pObj != null) player = pObj.transform;
                }
            }
        }

        if (playerController == null && player != null)
        {
            playerController = player.GetComponent<ThirdPersonController>();
        }

        if (shadow == null)
        {
            var sObj = GameObject.Find("Shadow") ?? GameObject.Find("AuthoritativeShadow");
            if (sObj != null)
            {
                shadow = sObj;
            }
            else
            {
                var tpcList = FindObjectsByType<ThirdPersonController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var t in tpcList)
                {
                    if (t != null && t.name.ToLower().Contains("shadow"))
                    {
                        shadow = t.gameObject;
                        shadowController = t;
                        break;
                    }
                }
            }
        }

        if (shadow != null)
        {
            if (shadowTransform == null) shadowTransform = shadow.transform;
            if (shadowController == null) shadowController = shadow.GetComponent<ThirdPersonController>();
            if (shadowSkeleton == null)
            {
                shadowSkeleton = shadow.transform.Find("body/Skeleton") ?? shadow.transform.Find("Skeleton");
            }
        }

        if (camera == null)
        {
            camera = FindAnyObjectByType<CinemachineCamera>(FindObjectsInactive.Include);
        }

        if (playerCameraTarget == null && player != null)
        {
            Transform pRoot = player.Find("PlayerCameraRoot") ?? player.Find("CameraRoot") ?? player;
            playerCameraTarget = pRoot;
        }

        if (shadowCameraTarget == null && shadow != null)
        {
            Transform sRoot = shadow.transform.Find("ShadowCameraRoot") ?? shadow.transform.Find("CameraRoot") ?? shadow.transform;
            shadowCameraTarget = sRoot;
        }
    }

    private void Start()
    {
        ResolveReferences();
        shadowMode = false;

        if (shadow != null)
            shadow.SetActive(false);

        if (playerController != null)
            playerController.enabled = true;

        if (shadowSkeleton != null)
            skeletonStartRotation = shadowSkeleton.localRotation;

        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
                playerAnimator.speed = 1f;
        }

        SetPlayerCamera();
    }

    private void OnEnable()
    {
        if (toggleAction != null && toggleAction.action != null)
            toggleAction.action.Enable();
    }

    private void OnDisable()
    {
        if (toggleAction != null && toggleAction.action != null)
            toggleAction.action.Disable();
    }

    private void Update()
    {
        var psSm = FindAnyObjectByType<PlayerShadowStateMachine>(FindObjectsInactive.Include);
        var inSys = FindAnyObjectByType<InputSystem>(FindObjectsInactive.Include);
        if (psSm != null && inSys != null && inSys.isActiveAndEnabled)
        {
            return;
        }

        bool triggered = false;
        if (toggleAction != null && toggleAction.action != null && toggleAction.action.WasPressedThisFrame())
        {
            triggered = true;
        }
        else
        {
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                var kb = UnityEngine.InputSystem.Keyboard.current;
                triggered = kb.eKey.wasPressedThisFrame || kb.qKey.wasPressedThisFrame;
            }
            if (!triggered)
            {
                try
                {
                    triggered = Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q);
                }
                catch { }
            }
        }

        if (triggered)
        {
            if (psSm != null)
            {
                if (psSm.CurrentState == PlayerShadowState.Attached)
                {
                    psSm.Detach();
                }
                else if (psSm.CurrentState == PlayerShadowState.Detached)
                {
                    psSm.Recall();
                }
            }
            else
            {
                if (shadowMode)
                    ExitShadow();
                else
                    SpawnShadow();
            }
        }
    }

    public void SpawnShadow()
    {
        ResolveReferences();
        if (player == null || shadow == null)
            return;

        var pInput = player.GetComponent<PlayerInput>();
        if (pInput != null) pInput.enabled = false;

        var sInput = shadow.GetComponent<PlayerInput>();
        if (sInput != null) sInput.enabled = true;

        shadowMode = true;

        // Stop player
        if (playerController != null)
            playerController.enabled = false;

        var pc = player.GetComponent<PlayerController>() ?? FindAnyObjectByType<PlayerController>();
        if (pc != null) pc.Freeze();

        if (playerAnimator != null)
            playerAnimator.SetBool("Isdetach", true);

        if (shadowTransform != null)
        {
            Vector3 position = player.position - player.forward * shadowDistance;
            position.y = player.position.y;
            shadowTransform.position = position;
            shadowTransform.rotation = player.rotation;
        }

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

        if (shadowSkeleton != null && shadowTransform != null)
            spawnCoroutine = StartCoroutine(ShadowSpawnAnimation());
        else if (shadowController != null)
            shadowController.enabled = true;

        SetShadowCamera();
    }

    private IEnumerator ShadowSpawnAnimation()
    {
        float time = 0f;

        Vector3 finalPosition = shadowTransform.position;
        Vector3 startPosition = finalPosition;
        startPosition.y -= 1.8f;

        shadowSkeleton.position = startPosition;

        while (time < spawnTime)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / spawnTime);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            if (shadowSkeleton != null)
                shadowSkeleton.position = Vector3.Lerp(startPosition, finalPosition, smooth);

            float currentRotation = t * rotations * 360f;

            if (shadowSkeleton != null)
                shadowSkeleton.localRotation = skeletonStartRotation * Quaternion.Euler(currentRotation, 0f, 0f);

            yield return null;
        }

        if (shadowSkeleton != null)
            shadowSkeleton.position = finalPosition;

        if (shadowSkeleton != null)
            shadowSkeleton.localRotation = skeletonStartRotation;

        spawnCoroutine = null;

        if (shadowMode && shadowController != null)
            shadowController.enabled = true;
    }

    public void ExitShadow()
    {
        ResolveReferences();
        shadowMode = false;
        if (playerAnimator != null)
            playerAnimator.SetBool("Isdetach", false);

        if (shadow != null)
        {
            var sInput = shadow.GetComponent<PlayerInput>();
            if (sInput != null) sInput.enabled = false;
        }

        if (player != null)
        {
            var pInput = player.GetComponent<PlayerInput>();
            if (pInput != null) pInput.enabled = true;
        }

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (shadowSkeleton != null)
            shadowSkeleton.localRotation = skeletonStartRotation;

        if (shadow != null)
            shadow.SetActive(false);

        if (playerAnimator != null)
            playerAnimator.speed = 1f;

        if (playerController != null)
            playerController.enabled = true;

        var pc = player != null ? player.GetComponent<PlayerController>() : null;
        if (pc == null) pc = FindAnyObjectByType<PlayerController>();
        if (pc != null) pc.Unfreeze();

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

        if (playerCameraTarget != null)
            shadowCameraTarget.position = playerCameraTarget.position;

        camera.Follow = shadowCameraTarget;
        camera.LookAt = shadowCameraTarget;
    }

    private Vector3 GetShadowHeadPosition()
    {
        if (playerHead == null || shadowLight == null)
            return player != null ? player.position : Vector3.zero;

        Vector3 shadowDirection = -shadowLight.transform.forward;
        Ray ray = new Ray(playerHead.position, shadowDirection);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            return hit.point;
        }

        return player != null ? player.position + Vector3.one : Vector3.zero;
    }
}
