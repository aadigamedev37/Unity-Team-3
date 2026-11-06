using UnityEngine;

namespace XorinSandbox.Driving
{
    /// <summary>Ordered checkpoint trigger that reports to the race manager.</summary>
    [RequireComponent(typeof(BoxCollider))]
    public class SandboxCheckpoint : MonoBehaviour
    {
        [SerializeField] private int index;
        [SerializeField] private bool isFinishLine;

        private SandboxRaceManager manager;

        public int Index => index;
        public bool IsFinishLine => isFinishLine;

        public void Configure(int newIndex, bool finishLine, SandboxRaceManager owner)
        {
            index = newIndex;
            isFinishLine = finishLine;
            manager = owner;
        }

        private void Awake()
        {
            var box = GetComponent<BoxCollider>();
            if (box != null)
            {
                box.isTrigger = true;
            }

            if (manager == null)
            {
                manager = FindFirstObjectByType<SandboxRaceManager>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (manager == null)
            {
                return;
            }

            var car = other.GetComponentInParent<SandboxCarController>();
            if (car == null)
            {
                return;
            }

            manager.NotifyCheckpoint(this, car);
        }
    }
}
