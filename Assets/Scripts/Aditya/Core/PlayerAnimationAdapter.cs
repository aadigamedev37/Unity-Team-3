using UnityEngine;

/// <summary>
/// Drives the humanoid player Animator from CharacterController movement,
/// enabling idle, walk, run, jump, and free-fall animations without modifying existing controllers.
/// </summary>
public class PlayerAnimationAdapter : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null) return;
        }

        float horizontalSpeed = 0f;
        bool isGrounded = true;
        float verticalVel = 0f;

        if (characterController != null)
        {
            Vector3 vel = characterController.velocity;
            verticalVel = vel.y;
            vel.y = 0f;
            horizontalSpeed = vel.magnitude;
            isGrounded = characterController.isGrounded;
        }

        animator.SetFloat("Speed", horizontalSpeed);
        animator.SetFloat("MotionSpeed", 1f);
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("Jump", !isGrounded && verticalVel > 0.5f);
        animator.SetBool("FreeFall", !isGrounded && verticalVel < -0.5f);
    }
}
