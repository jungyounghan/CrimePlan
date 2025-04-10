using System;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public class PlayerProfile : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _timerText;

    public void SetTimer(double value)
    {
        Debug.Log(value);
        _timerText.Set(value > 0 ? Math.Floor(value).ToString(): null);
    }
}