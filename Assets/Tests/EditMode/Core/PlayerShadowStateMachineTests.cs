using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class PlayerShadowStateMachineTests
{
    private GameObject testGo;
    private PlayerShadowStateMachine stateMachine;

    [SetUp]
    public void SetUp()
    {
        testGo = new GameObject("Test_StateMachine");
        stateMachine = testGo.AddComponent<PlayerShadowStateMachine>();
    }

    [TearDown]
    public void TearDown()
    {
        if (testGo != null)
        {
            Object.DestroyImmediate(testGo);
        }
    }

    [Test]
    public void InitialState_IsAttached()
    {
        Assert.AreEqual(PlayerShadowState.Attached, stateMachine.CurrentState);
    }

    [Test]
    public void Detach_FromAttached_TransitionsToDetached_AndInvokesEvent()
    {
        PlayerShadowState notifiedState = PlayerShadowState.Attached;
        bool eventFired = false;

        stateMachine.OnStateChanged += (state) =>
        {
            notifiedState = state;
            eventFired = true;
        };

        bool result = stateMachine.Detach();

        Assert.IsTrue(result);
        Assert.AreEqual(PlayerShadowState.Detached, stateMachine.CurrentState);
        Assert.IsTrue(eventFired);
        Assert.AreEqual(PlayerShadowState.Detached, notifiedState);
    }

    [Test]
    public void Detach_FromDetached_IsRejected()
    {
        stateMachine.Detach();
        bool secondDetach = stateMachine.Detach();

        Assert.IsFalse(secondDetach);
        Assert.AreEqual(PlayerShadowState.Detached, stateMachine.CurrentState);
    }

    [Test]
    public void Recall_FromDetached_TransitionsToReuniting()
    {
        stateMachine.Detach();
        bool result = stateMachine.Recall();

        Assert.IsTrue(result);
        Assert.AreEqual(PlayerShadowState.Reuniting, stateMachine.CurrentState);
    }

    [Test]
    public void Recall_FromAttached_IsRejected()
    {
        bool result = stateMachine.Recall();

        Assert.IsFalse(result);
        Assert.AreEqual(PlayerShadowState.Attached, stateMachine.CurrentState);
    }

    [Test]
    public void CompleteReunion_FromReuniting_TransitionsToAttached()
    {
        stateMachine.Detach();
        stateMachine.Recall();

        bool result = stateMachine.CompleteReunion();

        Assert.IsTrue(result);
        Assert.AreEqual(PlayerShadowState.Attached, stateMachine.CurrentState);
    }

    [Test]
    public void CompleteReunion_FromDetached_IsRejected()
    {
        stateMachine.Detach();
        bool result = stateMachine.CompleteReunion();

        Assert.IsFalse(result);
        Assert.AreEqual(PlayerShadowState.Detached, stateMachine.CurrentState);
    }

    [Test]
    public void Trap_FromDetached_TransitionsToTrapped()
    {
        stateMachine.Detach();
        bool result = stateMachine.Trap();

        Assert.IsTrue(result);
        Assert.AreEqual(PlayerShadowState.Trapped, stateMachine.CurrentState);
    }

    [Test]
    public void Trap_FromAttached_IsRejected()
    {
        bool result = stateMachine.Trap();

        Assert.IsFalse(result);
        Assert.AreEqual(PlayerShadowState.Attached, stateMachine.CurrentState);
    }

    [Test]
    public void Trap_FromTrapped_IsRejected()
    {
        stateMachine.Detach();
        stateMachine.Trap();
        bool secondTrap = stateMachine.Trap();

        Assert.IsFalse(secondTrap);
        Assert.AreEqual(PlayerShadowState.Trapped, stateMachine.CurrentState);
    }

    [Test]
    public void Recover_FromTrapped_TransitionsToReuniting()
    {
        stateMachine.Detach();
        stateMachine.Trap();

        bool result = stateMachine.Recover();

        Assert.IsTrue(result);
        Assert.AreEqual(PlayerShadowState.Reuniting, stateMachine.CurrentState);
    }

    [Test]
    public void Recover_FromDetached_IsRejected()
    {
        stateMachine.Detach();
        bool result = stateMachine.Recover();

        Assert.IsFalse(result);
        Assert.AreEqual(PlayerShadowState.Detached, stateMachine.CurrentState);
    }

    [Test]
    public void Steal_AllowedWhenDetached_RejectedWhenAttached()
    {
        Assert.IsFalse(stateMachine.Steal());

        stateMachine.Detach();
        Assert.IsTrue(stateMachine.Steal());
        Assert.AreEqual(PlayerShadowState.Detached, stateMachine.CurrentState);
    }

    [Test]
    public void CanPerformAction_AccurateForEveryState()
    {
        // Attached
        Assert.IsTrue(stateMachine.CanPerformAction(InputAction.Detach));
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Recall));
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Steal));
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Recover));

        // Detached
        stateMachine.Detach();
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Detach));
        Assert.IsTrue(stateMachine.CanPerformAction(InputAction.Recall));
        Assert.IsTrue(stateMachine.CanPerformAction(InputAction.Steal));
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Recover));

        // Trapped
        stateMachine.Trap();
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Detach));
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Recall));
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Steal));
        Assert.IsTrue(stateMachine.CanPerformAction(InputAction.Recover));

        // Reuniting
        stateMachine.Recover();
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Detach));
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Recall));
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Steal));
        Assert.IsFalse(stateMachine.CanPerformAction(InputAction.Recover));
    }

    [Test]
    public void RapidInputSpam_DoesNotCorruptState()
    {
        for (int i = 0; i < 50; i++)
        {
            stateMachine.Detach();
        }

        Assert.AreEqual(PlayerShadowState.Detached, stateMachine.CurrentState);

        for (int i = 0; i < 50; i++)
        {
            stateMachine.Recall();
        }

        Assert.AreEqual(PlayerShadowState.Reuniting, stateMachine.CurrentState);
    }

    [Test]
    public void ResetToAttached_RestoresInitialState()
    {
        stateMachine.Detach();
        stateMachine.Trap();
        Assert.AreEqual(PlayerShadowState.Trapped, stateMachine.CurrentState);

        stateMachine.ResetToAttached();
        Assert.AreEqual(PlayerShadowState.Attached, stateMachine.CurrentState);
    }
}
