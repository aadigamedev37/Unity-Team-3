using UnityEngine;

namespace Mohit.MetaSystems
{
    public static class UIColors
    {
        public static readonly Color Void         = Hex("0A0A10");
        public static readonly Color Text         = Hex("E0DDD8");
        public static readonly Color TextQuiet    = Hex("9A9AA6");
        public static readonly Color Crimson      = Hex("C92D3E");
        public static readonly Color CrimsonBright= Hex("E63946");
        public static readonly Color CrimsonDeep  = Hex("8B1A2B");
        public static readonly Color Gold         = Hex("D4A520");
        public static readonly Color GoldBright   = Hex("F0C040");

        public const string CrimsonBrightTag = "#E63946";
        public const string GoldTag          = "#D4A520";
        public const string TextQuietTag     = "#9A9AA6";

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out Color c);
            return c;
        }
    }
}