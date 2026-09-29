using UnityEngine;

public class IntegrationTestPlayer : MonoBehaviour, IPlayerController
{
    public void Freeze()
    {
        Debug.Log("TEST PLAYER: Freeze");
    }

    public void Unfreeze()
    {
        Debug.Log("TEST PLAYER: Unfreeze");
    }

    public void Reset()
    {
        Debug.Log("TEST PLAYER: Reset");
    }
}