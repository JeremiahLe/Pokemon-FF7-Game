namespace Managers
{
    using UnityEngine;
    using UnityEditor;

    public static class AudioManager
    {
        public static AudioHandler AudioPlayer;

        private const float SFXMax = 1f;
        private const float SFXMin = 0f;

        private const float MusicMax = 1f;
        private const float MusicMin = 0f;

        public static float SFXValue { get; private set; }
        public static float MusicValue { get; private set; }
        
        public static void Initialize()
        {
            if (AudioPlayer != null) return;
            
            var audioSource = Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/AudioSource.prefab"));
            Object.DontDestroyOnLoad(audioSource);
            
            if (!audioSource.TryGetComponent(out AudioHandler source))
            {
                Debug.LogError($"[AudioManager] AudioSource prefab is missing AudioHandler component!");
                return;
            }

            AudioPlayer = source;
            
            // Load preference audio settings
            // MusicValue = pref
            // SFXValue = pref
            MusicValue = 0.5f;
            SFXValue = 0.5f;
            
            ApplySettings();
        }
        
        public static void SetSFXValue(float toValue)
        {
            SFXValue = Mathf.Clamp(toValue, SFXMin, SFXMax);
        }

        public static void SetMusicValue(float toValue)
        {
            MusicValue = Mathf.Clamp(toValue, MusicMin, MusicMax);
            ApplySettings();
        }

        public static void MuteAllAudio()
        {
            MusicValue = 0f;
            SFXValue = 0f;
        }

        public static void PlayMusic(AudioClip clip)
        {
            AudioPlayer.MusicSource.clip = clip;
            AudioPlayer.MusicSource.Play();
            AudioPlayer.MusicSource.loop = true;
        }

        private static void ApplySettings()
        {
            AudioPlayer.MusicSource.volume = MusicValue;
            AudioPlayer.SFXSource.volume = SFXValue;
        }
    }
}
