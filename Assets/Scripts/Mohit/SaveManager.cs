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
        public static SaveManager Instance { get; private set; }

        public const int TotalLevels = 15;

        private SaveData _data;
        private string SavePath => Path.Combine(Application.persistentDataPath, "savedata.json");

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadData();
        }

        public void LoadData()
        {
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
                SaveToDisk();
            }
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
            return _data.unlockedLevels.Contains(levelNumber);
        }

        public bool IsLevelCompleted(int levelNumber)
        {
            return _data.completedLevels.Contains(levelNumber);
        }

        public void CompleteLevel(int levelNumber)
        {
            if (!_data.completedLevels.Contains(levelNumber))
            {
                _data.completedLevels.Add(levelNumber);
            }

            int nextLevel = levelNumber + 1;
            if (nextLevel <= TotalLevels && !_data.unlockedLevels.Contains(nextLevel))
            {
                _data.unlockedLevels.Add(nextLevel);
            }

            SaveToDisk();
        }

        public int GetHighestUnlockedLevel()
        {
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