using UnityEngine;
using Photon.Pun;

/// <summary>
/// �� ���� �Ѱ��ϴ� �Ŵ���
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
        //�̱������� Ȥ�� �ٸ� ��ü�� ������ ������ �� �ִ� ��Ҹ� �߰�����
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