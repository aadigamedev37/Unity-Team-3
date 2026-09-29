using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mohit.MetaSystems
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SettingsUI : MonoBehaviour
    {
        [Header("Confirm dialog")]
        [SerializeField] private GameObject resetConfirmPanel;

        [Header("Sliders")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Buttons inside the panel")]
        [SerializeField] private Button muteButton;
        [SerializeField] private Button qualityButton;
        [SerializeField] private Button fpsButton;
        [SerializeField] private Button resetSettingsButton;
        [SerializeField] private Button resetProgressButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button confirmYesButton;
        [SerializeField] private Button confirmNoButton;

        [Header("Button that opens this panel")]
        [SerializeField] private Button openButton;

        private CanvasGroup canvasGroup;
        private TMP_Text muteLabel;
        private TMP_Text qualityLabel;
        private TMP_Text fpsLabel;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            SetVisible(false);
        }

        private void Start()
        {
            SettingsManager s = SettingsManager.Instance;
            if (s == null)
            {
                Debug.LogError("SettingsUI: SettingsManager not found.");
                return;
            }

            muteLabel = muteButton.GetComponentInChildren<TMP_Text>();
            qualityLabel = qualityButton.GetComponentInChildren<TMP_Text>();
            fpsLabel = fpsButton.GetComponentInChildren<TMP_Text>();

            Refresh();

            masterSlider.onValueChanged.AddListener(s.SetMasterVolume);
            musicSlider.onValueChanged.AddListener(s.SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(s.SetSfxVolume);

            muteButton.onClick.AddListener(() => { s.SetMuted(!s.IsMuted); Refresh(); });
            qualityButton.onClick.AddListener(() =>
            {
                s.SetQuality((s.QualityLevel + 1) % QualitySettings.names.Length);
                Refresh();
            });
            fpsButton.onClick.AddListener(() => { s.SetFps60(!s.Fps60); Refresh(); });
            resetSettingsButton.onClick.AddListener(() => { s.ResetToDefaults(); Refresh(); });

            resetProgressButton.onClick.AddListener(() => resetConfirmPanel.SetActive(true));
            confirmYesButton.onClick.AddListener(ConfirmResetProgress);
            confirmNoButton.onClick.AddListener(() => resetConfirmPanel.SetActive(false));

            closeButton.onClick.AddListener(Close);
            if (openButton != null) openButton.onClick.AddListener(Open);

            resetConfirmPanel.SetActive(false);
        }

        public void Open()
        {
            transform.SetAsLastSibling(); // always draw on top
            Refresh();
            SetVisible(true);
        }

        public void Close()
        {
            if (SettingsManager.Instance != null)
                SettingsManager.Instance.Save();

            resetConfirmPanel.SetActive(false);
            SetVisible(false);
        }

        private void ConfirmResetProgress()
        {
            if (SaveManager.Instance != null)
                SaveManager.Instance.ResetProgress();
            else
                Debug.LogWarning("SettingsUI: No SaveManager in memory, nothing to reset.");

            resetConfirmPanel.SetActive(false);
        }

        private void Refresh()
        {
            SettingsManager s = SettingsManager.Instance;
            if (s == null || muteLabel == null) return;

            masterSlider.SetValueWithoutNotify(s.MasterVolume);
            musicSlider.SetValueWithoutNotify(s.MusicVolume);
            sfxSlider.SetValueWithoutNotify(s.SfxVolume);

            muteLabel.text = "Mute: " + (s.IsMuted ? "ON" : "OFF");
            qualityLabel.text = "Quality: " + QualitySettings.names[s.QualityLevel];
            fpsLabel.text = "Frame Rate: " + (s.Fps60 ? "60" : "30");
        }

        private void SetVisible(bool visible)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}