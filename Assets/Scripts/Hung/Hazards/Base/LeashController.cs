using UnityEngine;

public class LeashController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform shadow;

    [Header("Leash Settings")]
    [SerializeField] private float maxDistance = 25f;

    [Header("Leash Visual")]
    [SerializeField] private LineRenderer leashLine;
    [SerializeField] private float lineWidth = 0.05f;
    public UIShadowManager uiShadowManager;
    private void Awake()
    {
        SetupLeashLine();
    }

    private void LateUpdate()
    {
        if (player == null || shadow == null)
            return;
        // Hide leash when Shadow is inactive.
        if (!shadow.gameObject.activeInHierarchy)
        {
            if (leashLine != null)
                leashLine.enabled = false;

            return;
        }

        // Shadow is active, so show leash.
        if (leashLine != null)
            leashLine.enabled = true;
        EnforceShadowLeash();
        UpdateLeashVisual();
    }

    private void EnforceShadowLeash()
    {
        Vector3 playerPosition = player.position;
        Vector3 shadowPosition = shadow.position;

        // Direction from Player to Shadow.
        Vector3 playerToShadow =
            shadowPosition - playerPosition;
        
        float distance = playerToShadow.magnitude;
        uiShadowManager.limit.text = distance.ToString("F1") + "m";
        if (distance >= maxDistance)
        {
            uiShadowManager.limit.text = "Limit Reached";
        }
        // Shadow is inside the leash.
        if (distance <= maxDistance)
            return;

        // Safety check.
        if (distance < 0.001f)
            return;
        

        
        // Direction from Player toward Shadow.
        Vector3 directionFromPlayer =
            playerToShadow.normalized;

        // Put Shadow exactly on the 25m boundary.
        Vector3 constrainedPosition =
            playerPosition +
            directionFromPlayer * maxDistance;

        shadow.position = constrainedPosition;
    }

    private void UpdateLeashVisual()
    {
        if (leashLine == null)
            return;

        leashLine.positionCount = 2;

        leashLine.SetPosition(0, player.position);
        leashLine.SetPosition(1, shadow.position);
    }

    private void SetupLeashLine()
    {
        if (leashLine == null)
            return;

        leashLine.positionCount = 2;

        leashLine.startWidth = lineWidth;
        leashLine.endWidth = lineWidth;

        leashLine.useWorldSpace = true;

        Material material = leashLine.material;

        if (material != null)
        {
            material.color = Color.black;
        }
    }
}