using UnityEngine;

namespace Mohit.MetaSystems
{
    public class ScannerSweep : MonoBehaviour
    {
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private float periodSeconds = 14f;

        private RectTransform rt;
        private CanvasGroup group;

        private void Awake()
        {
            rt = (RectTransform)transform;
            group = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            bool off = MotionSettings.Reduced;
            if (group != null) group.alpha = off ? 0f : 1f;
            if (off || canvasRect == null) return;

            float width = canvasRect.rect.width + rt.rect.width;
            float k = (Time.unscaledTime % periodSeconds) / periodSeconds;
            rt.anchoredPosition = new Vector2(-width * 0.5f + width * k, 0f);
        }
    }
}