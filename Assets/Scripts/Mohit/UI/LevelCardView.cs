using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Mohit.MetaSystems
{
    // Sits on the LevelCard prefab. Only displays; it never decides anything.
    public class LevelCardView : MonoBehaviour
    {
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text bestTimeText;
        [SerializeField] private Image[] starImages = new Image[3];
        [SerializeField] private GameObject lockIcon;
        [SerializeField] private Image borderFill;   // the Visual image

        public void Bind(int levelId, bool unlocked, bool cleared, int stars, float bestTime)
        {
            numberText.text = levelId.ToString("D2");
            nameText.text = LevelNames.Get(levelId);

            numberText.color = unlocked ? UIColors.Text : UIColors.TextQuiet;
            nameText.color = UIColors.TextQuiet;
            if (lockIcon != null) lockIcon.SetActive(!unlocked);
            if (borderFill != null)
                borderFill.color = unlocked ? new Color(0.047f, 0.039f, 0.071f, 1f)
                                            : new Color(0.047f, 0.039f, 0.071f, 0.6f);

            for (int i = 0; i < starImages.Length; i++)
            {
                starImages[i].gameObject.SetActive(unlocked);
                starImages[i].color = (cleared && i < stars) ? UIColors.Gold : UIColors.CrimsonDeep;
            }

            bestTimeText.text = (cleared && bestTime > 0f && bestTime < 9999f) ? bestTime.ToString("F2") + " s" : "";
        }
    }
}