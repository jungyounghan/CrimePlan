using UnityEngine;
using Photon.Pun;

/// <summary>
/// 한 씬을 총괄하는 매니저
/// </summary>
[DisallowMultipleComponent]
public abstract class Manager : MonoBehaviourPunCallbacks
{
    private static readonly string LanguageValue = "Language";

    private static readonly string VersionValue = "1.0.0";

#if UNITY_EDITOR
    [SerializeField]
    private Translation.Language _language = Translation.Language.Korean;

    private void OnValidate()
    {
        //싱글턴으로 혹시 다른 개체가 있으면 삭제할 수 있는 요소를 추가하자
        PhotonNetwork.GameVersion = VersionValue;
        ChangeText(_language);
        SetInteractable(false);
    }
#endif

    protected virtual void Awake()
    {
        ChangeText((Translation.Language)PlayerPrefs.GetInt(LanguageValue));
    }

    protected abstract void SetInteractable(bool value);

    protected abstract void ChangeText(Translation.Language language);

    public void SetLanguage(int index)
    {
        if (index >= byte.MinValue && index <= byte.MaxValue)
        {
            PlayerPrefs.SetInt(LanguageValue, index);
            ChangeText((Translation.Language)index);
        }
    }
}