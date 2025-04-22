using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class StateController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _turnText;
    [SerializeField]
    private TMP_Text _remainsText;
    [SerializeField]
    private TMP_Text _timerText;
    [SerializeField]
    private TMP_Text _nameText;
    [SerializeField]
    private TMP_Text _stateText;
    [SerializeField]
    private Button _yesButton;
    [SerializeField]
    private Button _noButton;

    private byte _turn = 0;
    private byte _survivor = 0;
    private byte _criminal = 0;

    private bool _identity = Person.Citizen;
    private Person _person = null;

    private static readonly int MaxDay = byte.MaxValue / (int)GameManager.Cycle.End;

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetInteractable(false);
    }
#endif

    private void SetTurnText()
    {
        GameManager.Cycle cycle = (GameManager.Cycle)(_turn % (int)GameManager.Cycle.End);
        switch (cycle)
        {
            case GameManager.Cycle.Morning: //시간 증가, 시간 단축
                _yesButton.SetText(string.Format(Translation.Get(Translation.Letter.Increase), Translation.Get(Translation.Letter.Time)));
                _noButton.SetText(string.Format(Translation.Get(Translation.Letter.Decrease), Translation.Get(Translation.Letter.Time)));
                break;
            case GameManager.Cycle.Evening:
                _yesButton.SetText(null);
                _noButton.SetText(null);
                break;
            case GameManager.Cycle.Midday:  //찬성, 반대
                _yesButton.SetText(Translation.Get(Translation.Letter.Agree));
                _noButton.SetText(Translation.Get(Translation.Letter.Disagree));
                break;
        }
        Translation.Letter segment = (Translation.Letter)(cycle + (int)Translation.Letter.Evening);
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
        _remainsText.Set(stringBuilder.ToString());
    }

    private void SetStateText()
    {
        if (_person != null)
        {
            _nameText.Set(_person.name);
            StringBuilder stringBuilder = new StringBuilder();
            if (_person.owner == PhotonNetwork.NickName)
            {
                stringBuilder.Append(Translation.Get(Translation.Letter.Mine) + ":");
            }
            else
            {
                stringBuilder.Append(Translation.Get(Translation.Letter.Identity) + ":");
            }
            if (_person.alive == false)
            {
                switch (_person.identification)
                {
                    case Person.Citizen:
                        stringBuilder.Append(Translation.Get(Translation.Letter.Citizen) + " (" + Translation.Get(Translation.Letter.Dead) + ")");
                        break;
                    case Person.Criminal:
                        stringBuilder.Append(Translation.Get(Translation.Letter.Criminal) + " (" + Translation.Get(Translation.Letter.Dead) + ")");
                        break;
                }
            }
            else
            {
                if (_person.owner == PhotonNetwork.NickName || _identity == Person.Criminal)
                {
                    switch (_person.identification)
                    {
                        case Person.Citizen:
                            stringBuilder.Append(Translation.Get(Translation.Letter.Citizen) + " (" + Translation.Get(Translation.Letter.Alive) + ")");
                            break;
                        case Person.Criminal:
                            stringBuilder.Append(Translation.Get(Translation.Letter.Criminal) + " (" + Translation.Get(Translation.Letter.Alive) + ")");
                            break;
                    }
                }
                else
                {
                    stringBuilder.Append(Translation.Get(Translation.Letter.Unknown) + " (" + Translation.Get(Translation.Letter.Alive) + ")");
                }
            }
            _stateText.Set(stringBuilder.ToString());
        }
        else
        {
            _nameText.Set(null);
            _stateText.Set(null);
        }
    }

    private void SetInteractable(bool value)
    {
        _yesButton.SetInteractable(value);
        _noButton.SetInteractable(value);
    }

    public void Initialize(Action<bool> morningAction, Action<bool> middayAction)
    {
        _yesButton.SetListener(() =>
        {
            switch ((GameManager.Cycle)(_turn % (int)GameManager.Cycle.End))
            {
                case GameManager.Cycle.Morning:
                    if (morningAction != null)
                    {
                        morningAction.Invoke(true);
                        SetInteractable(false);
                    }
                    break;
                case GameManager.Cycle.Midday:
                    middayAction?.Invoke(true);
                    break;
            }
        });
        _noButton.SetListener(() =>
        {
            switch ((GameManager.Cycle)(_turn % (int)GameManager.Cycle.End))
            {
                case GameManager.Cycle.Morning:
                    if (morningAction != null)
                    {
                        morningAction.Invoke(false);
                        SetInteractable(false);
                    }
                    break;
                case GameManager.Cycle.Midday:
                    middayAction?.Invoke(false);
                    break;
            }
        });
    }

    public void UpdateState(double value)
    {
        _timerText.Set(value > 0 ? Mathf.Floor((float)value).ToString(): null);
        if(value <= 0)
        {
            SetInteractable(false);
        }
        SetStateText();
    }

    public void ChangeText()
    {
        SetTurnText();
        SetRemainingText();
        SetStateText();
    }

    public void ShowRemains((byte, byte) value)
    {
        _survivor = value.Item1;
        _criminal = value.Item2;
        SetRemainingText();
    }

    public void SelectPerson((Person, bool) info)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        _person = info.Item1;
        _identity = info.Item2;
    }

    public void OnRoomPropertiesUpdate(byte turn)
    {
        _turn = turn;
        switch ((GameManager.Cycle)(_turn % (int)GameManager.Cycle.End))
        {
            case GameManager.Cycle.Morning:
            case GameManager.Cycle.Midday:
                SetInteractable(true);
                break;
        }
        SetTurnText();
    }
}