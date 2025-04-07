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

    private bool _hasAudioSource = false;

    private AudioSource _audioSource = null;

    private AudioSource getAudioSource {
        get
        {
            if(_hasAudioSource == false)
            {

            }
            return _audioSource;
        }
    }

    [Header("오디오 믹서"), SerializeField]
    private AudioMixer _audioMixer;

    [Header("음량 텍스트"), SerializeField]
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

    [Header("전체 음량 설정"), SerializeField]
    private Volume _masterVolume;
    [Header("효과음 설정"), SerializeField]
    private Volume _effectVolume;
    [Header("배경음 설정"), SerializeField]
    private Volume _backgroundVolume;

    private static readonly string MasterMixer = "Master";
    private static readonly string EffectMixer = "Effect";
    private static readonly string BackgroundMixer = "Background";
    private static readonly string LanguageValue = "Language";

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

    protected virtual void Awake()
    {
        ChangeText((Translation.Language)PlayerPrefs.GetInt(LanguageValue));
        _masterVolume.SetListener(SetMasterVolume);
        _effectVolume.SetListener(SetEffectVolume);
        _backgroundVolume.SetListener(SetBackgroundVolume);
    }

    protected virtual void ChangeText(Translation.Language language)
    {
        _volumeText.Set(Translation.GetVolume(language));
        _masterVolume.SetText(Translation.GetMasterVolume(language));
        _effectVolume.SetText(Translation.GetEffectVolume(language));
        _backgroundVolume.SetText(Translation.GetBackgroundVolume(language));
    }

    protected abstract void SetInteractable(bool value);

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
            PlayerPrefs.SetInt(LanguageValue, index);
            ChangeText((Translation.Language)index);
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        getAudioSource.clip = audioClip;
        getAudioSource.Play();
    }
}