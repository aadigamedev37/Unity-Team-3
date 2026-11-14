using UnityEngine;

namespace Mohit.MetaSystems
{
    public class ScoreSelfTest : MonoBehaviour
    {
        private void Start()
        {
            Check(22f, 20f, 0, 1, 1, 1950, 3);   // clean
            Check(30f, 20f, 2, 2, 1, 1500, 2);   // okay
            Check(40f, 20f, 9, 4, 1, 1080, 1);   // rough
            Check(10f, 20f, 0, 0, 3, 1850, 3);   // 0 steals on a 3-steal level: efficiency 50, never above 200
        }

        private void Check(float time, float par, int deaths, int steals, int required, int expectScore, int expectStars)
        {
            var r = ScoreCalculator.Calculate(time, par, deaths, steals, required);
            bool ok = r.Score == expectScore && r.Stars == expectStars;
            Debug.Log($"{(ok ? "PASS" : "FAIL")}: score {r.Score} (want {expectScore}), stars {r.Stars} (want {expectStars})");
        }
    }
}