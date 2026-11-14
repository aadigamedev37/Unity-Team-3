using System;
using UnityEngine;

namespace Mohit.MetaSystems
{
    public struct RunSummary
    {
        public int LevelId, Deaths, Steals, RequiredSteals, BestScore;
        public float Time;
        public bool IsNewBest;
        public ScoreResult Result;
    }

    public class RunTracker : MonoBehaviour
    {
        public static RunTracker Instance { get; private set; }

        [SerializeField] private int levelId = 1;      // set per scene in the inspector
        [SerializeField] private bool autoStart = true;

        public int LevelId => levelId;
        public float Elapsed { get; private set; }
        public int Deaths { get; private set; }
        public int Steals { get; private set; }
        public bool IsRunning { get; private set; }
        public float Par => LevelConfigExtended.GetTargetTime(levelId);
        public int RequiredSteals => LevelConfigExtended.GetRequiredSteals(levelId);

        public event Action<RunSummary> OnRunCompleted;

        private void Awake()
        {
            Instance = this;
            Deaths = ScoringSystemBridge.DeathCarry;   // survives the restart reload
        }

        private void Start() { if (autoStart) BeginRun(); }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        private void Update()
        {
            // Time.deltaTime is 0 while the game is paused, so the timer freezes by itself.
            if (IsRunning) Elapsed += Time.deltaTime;
        }

        public void BeginRun() { IsRunning = true; }

        public void RegisterDeath()
        {
            Deaths++;
            ScoringSystemBridge.DeathCarry = Deaths;
        }

        public void RegisterSteal() { Steals++; }

        public void Complete()
        {
            if (!IsRunning) return;          // ignore a second call
            IsRunning = false;

            ScoreResult r = ScoreCalculator.Calculate(Elapsed, Par, Deaths, Steals, RequiredSteals);

            bool isNewBest = false;
            int best = r.Score;
            if (SaveManager.Instance != null)
            {
                isNewBest = SaveManager.Instance.SaveScore(levelId, r.Score, r.Stars, Elapsed, Deaths);
                SaveManager.Instance.CompleteLevel(levelId);
                var data = SaveManager.Instance.GetScoreData(levelId);
                if (data != null) best = data.bestScore;
            }

            ScoringSystemBridge.ResetDeaths();

            OnRunCompleted?.Invoke(new RunSummary
            {
                LevelId = levelId, Deaths = Deaths, Steals = Steals, RequiredSteals = RequiredSteals,
                Time = Elapsed, BestScore = best, IsNewBest = isNewBest, Result = r
            });
        }
    }
}