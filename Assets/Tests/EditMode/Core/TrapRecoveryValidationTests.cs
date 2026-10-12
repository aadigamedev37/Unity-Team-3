using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TrapRecoveryValidationTests
{
    private GameObject rootGo;
    private PlayerShadowStateMachine stateMachine;
    private GameplayEventIntegration eventIntegration;
    private TrapLight trapLight;
    private RecoverHandler recoverHandler;
    private IntegrationTestShadow shadow;

    [SetUp]
    public void SetUp()
    {
        rootGo = new GameObject("Test_TrapRoot");
        stateMachine = rootGo.AddComponent<PlayerShadowStateMachine>();
        eventIntegration = rootGo.AddComponent<GameplayEventIntegration>();
        recoverHandler = rootGo.AddComponent<RecoverHandler>();

        GameObject trapGo = new GameObject("Test_TrapLight");
        trapGo.transform.SetParent(rootGo.transform);
        trapLight = trapGo.AddComponent<TrapLight>();

        GameObject shadowGo = new GameObject("Test_Shadow");
        shadowGo.transform.SetParent(rootGo.transform);
        shadow = shadowGo.AddComponent<IntegrationTestShadow>();

        eventIntegration.RegisterTrapLight(trapLight);
    }

    [TearDown]
    public void TearDown()
    {
        if (rootGo != null)
        {
            Object.DestroyImmediate(rootGo);
        }
    }

    [Test]
    public void TrapEvent_WhenDetached_EntersTrappedState()
    {
        stateMachine.Detach();
        Assert.AreEqual(PlayerShadowState.Detached, stateMachine.CurrentState);

        eventIntegration.HandleShadowTrapped();

        Assert.AreEqual(PlayerShadowState.Trapped, stateMachine.CurrentState);
    }

    [Test]
    public void TrapEvent_WhenAttached_IsSafelyIgnored()
    {
        Assert.AreEqual(PlayerShadowState.Attached, stateMachine.CurrentState);

        eventIntegration.HandleShadowTrapped();

        Assert.AreEqual(PlayerShadowState.Attached, stateMachine.CurrentState);
    }

    [Test]
    public void RecoverHandler_ProcessTap_TransitionsToReuniting()
    {
        stateMachine.Detach();
        stateMachine.Trap();

        Assert.AreEqual(PlayerShadowState.Trapped, stateMachine.CurrentState);

        bool success = recoverHandler.ProcessRecoverTap();

        Assert.IsTrue(success);
        Assert.AreEqual(PlayerShadowState.Reuniting, stateMachine.CurrentState);
    }
}
