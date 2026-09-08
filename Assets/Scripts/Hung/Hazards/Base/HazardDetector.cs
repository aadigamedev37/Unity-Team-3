using UnityEngine;

public class HazardDetector : MonoBehaviour
{
    [Header("Hazard Detection")]
    public Transform surfaceProbe;
    public LayerMask hardLayer;

    [Tooltip("How far ahead the Shadow searches.")]
    public float detectionDistance = 2f;

    [Tooltip("Radius of the detection sphere.")]
    public float detectionRadius = 0.5f;

    [Tooltip("Maximum allowed surface slope.")]
    [Range(0f, 180f)]
    public float maxSlopeAngle = 60f;

    [Header("Detection")]
    public bool detectForward = true;
    public bool detectGround = true;

    [Header("Debug")]
    public bool drawDebug = true;

    public bool HasValidHazard { get; private set; }

    public Vector3 CurrentHazardPoint { get; private set; }

    public Vector3 CurrentHazardNormal { get; private set; }

    public Vector3 LastValidHazardPoint { get; private set; }

    public Vector3 LastValidHazardNormal { get; private set; }

    private GameObject lastDetectedHazard;
    public UIShadowManager uiShadowManager;

    private void Update()
    {
        DetectHazard();
    }


    private void DetectHazard()
    {
        if (surfaceProbe == null)
            return;

        bool detected = false;

        /*
         * FIRST:
         * Detect hazards in front of the Shadow.
         */
        if (detectForward)
        {
            Vector3 origin = surfaceProbe.position;

            Vector3 direction = transform.forward;

            if (drawDebug)
            {
                Debug.DrawRay(
                    origin,
                    direction * detectionDistance,
                    Color.red
                );
            }

            if (Physics.SphereCast(
                origin,
                detectionRadius,
                direction,
                out RaycastHit hit,
                detectionDistance,
                hardLayer))
            {
                ProcessHazard(hit);

                detected = true;
            }
        }


        /*
         * SECOND:
         * Detect ground underneath the Shadow.
         */
        if (!detected && detectGround)
        {
            Vector3 origin = surfaceProbe.position;

            Vector3 direction = Vector3.down;

            if (drawDebug)
            {
                Debug.DrawRay(
                    origin,
                    direction * detectionDistance,
                    Color.blue
                );
            }

            if (Physics.SphereCast(
                origin,
                detectionRadius,
                direction,
                out RaycastHit hit,
                detectionDistance,
                hardLayer))
            {
                ProcessHazard(hit);

                detected = true;
            }
        }


        if (!detected)
        {
            HasValidHazard = false;

            lastDetectedHazard = null;
        }
    }


    private void ProcessHazard(RaycastHit hit)
    {
        GameObject detectedHazard = hit.collider.gameObject;


        /*
         * Log only when the Shadow
         * detects a different object.
         */
        if (detectedHazard != lastDetectedHazard)
        {
            lastDetectedHazard = detectedHazard;

            Debug.Log(
                "Shadow detected nearby hazard: " +
                detectedHazard.name
            );
            uiShadowManager.Hazard.text = "Hazard: " + detectedHazard.name;
        }


        /*
         * Check the hazard angle.
         */
        float slopeAngle = Vector3.Angle(
            hit.normal,
            Vector3.up
        );


        if (slopeAngle <= maxSlopeAngle)
        {
            HasValidHazard = true;

            CurrentHazardPoint = hit.point;

            CurrentHazardNormal = hit.normal;


            /*
             * Remember the last valid surface.
             */
            LastValidHazardPoint = hit.point;

            LastValidHazardNormal = hit.normal;


            if (drawDebug)
            {
                Debug.DrawLine(
                    surfaceProbe.position,
                    hit.point,
                    Color.green
                );
            }
        }
        else
        {
            HasValidHazard = false;

            if (drawDebug)
            {
                Debug.DrawLine(
                    surfaceProbe.position,
                    hit.point,
                    Color.yellow
                );
            }
        }
    }
}
