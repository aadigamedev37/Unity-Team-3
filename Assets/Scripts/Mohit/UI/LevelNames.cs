namespace Mohit.MetaSystems
{
    public static class LevelNames
    {
        public static readonly string[] All =
        {
            "The Corridor", "The Sandbox", "The Ghost Gate", "The Vent", "The Spotlight",
            "Double Lock", "Long Detour", "Pulsing Corridor", "Split Decision", "The Gauntlet",
            "Trap Light Introduction", "The Moving Platform", "Light Switch", "Tight Corridors", "Final Escape"
        };

        public static string Get(int levelId) =>
            levelId >= 1 && levelId <= All.Length ? All[levelId - 1] : "Unknown";
    }
}