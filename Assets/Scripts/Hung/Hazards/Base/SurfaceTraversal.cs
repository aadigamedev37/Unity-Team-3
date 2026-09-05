using UnityEngine;

public class SurfaceTraversal : MonoBehaviour
{
    public LayerMask surfaceLayer;
    public UIShadowManager uiShadowManager;

    private void Start()
    {
    
    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsShadowSurface(other))
        {
            Debug.Log("Shadow ENTER: " + other.gameObject.name);
            uiShadowManager.ShadowSurface.text = "Shadow Surface: " + other.gameObject.name;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsShadowSurface(other))
        {
            Debug.Log("Shadow LEAVE: " + other.gameObject.name);
            uiShadowManager.ShadowSurface.text = "Shadow Surface: None";
        }
    }

    private bool IsShadowSurface(Collider other)
    {
        return (surfaceLayer.value & (1 << other.gameObject.layer)) != 0;
    }
}