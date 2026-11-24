using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mohit.MetaSystems
{
    public class RunHUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text deathsText;
        [SerializeField] private GameObject deathsRoot;       // skull + number, hidden until first death
        [SerializeField] private Image[] stealPips;           // placed in the scene, one per possible steal
        [SerializeField] private Color pipEmpty = new Color(0.55f, 0.1f, 0.17f, 0.35f);

        private int lastSteals = -1;

        private void Update()
        {
            var t = RunTracker.Instance;
            if (t == null) return;

            timerText.text = t.Elapsed.ToString("F2");
            timerText.color = t.Elapsed <= t.Par ? UIColors.GoldBright
                            : t.Elapsed <= 2f * t.Par ? UIColors.CrimsonBright
                            : UIColors.TextQuiet;

            if (deathsRoot != null) deathsRoot.SetActive(t.Deaths > 0);
            if (deathsText != null) deathsText.text = t.Deaths.ToString();

            if (t.Steals != lastSteals)
            {
                lastSteals = t.Steals;
                int required = t.RequiredSteals;
                for (int i = 0; i < stealPips.Length; i++)
                {
                    bool used = i < t.Steals;
                    stealPips[i].gameObject.SetActive(i < Mathf.Max(required, t.Steals));
                    stealPips[i].color = !used ? pipEmpty
                                       : i < required ? UIColors.CrimsonBright
                                       : UIColors.GoldBright;
                }
            }
        }
    }
}