using System.Text;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class StateController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _turnText;
    [SerializeField]
    private TMP_Text _personText;
    [SerializeField]
    private TMP_Text _timerText;

    private byte _turn = 0;
    private byte _survivor = 0;
    private byte _criminal = 0;

    private static readonly int MaxDay = byte.MaxValue / (int)GameManager.Cycle.End;

    private void SetTurnText()
    {
        Translation.Letter segment = (Translation.Letter)((_turn % (int)GameManager.Cycle.End) + (int)Translation.Letter.Evening);
        int day = (_turn + 2) / (int)GameManager.Cycle.End;
        if (day >= MaxDay)
        {
            day = 0;
        }
        _turnText.Set(string.Format(Translation.Get(Translation.Letter.Day), day.ToString("D2")) + " " + Translation.Get(segment));
    }

    private void SetRemainingText()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(string.Format(Translation.Get(Translation.Letter.Remaining), Translation.Get(Translation.Letter.Survivor)) + ":" + _survivor + "\t");
        stringBuilder.Append(string.Format(Translation.Get(Translation.Letter.Remaining), Translation.Get(Translation.Letter.Criminal)) + ":" + _criminal);
        _personText.Set(stringBuilder.ToString());
    }

    public void UpdateTime(double value)
    {
        _timerText.Set(value > 0 ? Mathf.Floor((float)value).ToString(): null);
    }

    public void ChangeText()
    {
        SetTurnText();
        SetRemainingText();
    }

    public void ShowRemains(IEnumerable<Person> persons)
    {
        _survivor = 0;
        _criminal = 0;
        if (persons != null)
        {
            foreach (Person person in persons)
            {
                if (person != null && person.alive == true)
                {
                    _survivor++;
                    if (person.identification == Person.Criminal)
                    {
                        _criminal++;
                    }
                }
            }
        }
        SetRemainingText();
    }

    public void UpdateSelect(Person person)
    {
        if(person != null)
        {

        }
        else
        {
            //¼û±â±â
        }
    }

    public void OnRoomPropertiesUpdate(byte turn)
    {
        _turn = turn;
        SetTurnText();
    }

    public void OnRoomPropertiesUpdate(object data)
    {

    }
}