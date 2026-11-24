using UnityEngine;

namespace Mohit.MetaSystems
{
    public struct ScoreResult
    {
        public int Score, Stars, TimeBonus, PrecisionBonus, EfficiencyBonus;
        public float PrecisionPercent;   // 100 = first-try clear
    }

    public static class ScoreCalculator
    {
        public const int Base = 1000, MaxScore = 2000, TwoStars = 1200, ThreeStars = 1700;

        public static ScoreResult Calculate(float time, float par, int deaths, int steals, int requiredSteals)
        {
            float timeRatio = par > 0f ? Mathf.Clamp01((2f * par - time) / par) : 0f;
            int timeBonus = Mathf.RoundToInt(500f * timeRatio);

            int safeDeaths = Mathf.Max(0, deaths);
            float precision = 1f / (safeDeaths + 1);
            int precisionBonus = Mathf.RoundToInt(300f * precision);

            int stealGap = Mathf.Abs(steals - requiredSteals);
            int efficiencyBonus = Mathf.Max(0, 200 - 50 * stealGap);

            int score = Mathf.Clamp(Base + timeBonus + precisionBonus + efficiencyBonus, 0, MaxScore);
            int stars = score >= ThreeStars ? 3 : score >= TwoStars ? 2 : 1;   // a clear is never below 1

            return new ScoreResult
            {
                Score = score, Stars = stars,
                TimeBonus = timeBonus, PrecisionBonus = precisionBonus, EfficiencyBonus = efficiencyBonus,
                PrecisionPercent = precision * 100f
            };
        }
    }
}