using UnityEngine;

public class RestoreTrigger : MonoBehaviour
{
    [SerializeField]
    private StealableBase stealableObject;

    private void Awake()
    {
        if (stealableObject == null)
        {
            stealableObject = GetComponentInParent<StealableBase>();
        }

        if (stealableObject == null)
        {
            Debug.LogError(
                "RestoreTrigger: Stealable object is not assigned!",
                gameObject
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("RESTORE TRIGGER ENTER: " + other.name);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("RESTORE TRIGGER EXIT: " + other.name);

        if (!IsPlayer(other))
            return;

        if (stealableObject == null)
            return;

        Debug.Log("PLAYER EXITED RESTORE TRIGGER");

        stealableObject.Restore();

        Debug.Log("STEALABLE RESTORED TO SOLID");
    }

    private bool IsPlayer(Collider other)
    {
        if (other.name.ToLower().Contains("shadow") || other.GetComponentInParent<IShadowController>() != null)
            return false;

        return other.CompareTag("Player") ||
               other.GetComponentInParent<PlayerController>() != null ||
               other.GetComponentInParent<StarterAssets.ThirdPersonController>() != null ||
               other.name.ToLower().Contains("player");
    }
}