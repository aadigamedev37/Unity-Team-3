using System;
using UnityEngine;

/// <summary>
/// Manages the RECOVER logic when the shadow is in the TRAPPED state.
/// Supports configurable recovery taps (default 1, or 3 taps within 2s window per GDD).
/// If time expires without recovery, signals failure.
/// </summary>
public class RecoverHandler : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;

    [Header("Recovery Configuration")]
    [Tooltip("Number of recover taps needed to escape the trap.")]
    [SerializeField] private int requiredTaps = 1;

    [Tooltip("Time limit in seconds to escape the trap before shadow destruction.")]
    [SerializeField] private float timeLimit = 3.5f;

    [Tooltip("Whether to enforce a strict countdown timer while trapped.")]
    [SerializeField] private bool useTimeLimit = true;

    private int currentTaps = 0;
    private float timer = 0f;
    private bool isTrapped = false;

    public event Action OnRecoverySuccessful;
    public event Action OnRecoveryFailed;

    public int RemainingTaps => Mathf.Max(0, requiredTaps - currentTaps);
    public float TimeRemaining => Mathf.Max(0f, timeLimit - timer);

    private void Awake()
    {
        if (stateMachine == null)
        {
            stateMachine = GetComponent<PlayerShadowStateMachine>();
        }
    }

    private void OnEnable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }
    }

    private void Update()
    {
        if (!isTrapped || !useTimeLimit)
            return;

        timer += Time.deltaTime;

        if (timer >= timeLimit)
        {
            HandleTimeExpired();
        }
    }

    private void HandleStateChanged(PlayerShadowState newState)
    {
        if (newState == PlayerShadowState.Trapped)
        {
            isTrapped = true;
            currentTaps = 0;
            timer = 0f;
            Debug.Log($"RecoverHandler: Trapped! Needs {requiredTaps} tap(s). Time limit: {timeLimit}s.");
        }
        else
        {
            isTrapped = false;
            currentTaps = 0;
            timer = 0f;
        }
    }

    public void SetStateMachine(PlayerShadowStateMachine sm)
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }

        stateMachine = sm;

        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
            stateMachine.OnStateChanged += HandleStateChanged;
            if (stateMachine.CurrentState == PlayerShadowState.Trapped)
            {
                isTrapped = true;
            }
        }
    }

    /// <summary>
    /// Processes a single recover tap attempt.
    /// Can be wired directly to InputSystem action or button press.
    /// </summary>
    public bool ProcessRecoverTap()
    {
        if (stateMachine == null)
        {
            stateMachine = GetComponent<PlayerShadowStateMachine>() ?? FindAnyObjectByType<PlayerShadowStateMachine>();
        }

        if (!isTrapped)
        {
            if (stateMachine != null && stateMachine.CurrentState == PlayerShadowState.Trapped)
            {
                isTrapped = true;
            }
            else
            {
                return false;
            }
        }

        currentTaps++;
        Debug.Log($"RecoverHandler: Tap registered ({currentTaps}/{requiredTaps}).");

        if (currentTaps >= requiredTaps)
        {
            isTrapped = false;
            OnRecoverySuccessful?.Invoke();

            if (stateMachine != null)
            {
                return stateMachine.Recover();
            }

            return true;
        }

        return false;
    }

    private void HandleTimeExpired()
    {
        isTrapped = false;
        Debug.LogWarning("RecoverHandler: Trap time expired! Recovery failed.");
        OnRecoveryFailed?.Invoke();
    }
}
