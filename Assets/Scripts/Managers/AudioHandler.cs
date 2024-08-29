namespace Managers
{
    using Sirenix.OdinInspector;
    using UnityEngine;
    
    public class AudioHandler : MonoBehaviour
    {
        public AudioSource MusicSource { get; private set; }
        public AudioSource SFXSource { get; private set; }

        [SerializeField] private GameObject _musicSource;
        [SerializeField] private GameObject _SFXSource;
    
        [field: SerializeField, BoxGroup("Music References")] public AudioClip MainMenuTheme { get; private set; }
        [field: SerializeField, BoxGroup("Music References")] public AudioClip BasicCombatTheme { get; private set; }
        [field: SerializeField, BoxGroup("Music References")] public AudioClip BasicBossTheme { get; private set; }
    
        private void Awake()
        {
            MusicSource = _musicSource.GetComponent<AudioSource>();
            SFXSource = _SFXSource.GetComponent<AudioSource>();
        }
    }
}
