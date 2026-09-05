using UnityEngine;

public class HazardDetector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public LayerMask hazardLayer;
    public UIShadowManager uiShadowManager;

    private void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (IsHazardLayer(other))
        {
            Debug.Log("Hazard    ENTER: " + other.gameObject.name);
            uiShadowManager.Hazard.text = "Hazard: " + other.gameObject.name;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsHazardLayer(other))
        {
            Debug.Log("Hazard LEAVE: " + other.gameObject.name);
            uiShadowManager.Hazard.text = "Hazard: None";
        }
    }

    private bool IsHazardLayer(Collider other)
    {
        return (hazardLayer.value & (1 << other.gameObject.layer)) != 0;
    }
}
