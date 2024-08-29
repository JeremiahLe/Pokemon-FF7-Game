using System.Text;

namespace Managers
{   
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using Databases;
    
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Scene Paths")]
        [SerializeField] private const string Main = "Main";
        [SerializeField] private const string AdventureMenu = "AdventureBeginScene";
    
        [Header("Menus")]
        [SerializeField] private GameObject[] _menuArray;
    
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _singlePlayerMenu;
        [SerializeField] private GameObject _creditsMenu;
        [SerializeField] private GameObject _settingsMenu;
        [SerializeField] private GameObject _codexMenu;

        [Header("Button References")] 
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _creditsButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _codexButton;
        [SerializeField] private Button _adventureButton;
    
        [SerializeField] private Button _returnFromSinglePlayerButton;
        [SerializeField] private Button _returnFromCreditsButton;
        [SerializeField] private Button _returnFromSettingsButton;
        [SerializeField] private Button _returnFromCodexButton;

        [Header("Codex Menu")] 
        [SerializeField] private Image _chimericMain;
        [SerializeField] private TextMeshProUGUI _chimericName;
        [SerializeField] private TextMeshProUGUI _chimericElements;
        [SerializeField] private TextMeshProUGUI _chimericFlavorText;
        [SerializeField] private Transform _contentGrid;
        [SerializeField] private GameObject _codexIcon;

        private List<CodexIcon> _codexIcons;

        [Header("Settings Menu")] 
        [SerializeField] private Slider _SFXSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private TextMeshProUGUI _sfxSliderValue;
        [SerializeField] private TextMeshProUGUI _musicSliderValue;
    
        [Header("Other References")]
        [SerializeField] private GameObject _fadeAnimatorParent;
        private Animator _fadeAnimator;
    
        private static readonly int Start = Animator.StringToHash("Start");

        private void Awake()
        {
            AudioManager.PlayMusic(AudioManager.AudioPlayer.MainMenuTheme);
            
            _fadeAnimator = _fadeAnimatorParent.GetComponent<Animator>();
            _fadeAnimatorParent.GetComponent<Image>().enabled = true;
            _codexIcons = new List<CodexIcon>();
        
            AssignButtonEvents();
            AssignSliderEvents();
        }
    
        private void AssignButtonEvents()
        {
            _playButton.onClick.AddListener(() => SetMenuActive(_singlePlayerMenu));
            _creditsButton.onClick.AddListener(() => SetMenuActive(_creditsMenu));
            _settingsButton.onClick.AddListener(() => SetMenuActive(_settingsMenu));
            _codexButton.onClick.AddListener(() =>
            {
                LoadChimericDatabase();
                SetMenuActive(_codexMenu);
            });
            _quitButton.onClick.AddListener(QuitButtonClicked);
        
            _returnFromSinglePlayerButton.onClick.AddListener(() => SetMenuActive(_mainMenu));
            _returnFromCodexButton.onClick.AddListener(() =>
            {
                ClearCodexIcons();
                SetMenuActive(_singlePlayerMenu);
            });
            _returnFromSettingsButton.onClick.AddListener(() => SetMenuActive(_mainMenu));
            _returnFromCreditsButton.onClick.AddListener(() => SetMenuActive(_mainMenu));
        }

        private void AssignSliderEvents()
        {
            _musicSlider.onValueChanged.AddListener(toValue=>
            {
                AudioManager.SetMusicValue(toValue);
                _musicSliderValue.text = new StringBuilder().Append("Music ").Append(Mathf.RoundToInt(AudioManager.MusicValue * 100f)).ToString();
            });
            _SFXSlider.onValueChanged.AddListener(toValue =>
            {
                AudioManager.SetSFXValue(toValue);
                _sfxSliderValue.text = new StringBuilder().Append("SFX ").Append(Mathf.RoundToInt(AudioManager.SFXValue * 100f)).ToString();
            });
            
            _musicSlider.value = AudioManager.MusicValue;
            _SFXSlider.value = AudioManager.SFXValue;
        }
        
        private void SetMenuActive(GameObject activeMenu)
        {
            foreach (var menu in _menuArray)
            {
                menu.SetActive(false);
            }
        
            activeMenu.SetActive(true);
        }

        #region Codex
        private void LoadChimericDatabase()
        {
            foreach (var chimeric in ChimericDatabase.Instance.Chimerics)
            {
                var codexIconObject = Instantiate(_codexIcon.gameObject, _contentGrid);
                codexIconObject.SetActive(true);
                var codexIcon = codexIconObject.GetComponent<CodexIcon>();
                codexIcon.SetMonsterData(chimeric, () => SelectChimericEntry(chimeric));
                _codexIcons.Add(codexIcon);
            }
        
            SelectChimericEntry(ChimericDatabase.Instance.Chimerics[0]);
        }

        private void SelectChimericEntry(Monster monster)
        {
            _chimericMain.sprite = monster.baseSprite;
            _chimericName.text = monster.name;
            _chimericElements.text = monster.monsterSubElement.element != ElementClass.MonsterElement.None
                ? $"{monster.monsterElement.element.ToString()}/{monster.monsterSubElement.element.ToString()}"
                : $"{monster.monsterElement.element.ToString()}";
            _chimericFlavorText.text = monster.monsterFlavourText;
        }

        private void ClearCodexIcons()
        {
            foreach (var icon in _codexIcons)
            {
                Destroy(icon.gameObject);
            }

            _codexIcons.Clear();
        }
        #endregion

        #region Scene Management
        private static void GoToScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    
        public void GoToSceneCoroutine(string sceneName)
        {
            StartCoroutine(TransitionScenes(sceneName));
        }

        private IEnumerator TransitionScenes(string sceneName)
        {
            _fadeAnimator.SetTrigger(Start);

            yield return new WaitForSeconds(.5f);

            GoToScene(sceneName);
        }
    
        private void QuitButtonClicked()
        {
            Application.Quit();
        }
        #endregion
    }
}