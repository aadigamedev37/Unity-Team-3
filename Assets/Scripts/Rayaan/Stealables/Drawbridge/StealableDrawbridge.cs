using System.Collections;
using UnityEngine;

/// <summary>
/// A drawbridge obstacle that lowers into a walkable crossing when its shadow is stolen.
/// Extends StealableBase following Rayaan's interaction framework.
/// </summary>
public class StealableDrawbridge : StealableBase
{
    [Header("Bridge References")]
    [SerializeField] private Collider walkableCollider;
    [SerializeField] private Collider blockingCollider;
    [SerializeField] private Transform bridgeArm;

    [Header("Rotation Configuration")]
    [SerializeField] private Vector3 raisedEuler = new Vector3(-60f, 0f, 0f);
    [SerializeField] private Vector3 loweredEuler = Vector3.zero;
    [SerializeField] private float transitionDuration = 0.8f;

    private Coroutine transitionCoroutine;

    private void Awake()
    {
        if (bridgeArm == null)
        {
            Transform arm = transform.Find("BridgeArm") ?? transform.Find("Visual");
            bridgeArm = arm != null ? arm : transform;
        }

        if (walkableCollider == null)
        {
            walkableCollider = GetComponent<Collider>();
        }

        ApplyCurrentState(false);
    }

    protected override StealableState GetStateAfterSteal()
    {
        // When stolen, the bridge drops down (becomes usable/walkable)
        return StealableState.Ghost;
    }

    protected override void OnStateChanged(StealableState newState)
    {
        ApplyCurrentState(true);
        Debug.Log("StealableDrawbridge: State changed to " + newState);
    }

    private void ApplyCurrentState(bool animated)
    {
        bool isLowered = IsGhost;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        if (animated && Application.isPlaying && gameObject.activeInHierarchy)
        {
            transitionCoroutine = StartCoroutine(AnimateTransition(isLowered));
        }
        else
        {
            SetImmediateState(isLowered);
        }
    }

    private void SetImmediateState(bool isLowered)
    {
        if (walkableCollider != null)
        {
            walkableCollider.enabled = isLowered;
        }

        if (blockingCollider != null)
        {
            blockingCollider.enabled = !isLowered;
        }

        if (bridgeArm != null)
        {
            bridgeArm.localEulerAngles = isLowered ? loweredEuler : raisedEuler;
        }
    }

    private IEnumerator AnimateTransition(bool toLowered)
    {
        Vector3 startEuler = bridgeArm != null ? bridgeArm.localEulerAngles : (toLowered ? raisedEuler : loweredEuler);
        Vector3 targetEuler = toLowered ? loweredEuler : raisedEuler;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            if (bridgeArm != null)
            {
                bridgeArm.localEulerAngles = new Vector3(
                    Mathf.LerpAngle(startEuler.x, targetEuler.x, smooth),
                    Mathf.LerpAngle(startEuler.y, targetEuler.y, smooth),
                    Mathf.LerpAngle(startEuler.z, targetEuler.z, smooth)
                );
            }

            yield return null;
        }

        SetImmediateState(toLowered);
        transitionCoroutine = null;
    }
}
