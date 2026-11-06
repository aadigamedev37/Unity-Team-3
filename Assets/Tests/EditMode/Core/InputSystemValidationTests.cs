using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class InputSystemValidationTests
{
    private GameObject rootGo;
    private InputSystem inputSystem;
    private PlayerShadowStateMachine stateMachine;
    private IntegrationTestPlayer player;
    private IntegrationTestShadow shadow;

    [SetUp]
    public void SetUp()
    {
        rootGo = new GameObject("Test_InputRoot");
        stateMachine = rootGo.AddComponent<PlayerShadowStateMachine>();
        inputSystem = rootGo.AddComponent<InputSystem>();

        GameObject playerGo = new GameObject("Test_Player");
        playerGo.transform.SetParent(rootGo.transform);
        player = playerGo.AddComponent<IntegrationTestPlayer>();

        GameObject shadowGo = new GameObject("Test_Shadow");
        shadowGo.transform.SetParent(rootGo.transform);
        shadow = shadowGo.AddComponent<IntegrationTestShadow>();

        inputSystem.SetStateMachine(stateMachine);
        
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
    public void DefaultActiveTarget_IsPlayer()
    {
        Assert.AreEqual(InputTarget.Player, inputSystem.ActiveTarget);
    }

    [Test]
    public void TriggerDetach_WhenAttached_SucceedsAndSwitchesTargetToShadow()
    {
        bool actionInvoked = false;
        InputAction triggeredAction = InputAction.Recall;

        inputSystem.OnAction += (action) =>
        {
            actionInvoked = true;
            triggeredAction = action;
        };

        inputSystem.TriggerDetach();

        Assert.IsTrue(actionInvoked);
        Assert.AreEqual(InputAction.Detach, triggeredAction);
        Assert.AreEqual(InputTarget.Shadow, inputSystem.ActiveTarget);
    }

    [Test]
    public void TriggerRecall_WhenAttached_IsRejected()
    {
        bool actionInvoked = false;

        inputSystem.OnAction += (action) =>
        {
            actionInvoked = true;
        };

        inputSystem.TriggerRecall();

        Assert.IsFalse(actionInvoked);
        Assert.AreEqual(InputTarget.Player, inputSystem.ActiveTarget);
    }

    [Test]
    public void TriggerRecover_WhenTrapped_Succeeds()
    {
        inputSystem.TriggerDetach();
        stateMachine.Trap();

        Assert.AreEqual(InputTarget.Shadow, inputSystem.ActiveTarget);

        bool actionInvoked = false;
        inputSystem.OnAction += (action) =>
        {
            actionInvoked = true;
        };

        inputSystem.TriggerRecover();

        Assert.IsTrue(actionInvoked);
        Assert.AreEqual(PlayerShadowState.Reuniting, stateMachine.CurrentState);
    }

    [Test]
    public void StateReunion_RestoresPlayerActiveTarget()
    {
        inputSystem.TriggerDetach();
        Assert.AreEqual(InputTarget.Shadow, inputSystem.ActiveTarget);

        inputSystem.TriggerRecall();
        Assert.AreEqual(InputTarget.Shadow, inputSystem.ActiveTarget);

        stateMachine.CompleteReunion();
        Assert.AreEqual(InputTarget.Player, inputSystem.ActiveTarget);
    }
}
