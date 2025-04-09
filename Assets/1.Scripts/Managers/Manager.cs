using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public abstract class Manager<T> : MonoBehaviourPunCallbacks where T : MonoBehaviour
{
    private static T _instance = null;

    [Header("오디오 믹서"), SerializeField]
    private AudioMixer _audioMixer;

    private bool _hasAudioSource = false;

    private AudioSource _audioSource = null;

    private AudioSource getAudioSource
    {
        get
        {
            if (_hasAudioSource == false)
            {
                _hasAudioSource = TryGetComponent(out _audioSource);
            }
            return _audioSource;
        }
    }

    [Header("설정"), SerializeField]
    private Button _settingButton;
    [SerializeField]
    private TMP_Text _settingText;

    [Serializable]
    private struct Volume
    {
        [SerializeField]
        private TMP_Text tmpText;
        [SerializeField]
        private Slider slider;

        public void SetText(string value)
        {
            tmpText.Set(value);
        }

        public void SetListener(UnityAction<float> action)
        {
            slider.SetListener(action);
        }
    }

    [SerializeField]
    private Volume _masterVolume;
    [SerializeField]
    private Volume _effectVolume;
    [SerializeField]
    private Volume _backgroundVolume;

    [SerializeField]
    private TMP_Text _languageText;
    [SerializeField]
    private TMP_Text _closeText;

    [Header("알림 팝업"), SerializeField]
    private GameObject _noticeObject;
    [SerializeField]
    private TMP_Text _explainText;

    [SerializeField]
    private Button _yesButton;
    [SerializeField]
    private Button _noButton;

    private static readonly string MasterMixer = "Master";
    private static readonly string EffectMixer = "Effect";
    private static readonly string BackgroundMixer = "Background";
    protected static readonly string LanguageTag = "Language";

#if UNITY_EDITOR
    [Header("언어 변경"), SerializeField]
    private Translation.Language _language = Translation.Language.Korean;

    private void OnValidate()
    {
        if (gameObject.scene == SceneManager.GetActiveScene())
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
            }
            if (this == _instance)
            {
                PhotonNetwork.GameVersion = Application.version;
                ChangeText(_language);
                SetInteractable(false);
            }
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != _instance && this != null)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(this);
                }
            };
        }
    }
#endif

    private void Awake()
    {
        Initialize();
    }

    private void SetMasterVolume(float volume)
    {
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat(MasterMixer, volume);
        }
    }

    private void SetEffectVolume(float volume)
    {
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat(EffectMixer, volume);
        }
    }

    private void SetBackgroundVolume(float volume)
    {
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat(BackgroundMixer, volume);
        }
    }

    protected void SetExplain(string value)
    {
        _explainText.Set(value);
    }

    protected void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    protected void ShowPopup(UnityAction action)
    {
        _noticeObject.Set(true);
        _yesButton.SetListener(action, Translation.Get(Translation.Letter.Confirm));
        _noButton.SetListener(null, null, false);
    }

    protected void ShowPopup(UnityAction yesAction, UnityAction noAction)
    {
        _noticeObject.Set(true);
        _yesButton.SetListener(yesAction, Translation.Get(Translation.Letter.Yes));
        _noButton.SetListener(noAction, Translation.Get(Translation.Letter.No));
    }

    protected void ClosePopup()
    {
        _noticeObject.Set(false);
        _yesButton.SetListener(null, null, false);
        _noButton.SetListener(null, null, false);
    }

    public void SetLanguage(int index)
    {
        if (index >= byte.MinValue && index <= byte.MaxValue)
        {
            PlayerPrefs.SetInt(LanguageTag, index);
            ChangeText((Translation.Language)index);
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        getAudioSource.Stop();
        getAudioSource.clip = audioClip;
        getAudioSource.Play();
    }

    protected virtual void Initialize()
    {
        PhotonNetwork.GameVersion = Application.version;
        ChangeText((Translation.Language)PlayerPrefs.GetInt(LanguageTag));
        _masterVolume.SetListener(SetMasterVolume);
        _effectVolume.SetListener(SetEffectVolume);
        _backgroundVolume.SetListener(SetBackgroundVolume);
    }

    protected virtual void ChangeText(Translation.Language language)
    {
        Translation.Set(language);
        _settingText.Set(Translation.Get(Translation.Letter.Setting));
        string volume = Translation.Get(Translation.Letter.Volume);
        _masterVolume.SetText(Translation.Get(Translation.Letter.Master) + " " + volume);
        _effectVolume.SetText(Translation.Get(Translation.Letter.Effect) + " " + volume);
        _backgroundVolume.SetText(Translation.Get(Translation.Letter.Background) + " " + volume);
        _languageText.Set(Translation.Get(Translation.Letter.Language));
        _closeText.Set(Translation.Get(Translation.Letter.Close));
        if (_noButton == null || _noButton.gameObject.activeSelf == false)
        {
            _yesButton.SetText(Translation.Get(Translation.Letter.Confirm));
        }
        else
        {
            _noButton.SetText(Translation.Get(Translation.Letter.No));
            _yesButton.SetText(Translation.Get(Translation.Letter.Yes));
        }
    }

    protected virtual void SetInteractable(bool value)
    {
        _settingButton.SetInteractable(value);
    }
}