
using UnityEngine;
using System.Collections;

public class StealableMover : StealableBase
{
    [Header("Movement References")]
    [SerializeField] private Transform objectToMove;
    [SerializeField] private Transform destination;

    [Header("Movement Settings")]
    [SerializeField] private float moveDuration = 2f;
    [SerializeField]
    private AnimationCurve movementCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Ghost Settings")]
    [SerializeField] private Collider objectCollider;
    [SerializeField] private MeshRenderer objectRenderer;
    [SerializeField]
    private Color ghostColor =
        new Color(0.3f, 0.7f, 1f, 0.35f);

    private Vector3 originalPosition;
    private Vector3 destinationPosition;

    private Color originalColor;
    private bool hasOriginalColor;

    private bool hasMoved;
    private bool isMoving;

    private Coroutine moveCoroutine;

    private void Awake()
    {
        if (objectToMove == null)
            objectToMove = transform;

        if (objectCollider == null)
            objectCollider = GetComponent<Collider>();

        if (objectRenderer == null)
            objectRenderer = GetComponent<MeshRenderer>();

        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
            hasOriginalColor = true;
        }

        if (destination == null)
        {
            Debug.LogError(
                "StealableMover: Destination is NOT assigned!", this);
            return;
        }

        originalPosition = objectToMove.position;
        destinationPosition = destination.position;

        ApplyCurrentState();
    }

    protected override StealableState GetStateAfterSteal()
    {
        return StealableState.Ghost;
    }

    protected override void OnStateChanged(StealableState newState)
    {
        if (IsGhost)
        {
            if (moveCoroutine == null && !hasMoved)
            {
                moveCoroutine = StartCoroutine(
                    MoveObject(destinationPosition, destination.rotation.eulerAngles));
            }
        }
        else
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }

            if (hasMoved)
            {
                moveCoroutine = StartCoroutine(
                    MoveObject(originalPosition, objectToMove.rotation.eulerAngles));
            }
        }

        ApplyCurrentState();

        Debug.Log("Mover State Changed To: " + newState);
    }

    private void ApplyCurrentState()
    {
        if (objectCollider != null)
            objectCollider.enabled = !IsGhost;

        if (objectRenderer != null)
        {
            objectRenderer.shadowCastingMode = IsGhost
                ? UnityEngine.Rendering.ShadowCastingMode.Off
                : UnityEngine.Rendering.ShadowCastingMode.On;

            if (hasOriginalColor)
            {
                objectRenderer.material.color = IsGhost
                    ? ghostColor
                    : originalColor;
            }
        }
    }

    private IEnumerator MoveObject(Vector3 targetPosition, Vector3 targetRotation)
    {
        isMoving = true;

        Vector3 startPosition = objectToMove.position;
        Quaternion startRotation = objectToMove.rotation;
        Quaternion endRotation = Quaternion.Euler(targetRotation);
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / moveDuration);
            float smoothT = movementCurve.Evaluate(t);

            objectToMove.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothT
            );
            objectToMove.rotation = Quaternion.Slerp(
                startRotation,
                endRotation,
                smoothT
            );


            yield return null;
        }

        objectToMove.position = targetPosition;

        hasMoved = !hasMoved;
        isMoving = false;
        moveCoroutine = null;
    }
}
