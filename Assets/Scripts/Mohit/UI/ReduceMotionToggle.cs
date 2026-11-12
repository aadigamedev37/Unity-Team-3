using UnityEngine;
using UnityEngine.UI;

namespace Mohit.MetaSystems
{
    [RequireComponent(typeof(Toggle))]
    public class ReduceMotionToggle : MonoBehaviour
    {
        private Toggle toggle;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.SetIsOnWithoutNotify(MotionSettings.Reduced);
            toggle.onValueChanged.AddListener(OnChanged);
        }

        private void OnChanged(bool on)
        {
            PlayerPrefs.SetInt(MotionSettings.Key, on ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}