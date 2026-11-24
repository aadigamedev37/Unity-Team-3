using System.Collections.Generic;
using UnityEngine;

namespace Mohit.MetaSystems
{
    /// <summary>
    /// Hardcoded fallback defaults for level scoring when no ScriptableObject instance is available.
    /// </summary>
    public struct LevelScoringDefaults
    {
        /// <summary>Target completion time in seconds.</summary>
        public float TargetTime;

        /// <summary>Minimum number of steals required for a perfect efficiency bonus.</summary>
        public int RequiredSteals;

        /// <summary>Difficulty rating from 1 (easiest) to 10 (hardest).</summary>
        public int Difficulty;

        public LevelScoringDefaults(float targetTime, int requiredSteals, int difficulty)
        {
            TargetTime = targetTime;
            RequiredSteals = requiredSteals;
            Difficulty = difficulty;
        }
    }

    /// <summary>
    /// A companion ScriptableObject that extends scoring metadata for levels
    /// without modifying the original <c>LevelConfig</c>.
    /// <para>
    /// When a runtime asset exists it is used; otherwise hardcoded GDD defaults
    /// are returned via <see cref="GetTargetTime"/> and <see cref="GetRequiredSteals"/>.
    /// </para>
    /// </summary>
    [CreateAssetMenu(fileName = "LevelScoring", menuName = "Game/Levels/Level Scoring Config")]
    public class LevelConfigExtended : ScriptableObject
    {
        // ──────────────────────────────────────────────
        //  Serialised Fields
        // ──────────────────────────────────────────────

        [Tooltip("Must match the corresponding LevelConfig.LevelId.")]
        [SerializeField] private int levelId;

        [Tooltip("Target completion time in seconds (for 3-star time bonus).")]
        [SerializeField] private float targetTime;

        [Tooltip("Number of steals required for a perfect efficiency bonus.")]
        [SerializeField] private int requiredSteals;

        [Tooltip("Difficulty rating 1-10.")]
        [SerializeField, Range(1, 10)] private int difficulty = 1;

        // ──────────────────────────────────────────────
        //  Public Accessors
        // ──────────────────────────────────────────────

        /// <summary>Level identifier that mirrors <c>LevelConfig.LevelId</c>.</summary>
        public int LevelId => levelId;

        /// <summary>Target completion time in seconds.</summary>
        public float TargetTime => targetTime;

        /// <summary>Required number of steals for a perfect efficiency score.</summary>
        public int RequiredSteals => requiredSteals;

        /// <summary>Difficulty rating (1–10).</summary>
        public int Difficulty => difficulty;

        // ──────────────────────────────────────────────
        //  Runtime Instance Registry
        // ──────────────────────────────────────────────

        /// <summary>All loaded instances keyed by level id for fast lookup.</summary>
        private static readonly Dictionary<int, LevelConfigExtended> s_instances = new Dictionary<int, LevelConfigExtended>();

        private void OnEnable()
        {
            s_instances[levelId] = this;
        }

        private void OnDisable()
        {
            if (s_instances.TryGetValue(levelId, out var existing) && existing == this)
                s_instances.Remove(levelId);
        }

        // ──────────────────────────────────────────────
        //  Hardcoded GDD Defaults
        // ──────────────────────────────────────────────

        /// <summary>
        /// Hardcoded fallback data for all 15 levels, sourced from the GDD.
        /// Used when no <see cref="LevelConfigExtended"/> asset is loaded for a given level.
        /// </summary>
        public static readonly Dictionary<int, LevelScoringDefaults> Defaults = new Dictionary<int, LevelScoringDefaults>
        {
            //                          targetTime  requiredSteals  difficulty
            {  1, new LevelScoringDefaults( 5f,  0, 1) },  // Tutorial — no steals
            {  2, new LevelScoringDefaults(10f,  0, 1) },  // Tutorial — no steals
            {  3, new LevelScoringDefaults(15f,  1, 2) },  // 1 Door steal
            {  4, new LevelScoringDefaults(20f,  1, 2) },  // 1 Drawbridge steal
            {  5, new LevelScoringDefaults(25f,  1, 3) },  // 1 Door steal
            {  6, new LevelScoringDefaults(25f,  3, 4) },
            {  7, new LevelScoringDefaults(30f,  3, 5) },
            {  8, new LevelScoringDefaults(30f,  4, 5) },
            {  9, new LevelScoringDefaults(30f,  4, 6) },
            { 10, new LevelScoringDefaults(35f,  4, 6) },
            { 11, new LevelScoringDefaults(35f,  5, 7) },
            { 12, new LevelScoringDefaults(35f,  5, 7) },
            { 13, new LevelScoringDefaults(40f,  5, 8) },
            { 14, new LevelScoringDefaults(40f,  6, 9) },
            { 15, new LevelScoringDefaults(40f,  6, 10) },
        };

        // ──────────────────────────────────────────────
        //  Static Accessors
        // ──────────────────────────────────────────────

        /// <summary>
        /// Returns the target completion time for <paramref name="levelId"/>.
        /// Prefers a loaded <see cref="LevelConfigExtended"/> asset; falls back to
        /// the hardcoded GDD defaults.
        /// </summary>
        /// <param name="levelId">1-based level identifier.</param>
        /// <returns>Target time in seconds, or 30 s if the level id is unknown.</returns>
        public static float GetTargetTime(int levelId)
        {
            if (s_instances.TryGetValue(levelId, out var ext))
                return ext.targetTime;

            if (Defaults.TryGetValue(levelId, out var def))
                return def.TargetTime;

            Debug.LogWarning($"[LevelConfigExtended] No data for level {levelId}. Returning 30 s default.");
            return 30f;
        }

        /// <summary>
        /// Returns the required steal count for <paramref name="levelId"/>.
        /// Prefers a loaded <see cref="LevelConfigExtended"/> asset; falls back to
        /// the hardcoded GDD defaults.
        /// </summary>
        /// <param name="levelId">1-based level identifier.</param>
        /// <returns>Required steals, or 3 if the level id is unknown.</returns>
        public static int GetRequiredSteals(int levelId)
        {
            if (s_instances.TryGetValue(levelId, out var ext))
                return ext.requiredSteals;

            if (Defaults.TryGetValue(levelId, out var def))
                return def.RequiredSteals;

            Debug.LogWarning($"[LevelConfigExtended] No data for level {levelId}. Returning 3 steals default.");
            return 3;
        }
    }
}
