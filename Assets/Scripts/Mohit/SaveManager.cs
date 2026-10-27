using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mohit.MetaSystems
{
    [Serializable]
    public class SaveData
    {
        public List<int> unlockedLevels = new List<int> { 1 }; // Level 1 always unlocked
        public List<int> completedLevels = new List<int>();
    }

    public class SaveManager : MonoBehaviour
    {
        private static SaveManager _instance;
        public static SaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<SaveManager>();
                }
                return _instance;
            }
            set => _instance = value;
        }

        public const int TotalLevels = 15;

        public event Action OnGameCompleted;

        public bool IsGameCompleted
        {
            get
            {
                EnsureDataLoaded();
                return _data.completedLevels.Contains(TotalLevels);
            }
        }

        private SaveData _data;
        private string SavePath => Path.Combine(Application.persistentDataPath, "savedata.json");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (Instance == null)
            {
                GameObject smObj = new GameObject("SaveManager");
                smObj.AddComponent<SaveManager>();
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                if (Application.isPlaying)
                    Destroy(gameObject);
                else
                    DestroyImmediate(gameObject);
                return;
            }

            _instance = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }

            LoadData();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public void EnsureDataLoaded()
        {
            if (_data == null)
            {
                LoadData();
            }
        }

        public void LoadData()
        {

            Debug.Log("SaveManager: loading from " + SavePath);
            
            if (File.Exists(SavePath))
            {
                try
                {
                    string json = File.ReadAllText(SavePath);
                    _data = JsonUtility.FromJson<SaveData>(json);
                    if (_data == null) _data = new SaveData();
                }
                catch (Exception e)
                {
                    Debug.LogError($"SaveManager: Failed to load save data, creating new. {e.Message}");
                    _data = new SaveData();
                }
            }
            else
            {
                _data = new SaveData();
            }

            ValidateData();
            SaveToDisk();
        }

        private void ValidateData()
        {
            _data.unlockedLevels = CleanList(_data.unlockedLevels);
            _data.completedLevels = CleanList(_data.completedLevels);

            // Find the highest level the player should have access to
            int highest = 1;
            foreach (int lvl in _data.unlockedLevels)
                if (lvl > highest) highest = lvl;

            foreach (int lvl in _data.completedLevels)
                highest = Mathf.Max(highest, Mathf.Min(lvl + 1, TotalLevels));

            // Levels unlock in order, so everything up to the highest must be unlocked
            _data.unlockedLevels.Clear();
            for (int i = 1; i <= highest; i++)
                _data.unlockedLevels.Add(i);

                Debug.Log($"SaveManager: validated. Unlocked = [{string.Join(",", _data.unlockedLevels)}], Completed = [{string.Join(",", _data.completedLevels)}]");
        }

        private List<int> CleanList(List<int> source)
        {
            List<int> result = new List<int>();
            if (source == null) return result;

            foreach (int lvl in source)
            {
                if (lvl >= 1 && lvl <= TotalLevels && !result.Contains(lvl))
                    result.Add(lvl);
            }

            result.Sort();
            return result;
        }

        private void SaveToDisk()
        {
            try
            {
                string json = JsonUtility.ToJson(_data, true);
                File.WriteAllText(SavePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"SaveManager: Failed to save data. {e.Message}");
            }
        }

        public bool IsLevelUnlocked(int levelNumber)
        {
            EnsureDataLoaded();
            return _data.unlockedLevels.Contains(levelNumber);
        }

        public bool IsLevelCompleted(int levelNumber)
        {
            EnsureDataLoaded();
            return _data.completedLevels.Contains(levelNumber);
        }

        public void CompleteLevel(int levelNumber)
        {
            EnsureDataLoaded();
            if (levelNumber < 1 || levelNumber > TotalLevels)
            {
                Debug.LogWarning($"SaveManager: Ignoring CompleteLevel({levelNumber}), out of range.");
                return;
            }

            if (!IsLevelUnlocked(levelNumber))
            {
                Debug.LogWarning($"SaveManager: Level {levelNumber} completed but was never unlocked. Ignoring.");
                return;
            }

            bool firstTimeCompleting = !_data.completedLevels.Contains(levelNumber);

            if (firstTimeCompleting)
                _data.completedLevels.Add(levelNumber);

            int nextLevel = levelNumber + 1;
            if (nextLevel <= TotalLevels && !_data.unlockedLevels.Contains(nextLevel))
                _data.unlockedLevels.Add(nextLevel);

            SaveToDisk();

            if (levelNumber == TotalLevels && firstTimeCompleting)
            {
                Debug.Log("SaveManager: Final level completed.");
                OnGameCompleted?.Invoke();
            }
        }

        public int GetHighestUnlockedLevel()
        {
            EnsureDataLoaded();
            int highest = 1;
            foreach (int lvl in _data.unlockedLevels)
            {
                if (lvl > highest) highest = lvl;
            }
            return highest;
        }

        // For testing/debug only — resets all progress
        public void ResetProgress()
        {
            _data = new SaveData();
            SaveToDisk();
        }
    }
}