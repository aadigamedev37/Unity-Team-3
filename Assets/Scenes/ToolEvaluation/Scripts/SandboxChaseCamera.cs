using UnityEngine;

namespace XorinSandbox.Driving
{
    /// <summary>Smooth chase camera for the sandbox car.</summary>
    public class SandboxChaseCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 localOffset = new Vector3(0f, 4.2f, -9f);
        [SerializeField] private float positionLerp = 6f;
        [SerializeField] private float rotationLerp = 5f;
        [SerializeField] private float lookAtHeight = 1.4f;

        public Transform Target => target;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desired = target.TransformPoint(localOffset);
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-positionLerp * Time.deltaTime));

            Vector3 lookPoint = target.position + Vector3.up * lookAtHeight;
            Vector3 dir = lookPoint - transform.position;
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion desiredRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-rotationLerp * Time.deltaTime));
            }
        }

        /// <summary>Snap instantly behind the car (used after respawn/restart).</summary>
        public void SnapToTarget()
        {
            if (target == null)
            {
                return;
            }

            transform.position = target.TransformPoint(localOffset);
            Vector3 dir = target.position + Vector3.up * lookAtHeight - transform.position;
            if (dir.sqrMagnitude > 0.0001f)
            {
                transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }
        }
    }
}
