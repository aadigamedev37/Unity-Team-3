using UnityEngine;

public class IntegrationTestShadow : MonoBehaviour, IShadowController
{
    public void Activate()
    {
        Debug.Log("TEST SHADOW: Activate");
    }

    public void Deactivate()
    {
        Debug.Log("TEST SHADOW: Deactivate");
    }

    public void Stop()
    {
        Debug.Log("TEST SHADOW: Stop");
    }

    public void Reset()
    {
        Debug.Log("TEST SHADOW: Reset");
    }

    public void HandleMovement(Vector2 input)
    {
        Debug.Log("TEST SHADOW: HandleMovement: " + input);
    }
}