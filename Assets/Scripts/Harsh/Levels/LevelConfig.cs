using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Game/Levels/Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField]
    private int levelId;

    [SerializeField]
    private string displayName;

    [SerializeField]
    private string sceneName;

    public int LevelId => levelId;
    public string DisplayName => displayName;
    public string SceneName => sceneName;
}