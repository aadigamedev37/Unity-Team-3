using UnityEngine;

namespace Mohit.MetaSystems
{
    public class ResultsDebug : MonoBehaviour
    {
        [SerializeField] private LevelCompleteScreen screen;

        [ContextMenu("Show clean run")]
        public void Clean() { Show(22f, 0, 1); }

        [ContextMenu("Show okay run")]
        public void Okay() { Show(30f, 2, 2); }

        [ContextMenu("Show rough run")]
        public void Rough() { Show(40f, 9, 4); }

        private void Show(float time, int deaths, int steals)
        {
            screen.OnNext = () => Debug.Log("NEXT pressed (test only)");  
            var r = ScoreCalculator.Calculate(time, 20f, deaths, steals, 1);
            screen.Show(4, LevelNames.Get(4), time, deaths, steals, 1, r, r.Score, true);
        }
    }
}