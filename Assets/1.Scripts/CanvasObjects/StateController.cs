using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class StateController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _turnText;
    [SerializeField]
    private TMP_Text _energyText;
    [SerializeField]
    private TMP_Text _timerText;
    [SerializeField]
    private Slider _energySlider;

    private byte _turn = 0;

    public static readonly int DaySegmentValue = 3;

    public void Initialize()
    {

    }

    public void Set(bool identity)
    {
        Debug.Log(identity);
    }

    public void UpdateTime(double value)
    {
        _timerText.Set(value > 0 ? Mathf.Floor((float)value).ToString(): null);
    }

    public void ChangeText()
    {
        int day = _turn / DaySegmentValue;
        Translation.Letter segment = (Translation.Letter)((_turn % DaySegmentValue) + (int)Translation.Letter.Morning);
        _turnText.Set(string.Format(Translation.Get(Translation.Letter.Day), day.ToString("D2")) + " " + Translation.Get(segment));
    }

    public void OnRoomPropertiesUpdate(byte turn)
    {
        _turn = turn;
        ChangeText();
    }
}