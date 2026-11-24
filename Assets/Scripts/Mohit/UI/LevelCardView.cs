using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Mohit.MetaSystems
{
    // Sits on the LevelCard prefab. Only displays; it never decides anything.
    public class LevelCardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private TMP_Text numberText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text bestTimeText;
        [SerializeField] private Image[] starImages = new Image[3];
        [SerializeField] private GameObject lockIcon;
        [SerializeField] private Image borderFill;   // the Visual image
        [SerializeField] private Image thumbnail;
        [SerializeField] private GameObject playIcon;
        [SerializeField] private GameObject currentGlow;


        public void Bind(int levelId, bool unlocked, bool cleared, int stars, float bestTime)
        {
            numberText.text = levelId.ToString("D2");
            nameText.text = LevelNames.Get(levelId);

            numberText.color = unlocked ? UIColors.Text : UIColors.TextQuiet;
            nameText.color = UIColors.TextQuiet;
            if (lockIcon != null) lockIcon.SetActive(!unlocked);
            if (borderFill != null)
            {
                borderFill.color = new Color(0.047f, 0.039f, 0.071f, 1f);   // always opaque, or the outline bleeds through

                Outline outline = borderFill.GetComponent<Outline>();
                if (outline != null)
                    outline.effectColor = unlocked ? new Color(0.788f, 0.176f, 0.243f, 0.6f)    // C92D3E, strong border
                                                   : new Color(0.788f, 0.176f, 0.243f, 0.15f);  // C92D3E, faint border
            }

            for (int i = 0; i < starImages.Length; i++)
            {
                starImages[i].gameObject.SetActive(unlocked);
                starImages[i].color = (cleared && i < stars) ? UIColors.Gold : UIColors.CrimsonDeep;
            }

            bestTimeText.text = (cleared && bestTime > 0f && bestTime < 9999f) ? bestTime.ToString("F2") + " s" : "";
        }

        public void SetThumbnail(Sprite sprite, bool locked)
        {
            if (thumbnail == null) return;
            if (sprite != null) thumbnail.sprite = sprite;
            thumbnail.color = locked ? new Color(0.30f, 0.30f, 0.36f, 1f) : Color.white;
        }

        public void SetCurrent(bool unlocked, bool current)
        {
            // every unlocked card shows the arrow, so cleared levels can be replayed
            if (playIcon != null)
            {
                playIcon.SetActive(unlocked);
                Image arrow = playIcon.GetComponent<Image>();
                if (arrow != null)
                    arrow.color = current ? Color.white : new Color(1f, 1f, 1f, 0.75f);
            }

            // only the next level to play gets the glow and the bright edge
            if (currentGlow != null) currentGlow.SetActive(current);

            Outline edge = GetComponentInChildren<Outline>(true);
            if (edge != null && current)
            {
                edge.effectColor = new Color(0.90f, 0.22f, 0.27f, 1f);
                edge.effectDistance = new Vector2(3f, -3f);
            }

            popScale = 1f;
            if (playIcon != null) playIcon.transform.localScale = Vector3.one;
        }

        // ---- arrow pop on hover / press (a touch counts as a press) ----
        bool hovered, pressed;
        float popScale = 1f;

        public void OnPointerEnter(PointerEventData e) { hovered = true; }
        public void OnPointerExit(PointerEventData e) { hovered = false; pressed = false; }
        public void OnPointerDown(PointerEventData e) { pressed = true; }
        public void OnPointerUp(PointerEventData e) { pressed = false; }

        void Update()
        {
            if (playIcon == null || !playIcon.activeSelf) return;
            float target = pressed ? 1.35f : (hovered ? 1.2f : 1f);
            popScale = Mathf.Lerp(popScale, target, 1f - Mathf.Exp(-18f * Time.unscaledDeltaTime));
            playIcon.transform.localScale = Vector3.one * popScale;
        }
    }
}