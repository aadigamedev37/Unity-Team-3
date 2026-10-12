using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class StealIntegrationValidationTests
{
    private GameObject rootGo;
    private StealableDoor door;
    private BoxCollider doorCollider;

    [SetUp]
    public void SetUp()
    {
        rootGo = new GameObject("Test_DoorRoot");
        doorCollider = rootGo.AddComponent<BoxCollider>();
        door = rootGo.AddComponent<StealableDoor>();

        // Set serialized doorCollider via reflection since it is private
        var field = typeof(StealableDoor).GetField("doorCollider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(door, doorCollider);
        }
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
    public void Door_InitiallySolid_HasColliderEnabled()
    {
        Assert.IsTrue(door.IsSolid);
        Assert.IsTrue(door.CanSteal());
        Assert.IsTrue(doorCollider.enabled);
    }

    [Test]
    public void Door_WhenStolen_BecomesGhost_AndDisablesCollider()
    {
        door.Steal();

        Assert.IsTrue(door.IsGhost);
        Assert.IsFalse(door.CanSteal());
        Assert.IsFalse(doorCollider.enabled);
    }

    [Test]
    public void Door_WhenRestored_BecomesSolid_AndEnablesCollider()
    {
        door.Steal();
        Assert.IsTrue(door.IsGhost);

        door.Restore();

        Assert.IsTrue(door.IsSolid);
        Assert.IsTrue(door.CanSteal());
        Assert.IsTrue(doorCollider.enabled);
    }
}
