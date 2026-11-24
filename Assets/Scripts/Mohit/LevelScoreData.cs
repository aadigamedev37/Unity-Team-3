using System;
using System.Collections.Generic;

namespace Mohit.MetaSystems
{
    [Serializable]
    public class LevelScoreData
    {
        public int levelId;
        public int bestScore;
        public int bestStars;      // 0 to 3
        public float bestTime;     // seconds, 0 = never cleared
        public int fewestDeaths;   // -1 = never cleared
        public int clears;         // how many times the level was cleared

        public LevelScoreData() { fewestDeaths = -1; }
        public LevelScoreData(int levelId) : this() { this.levelId = levelId; }

        // Returns true when this run beat the saved best score.
        public bool Record(int score, int stars, float time, int deaths)
        {
            bool scoreImproved = score > bestScore;
            clears++;
            if (scoreImproved) bestScore = score;
            if (stars > bestStars) bestStars = stars;
            if (bestTime <= 0f || time < bestTime) bestTime = time;
            if (fewestDeaths < 0 || deaths < fewestDeaths) fewestDeaths = deaths;
            return scoreImproved;
        }
    }

    [Serializable]
    public class ScoreDataCollection
    {
        public List<LevelScoreData> scores = new List<LevelScoreData>();

        public LevelScoreData Find(int levelId)
        {
            foreach (var e in scores) if (e.levelId == levelId) return e;
            return null;   // reading never creates a record
        }

        public LevelScoreData GetOrCreate(int levelId)
        {
            var e = Find(levelId);
            if (e == null) { e = new LevelScoreData(levelId); scores.Add(e); }
            return e;
        }

        public int GetTotalStars()
        {
            int total = 0;
            foreach (var e in scores) total += e.bestStars;
            return total;
        }
    }
}