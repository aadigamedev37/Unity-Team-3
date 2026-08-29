using UnityEngine;

public interface IPlayerAgent
{
    void Freeze();
    void Unfreeze();
    void ResetToSpawn(Vector3 position, Quaternion rotation);
}