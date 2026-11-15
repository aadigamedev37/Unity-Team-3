using UnityEngine;

namespace Mohit.MetaSystems
{
    public static class MotionSettings
    {
        public const string Key = "ReduceMotion";
        public static bool Reduced => PlayerPrefs.GetInt(Key, 0) == 1;
    }

    public class ShadowDrift : MonoBehaviour
    {
        [SerializeField] private Vector2 restOffset = new Vector2(8f, -8f);
        [SerializeField] private float driftPixels = 3f;
        [SerializeField] private float periodSeconds = 6f;

        private RectTransform rt;
        private Vector2 basePos;

        private void Awake()
        {
            rt = (RectTransform)transform;
            basePos = rt.anchoredPosition;
        }

        private void Update()
        {
            if (MotionSettings.Reduced) { rt.anchoredPosition = basePos + restOffset; return; }
            float k = Mathf.Sin(Time.unscaledTime * 2f * Mathf.PI / periodSeconds);
            rt.anchoredPosition = basePos + restOffset + new Vector2(k, -k) * driftPixels;
        }
    }
}