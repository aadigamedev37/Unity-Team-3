using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class FeedbackAndContextualActionValidationTests
{
    private GameObject rootGo;
    private PlayerShadowStateMachine stateMachine;
    private InputSystem inputSystem;
    private ContextualActionButton actionButton;
    private Button uiButton;
    private FeedbackHooks feedbackHooks;

    [SetUp]
    public void SetUp()
    {
        rootGo = new GameObject("Test_FeedbackAndContextRoot");
        stateMachine = rootGo.AddComponent<PlayerShadowStateMachine>();
        inputSystem = rootGo.AddComponent<InputSystem>();
        inputSystem.SetStateMachine(stateMachine);

        GameObject buttonGo = new GameObject("Test_ContextualButton");
        buttonGo.transform.SetParent(rootGo.transform);
        uiButton = buttonGo.AddComponent<Button>();
        actionButton = buttonGo.AddComponent<ContextualActionButton>();

        // Wire serialized dependencies via reflection
        var smField = typeof(ContextualActionButton).GetField("stateMachine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (smField != null) smField.SetValue(actionButton, stateMachine);

        var isField = typeof(ContextualActionButton).GetField("inputSystem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (isField != null) isField.SetValue(actionButton, inputSystem);

        GameObject feedbackGo = new GameObject("Test_FeedbackHooks");
        feedbackGo.transform.SetParent(rootGo.transform);
        feedbackHooks = feedbackGo.AddComponent<FeedbackHooks>();

        var fhSmField = typeof(FeedbackHooks).GetField("stateMachine", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fhSmField != null) fhSmField.SetValue(feedbackHooks, stateMachine);

        var fhIsField = typeof(FeedbackHooks).GetField("inputSystem", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (fhIsField != null) fhIsField.SetValue(feedbackHooks, inputSystem);

        // Force enable / state update
        actionButton.UpdateContextualState();
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
    public void ActionButton_WhenAttached_IsDetach()
    {
        actionButton.UpdateContextualState();
        Assert.AreEqual(InputAction.Detach, actionButton.CurrentAction);
        Assert.IsTrue(uiButton.interactable);
    }

    [Test]
    public void ActionButton_WhenDetached_IsRecall()
    {
        stateMachine.Detach();
        actionButton.UpdateContextualState();

        Assert.AreEqual(InputAction.Recall, actionButton.CurrentAction);
        Assert.IsTrue(uiButton.interactable);
    }

    [Test]
    public void ActionButton_WhenDetachedAndTargetInRange_IsSteal()
    {
        stateMachine.Detach();
        actionButton.SetStealTargetInRange(true);

        Assert.AreEqual(InputAction.Steal, actionButton.CurrentAction);
        Assert.IsTrue(uiButton.interactable);
    }

    [Test]
    public void ActionButton_WhenTrapped_IsRecover()
    {
        stateMachine.Detach();
        stateMachine.Trap();
        actionButton.UpdateContextualState();

        Assert.AreEqual(InputAction.Recover, actionButton.CurrentAction);
        Assert.IsTrue(uiButton.interactable);
    }

    [Test]
    public void ActionButton_WhenReuniting_IsDisabled()
    {
        stateMachine.Detach();
        stateMachine.Recall();
        actionButton.UpdateContextualState();

        Assert.IsFalse(uiButton.interactable);
    }

    [Test]
    public void FeedbackHooks_FiresOnDetach()
    {
        bool fired = false;
        feedbackHooks.OnDetachTriggered += () => fired = true;

        feedbackHooks.TriggerDetachFeedback();

        Assert.IsTrue(fired);
    }

    [Test]
    public void FeedbackHooks_FiresOnRecall()
    {
        bool fired = false;
        feedbackHooks.OnRecallTriggered += () => fired = true;

        feedbackHooks.TriggerRecallFeedback();

        Assert.IsTrue(fired);
    }

    [Test]
    public void FeedbackHooks_FiresOnTrap()
    {
        bool fired = false;
        feedbackHooks.OnTrapTriggered += () => fired = true;

        feedbackHooks.TriggerTrapFeedback();

        Assert.IsTrue(fired);
    }

    [Test]
    public void FeedbackHooks_FiresOnRecover()
    {
        bool fired = false;
        feedbackHooks.OnRecoverTriggered += () => fired = true;

        feedbackHooks.TriggerRecoverFeedback();

        Assert.IsTrue(fired);
    }

    [Test]
    public void FeedbackHooks_NullAudioAndVFX_DoesNotThrow()
    {
        Assert.DoesNotThrow(() =>
        {
            feedbackHooks.TriggerDetachFeedback();
            feedbackHooks.TriggerRecallFeedback();
            feedbackHooks.TriggerStealFeedback();
            feedbackHooks.TriggerTrapFeedback();
            feedbackHooks.TriggerRecoverFeedback();
        });
    }
}
