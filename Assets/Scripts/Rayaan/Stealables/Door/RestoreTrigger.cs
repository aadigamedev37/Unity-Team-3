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

        if (!other.CompareTag("Player"))
            return;

        if (stealableObject == null)
            return;

        Debug.Log("PLAYER EXITED RESTORE TRIGGER");

        stealableObject.Restore();

        Debug.Log("STEALABLE RESTORED TO SOLID");
    }
}