using UnityEngine;
using TMPro;

namespace Mohit.MetaSystems
{
    // Put on the Pause panel object. OnEnable runs each time the panel is shown.
    public class PauseStats : MonoBehaviour
    {
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text deathsText;
        [SerializeField] private TMP_Text stealsText;

        private void OnEnable()
        {
            var t = RunTracker.Instance;
            if (t == null) return;
            timeText.text = t.Elapsed.ToString("F2");
            deathsText.text = t.Deaths.ToString();
            stealsText.text = t.Steals + " / " + t.RequiredSteals;
        }
    }
}
