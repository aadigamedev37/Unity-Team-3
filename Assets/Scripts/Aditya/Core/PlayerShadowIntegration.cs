using UnityEngine;

public class PlayerShadowIntegration : MonoBehaviour
{
    [SerializeField] private PlayerShadowStateMachine stateMachine;

    private IPlayerController player; 
    private IShadowController shadow;
    

    private void Start()
    {
    Initialize( FindAnyObjectByType<IntegrationTestPlayer>() , FindAnyObjectByType<IntegrationTestShadow>());
    }   

    public void Initialize(IPlayerController playerController, IShadowController shadowController)
    {
        player = playerController;
        shadow = shadowController;
    }


    public bool TryDetach()
    {
        if(!stateMachine.Detach())
        {
            Debug.Log("detach rejected.");
            return false;
        }

        Debug.Log("detach accepted.");
        player.Freeze();
        shadow.Activate();
        
        return true;
    }

    public bool TryRecall()
    {
        if (!stateMachine.Recall())
        {
            Debug.Log ("recall rejected.");
             return false;
        }

        Debug.Log("recall accepted.");

        shadow.Stop();  
         return true;
    }
    
    public bool CompleteReunion()
{
    if (!stateMachine.CompleteReunion())
    {
        Debug.Log("complete reunion rejected.");
        return false;
    }

    Debug.Log("complete reunion accepted.");
    shadow.Deactivate();
    player.Unfreeze();

    return true;
}



}
