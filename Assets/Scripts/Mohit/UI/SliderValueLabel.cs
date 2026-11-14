using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mohit.MetaSystems
{
    // Shows a slider's value as a percentage. Display only.
    public class SliderValueLabel : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text label;

        private int lastShown = -1;

        private void LateUpdate()
        {
            if (slider == null || label == null) return;

            float range = slider.maxValue - slider.minValue;
            int percent = range > 0f
                ? Mathf.RoundToInt((slider.value - slider.minValue) / range * 100f)
                : 0;

            if (percent == lastShown) return;
            lastShown = percent;
            label.text = percent + "%";
        }
    }
}