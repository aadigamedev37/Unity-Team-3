using UnityEngine;

namespace Mohit.MetaSystems
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        private Rect lastSafe;
        private Vector2Int lastScreen;

        private void OnEnable() => Apply();
        private void Update()
        {
            if (Screen.safeArea != lastSafe || lastScreen.x != Screen.width || lastScreen.y != Screen.height)
                Apply();
        }

        private void Apply()
        {
            Rect safe = Screen.safeArea;
            lastSafe = safe;
            lastScreen = new Vector2Int(Screen.width, Screen.height);
            if (Screen.width <= 0 || Screen.height <= 0) return;

            var rt = (RectTransform)transform;
            rt.anchorMin = new Vector2(safe.xMin / Screen.width, safe.yMin / Screen.height);
            rt.anchorMax = new Vector2(safe.xMax / Screen.width, safe.yMax / Screen.height);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }
    }
}