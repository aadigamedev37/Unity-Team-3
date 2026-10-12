using System;
using UnityEngine;

/// <summary>
/// Central hook provider for Audio and Visual feedback across core gameplay transitions.
/// Fires events and triggers assigned audio clips/particle systems safely without throwing
/// when assets are unassigned.
/// </summary>
public class FeedbackHooks : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerShadowStateMachine stateMachine;
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private StealInteractor stealInteractor;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip detachSFX;
    [SerializeField] private AudioClip recallSFX;
    [SerializeField] private AudioClip stealSFX;
    [SerializeField] private AudioClip trapSFX;
    [SerializeField] private AudioClip recoverSFX;

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem detachVFX;
    [SerializeField] private ParticleSystem recallVFX;
    [SerializeField] private ParticleSystem stealVFX;
    [SerializeField] private ParticleSystem trapVFX;
    [SerializeField] private ParticleSystem recoverVFX;

    public event Action OnDetachTriggered;
    public event Action OnRecallTriggered;
    public event Action OnStealTriggered;
    public event Action OnTrapTriggered;
    public event Action OnRecoverTriggered;

    private void Awake()
    {
        if (stateMachine == null)
            stateMachine = FindAnyObjectByType<PlayerShadowStateMachine>();

        if (inputSystem == null)
            inputSystem = FindAnyObjectByType<InputSystem>();

        if (stealInteractor == null)
            stealInteractor = FindAnyObjectByType<StealInteractor>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged += HandleStateChanged;
        }

        if (inputSystem != null)
        {
            inputSystem.OnAction += HandleAction;
        }

        if (stealInteractor != null)
        {
            stealInteractor.OnStealExecuted += HandleStealExecuted;
        }
    }

    private void OnDisable()
    {
        if (stateMachine != null)
        {
            stateMachine.OnStateChanged -= HandleStateChanged;
        }

        if (inputSystem != null)
        {
            inputSystem.OnAction -= HandleAction;
        }

        if (stealInteractor != null)
        {
            stealInteractor.OnStealExecuted -= HandleStealExecuted;
        }
    }

    private void HandleStateChanged(PlayerShadowState newState)
    {
        switch (newState)
        {
            case PlayerShadowState.Detached:
                TriggerDetachFeedback();
                break;

            case PlayerShadowState.Reuniting:
                TriggerRecallFeedback();
                break;

            case PlayerShadowState.Trapped:
                TriggerTrapFeedback();
                break;

            case PlayerShadowState.Attached:
                // Reunited
                break;
        }
    }

    private void HandleAction(InputAction action)
    {
        if (action == InputAction.Recover)
        {
            TriggerRecoverFeedback();
        }
    }

    private void HandleStealExecuted(StealableBase stolen)
    {
        TriggerStealFeedback();
    }

    public void TriggerDetachFeedback()
    {
        PlaySound(detachSFX);
        PlayVFX(detachVFX);
        OnDetachTriggered?.Invoke();
        Debug.Log("FeedbackHooks: Detach feedback executed.");
    }

    public void TriggerRecallFeedback()
    {
        PlaySound(recallSFX);
        PlayVFX(recallVFX);
        OnRecallTriggered?.Invoke();
        Debug.Log("FeedbackHooks: Recall feedback executed.");
    }

    public void TriggerStealFeedback()
    {
        PlaySound(stealSFX);
        PlayVFX(stealVFX);
        OnStealTriggered?.Invoke();
        Debug.Log("FeedbackHooks: Steal feedback executed.");
    }

    public void TriggerTrapFeedback()
    {
        PlaySound(trapSFX);
        PlayVFX(trapVFX);
        OnTrapTriggered?.Invoke();
        Debug.Log("FeedbackHooks: Trap feedback executed.");
    }

    public void TriggerRecoverFeedback()
    {
        PlaySound(recoverSFX);
        PlayVFX(recoverVFX);
        OnRecoverTriggered?.Invoke();
        Debug.Log("FeedbackHooks: Recover feedback executed.");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlayVFX(ParticleSystem ps)
    {
        if (ps != null)
        {
            ps.Play();
        }
    }
}
