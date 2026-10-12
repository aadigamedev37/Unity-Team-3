using UnityEngine;

public interface IShadowController
{
    void Activate();
    void Deactivate();
    void Stop();
    void Reset();
    void HandleMovement(Vector2 input);
}