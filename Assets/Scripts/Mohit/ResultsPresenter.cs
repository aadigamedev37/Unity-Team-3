using UnityEngine;

namespace Mohit.MetaSystems
{
    public class ResultsPresenter : MonoBehaviour
    {
        [SerializeField] private LevelCompleteScreen screen;

        private void Start()
        {
            if (RunTracker.Instance != null) RunTracker.Instance.OnRunCompleted += Present;
        }

        private void OnDestroy()
        {
            if (RunTracker.Instance != null) RunTracker.Instance.OnRunCompleted -= Present;
        }

        private void Present(RunSummary s)
        {
            screen.Show(s.LevelId, LevelNames.Get(s.LevelId), s.Time, s.Deaths, s.Steals,
                        s.RequiredSteals, s.Result, s.BestScore, s.IsNewBest);
        }
    }
}