using UnityEngine;

public class SurfaceTraversal : MonoBehaviour
{
    [Header("Surface Detection")]
    public Transform surfaceProbe;
    public LayerMask surfaceLayer;

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

    public bool HasValidSurface { get; private set; }

    public Vector3 CurrentSurfacePoint { get; private set; }

    public Vector3 CurrentSurfaceNormal { get; private set; }

    public Vector3 LastValidSurfacePoint { get; private set; }

    public Vector3 LastValidSurfaceNormal { get; private set; }

    private GameObject lastDetectedSurface;
    public UIShadowManager uiShadowManager;

    private void Update()
    {
        DetectSurface();
    }


    private void DetectSurface()
    {
        if (surfaceProbe == null)
            return;

        bool detected = false;

        /*
         * FIRST:
         * Detect surfaces in front of the Shadow.
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
                surfaceLayer))
            {
                ProcessSurface(hit);

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
                surfaceLayer))
            {
                ProcessSurface(hit);

                detected = true;
            }
        }


        if (!detected)
        {
            HasValidSurface = false;

            lastDetectedSurface = null;
        }
    }


    private void ProcessSurface(RaycastHit hit)
    {
        GameObject detectedSurface = hit.collider.gameObject;


        /*
         * Log only when the Shadow
         * detects a different object.
         */
        if (detectedSurface != lastDetectedSurface)
        {
            lastDetectedSurface = detectedSurface;

            Debug.Log(
                "Shadow detected nearby surface: " +
                detectedSurface.name
            );
            uiShadowManager.ShadowSurface.text = "Surface: " + detectedSurface.name;
        }


        /*
         * Check the surface angle.
         */
        float slopeAngle = Vector3.Angle(
            hit.normal,
            Vector3.up
        );


        if (slopeAngle <= maxSlopeAngle)
        {
            HasValidSurface = true;

            CurrentSurfacePoint = hit.point;

            CurrentSurfaceNormal = hit.normal;


            /*
             * Remember the last valid surface.
             */
            LastValidSurfacePoint = hit.point;

            LastValidSurfaceNormal = hit.normal;


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
            HasValidSurface = false;

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