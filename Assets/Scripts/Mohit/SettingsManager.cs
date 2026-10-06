using System;
using UnityEngine;

namespace Mohit.MetaSystems
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        public event Action OnSettingsChanged;

        private const string MasterKey = "settings_master";
        private const string MusicKey = "settings_music";
        private const string SfxKey = "settings_sfx";
        private const string MuteKey = "settings_mute";
        private const string QualityKey = "settings_quality";
        private const string FpsKey = "settings_fps60";

        public float MasterVolume { get; private set; } = 1f;
        public float MusicVolume { get; private set; } = 1f;
        public float SfxVolume { get; private set; } = 1f;
        public bool IsMuted { get; private set; }
        public int QualityLevel { get; private set; }
        public bool Fps60 { get; private set; } = true;

        private int defaultQualityLevel;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoCreate()
        {
            if (Instance != null) return;
            new GameObject("SettingsManager").AddComponent<SettingsManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            defaultQualityLevel = QualitySettings.GetQualityLevel();
            Load();
            ApplyAudio();
            ApplyQuality();
            ApplyFrameRate();
        }

        public void SetMasterVolume(float value)
        {
            MasterVolume = Mathf.Clamp01(value);
            ApplyAudio();
            OnSettingsChanged?.Invoke();
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = Mathf.Clamp01(value);
            OnSettingsChanged?.Invoke();
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume = Mathf.Clamp01(value);
            OnSettingsChanged?.Invoke();
        }

        public void SetMuted(bool muted)
        {
            IsMuted = muted;
            ApplyAudio();
            OnSettingsChanged?.Invoke();
        }

        public void SetQuality(int level)
        {
            QualityLevel = Mathf.Clamp(level, 0, QualitySettings.names.Length - 1);
            ApplyQuality();
            OnSettingsChanged?.Invoke();
        }

        public void SetFps60(bool on)
        {
            Fps60 = on;
            ApplyFrameRate();
            OnSettingsChanged?.Invoke();
        }

        public void ResetToDefaults()
        {
            MasterVolume = 1f;
            MusicVolume = 1f;
            SfxVolume = 1f;
            IsMuted = false;
            QualityLevel = defaultQualityLevel;
            Fps60 = true;

            ApplyAudio();
            ApplyQuality();
            ApplyFrameRate();
            Save();
            OnSettingsChanged?.Invoke();
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(MasterKey, MasterVolume);
            PlayerPrefs.SetFloat(MusicKey, MusicVolume);
            PlayerPrefs.SetFloat(SfxKey, SfxVolume);
            PlayerPrefs.SetInt(MuteKey, IsMuted ? 1 : 0);
            PlayerPrefs.SetInt(QualityKey, QualityLevel);
            PlayerPrefs.SetInt(FpsKey, Fps60 ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void Load()
        {
            MasterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MasterKey, 1f));
            MusicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicKey, 1f));
            SfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxKey, 1f));
            IsMuted = PlayerPrefs.GetInt(MuteKey, 0) == 1;
            QualityLevel = Mathf.Clamp(PlayerPrefs.GetInt(QualityKey, defaultQualityLevel),
                                       0, QualitySettings.names.Length - 1);
            Fps60 = PlayerPrefs.GetInt(FpsKey, 1) == 1;
        }

        private void ApplyAudio() => AudioListener.volume = IsMuted ? 0f : MasterVolume;
        private void ApplyQuality() => QualitySettings.SetQualityLevel(QualityLevel, true);
        private void ApplyFrameRate() => Application.targetFrameRate = Fps60 ? 60 : 30;

        private void OnApplicationPause(bool paused) { if (paused) Save(); }
        private void OnApplicationQuit() => Save();
    }
}