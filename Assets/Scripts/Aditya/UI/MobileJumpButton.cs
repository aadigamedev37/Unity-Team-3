using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// On-screen mobile Jump Button presenter.
/// Tapping the button or pressing Space Bar triggers Jump on the active character.
/// </summary>
[DisallowMultipleComponent]
public class MobileJumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private InputSystem inputSystem;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonBackground;
    [SerializeField] private Image jumpIcon;

    [Header("Visual Feedback")]
    [SerializeField] private float pressedScale = 0.92f;
    [SerializeField] private Color normalColor = new Color(0.08f, 0.10f, 0.13f, 0.92f);
    [SerializeField] private Color pressedColor = new Color(0.12f, 0.22f, 0.28f, 0.98f);

    private Vector3 originalScale;
    private bool isPointerDown = false;

    private void Awake()
    {
        originalScale = transform.localScale;

        if (inputSystem == null)
        {
            inputSystem = FindAnyObjectByType<InputSystem>();
        }

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (buttonBackground == null)
        {
            buttonBackground = GetComponent<Image>();
        }

        if (jumpIcon == null)
        {
            var iconTrans = transform.Find("Icon");
            if (iconTrans != null) jumpIcon = iconTrans.GetComponent<Image>();
        }

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        transform.localScale = originalScale * pressedScale;
        if (buttonBackground != null)
        {
            buttonBackground.color = pressedColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        transform.localScale = originalScale;
        if (buttonBackground != null)
        {
            buttonBackground.color = normalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TriggerJumpAction();
    }

    private void OnButtonClicked()
    {
        if (!isPointerDown)
        {
            TriggerJumpAction();
        }
    }

    public void TriggerJumpAction()
    {
        if (inputSystem == null)
        {
            inputSystem = FindAnyObjectByType<InputSystem>();
        }

        if (inputSystem != null)
        {
            inputSystem.TriggerJump();
        }
    }
}
