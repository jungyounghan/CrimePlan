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

    [Header("설정"), SerializeField]
    private Button _settingButton;

    private bool _hasAudioSource = false;

    private AudioSource _audioSource = null;

    private AudioSource getAudioSource {
        get
        {
            if(_hasAudioSource == false)
            {
                _hasAudioSource = TryGetComponent(out _audioSource);
            }
            return _audioSource;
        }
    }

    [Header("오디오 믹서"), SerializeField]
    private AudioMixer _audioMixer;
    [Header("음량"), SerializeField]
    private TMP_Text _volumeText;

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

    [Header("언어"), SerializeField]
    private TMP_Text _languageText;
    [SerializeField]
    private TMP_Text _closeText;

    [Header("종료"), SerializeField]
    private GameObject _quitObject;
    [SerializeField]
    private TMP_Text _quitText;
    [SerializeField]
    private TMP_Text _yesText;
    [SerializeField]
    private TMP_Text _noText;

    protected Coroutine _coroutine = null;

    private static readonly string MasterMixer = "Master";
    private static readonly string EffectMixer = "Effect";
    private static readonly string BackgroundMixer = "Background";
    protected static readonly string LanguageTag = "Language";
    protected static readonly string UsersTag = "Users";

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
        PhotonNetwork.GameVersion = Application.version;
        ChangeText((Translation.Language)PlayerPrefs.GetInt(LanguageTag));
        _masterVolume.SetListener(SetMasterVolume);
        _effectVolume.SetListener(SetEffectVolume);
        _backgroundVolume.SetListener(SetBackgroundVolume);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _quitObject != null)
        {
            _quitObject.SetActive(!_quitObject.activeInHierarchy);
        }
    }

    private void OnApplicationQuit()
    {
        Authentication.SignOut();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }

    protected virtual void ChangeText(Translation.Language language)
    {
        _volumeText.Set(Translation.GetVolume(language));
        _masterVolume.SetText(Translation.GetMasterVolume(language));
        _effectVolume.SetText(Translation.GetEffectVolume(language));
        _backgroundVolume.SetText(Translation.GetBackgroundVolume(language));
        _languageText.Set(Translation.GetLanguage(language));
        _closeText.Set(Translation.GetClose(language));
        _quitText.Set(Translation.GetQuit(language));
        _yesText.Set(Translation.GetYes(language));
        _noText.Set(Translation.GetNo(language));
    }

    protected virtual void SetInteractable(bool value)
    {
        _settingButton.SetInteractable(value);
    }

    public abstract void Quit();

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
        getAudioSource.clip = audioClip;
        getAudioSource.Play();
    }
}