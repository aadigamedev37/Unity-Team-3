using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mohit.MetaSystems
{
    public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private Image accentBar;
        [SerializeField] private RectTransform content;
        [SerializeField] private float duration = 0.12f;
        [SerializeField] private float pressShift = 4f;

        private Vector2 restPos;
        private float t;
        private bool pressed;

        private void Awake()
        {
            if (content != null) restPos = content.anchoredPosition;
            Apply();
        }

        private void OnDisable() { pressed = false; t = 0f; Apply(); }

        private void Update()
        {
            float target = pressed ? 1f : 0f;
            if (Mathf.Approximately(t, target)) return;
            t = Mathf.MoveTowards(t, target, Time.unscaledDeltaTime / Mathf.Max(0.01f, duration));
            Apply();
        }

        private void Apply()
        {
            if (accentBar != null)
                accentBar.color = Color.Lerp(UIColors.CrimsonDeep, UIColors.CrimsonBright, t);
            if (content != null)
                content.anchoredPosition = restPos + new Vector2(pressShift * t, 0f);
        }

        public void OnPointerDown(PointerEventData e) { pressed = true; }
        public void OnPointerUp(PointerEventData e)   { pressed = false; }
        public void OnPointerExit(PointerEventData e) { pressed = false; }
    }
}