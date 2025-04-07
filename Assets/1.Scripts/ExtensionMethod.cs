using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class ExtensionMethod
{
    public static void Set(this TMP_Text tmpText, string value)
    {
        if (tmpText != null)
        {
            tmpText.text = value;
        }
    }

    public static void SetText(this TMP_InputField tmpInputField, string value)
    {
        if(tmpInputField != null && tmpInputField.placeholder is TextMeshProUGUI placeholderText)
        {
            placeholderText.text = value;
        }
    }

    public static void SetInteractable(this TMP_InputField tmpInputField, bool value)
    {
        if(tmpInputField != null)
        {
            tmpInputField.interactable = value;
        }
    }

    public static void SetText(this Button button, string value)
    {
        if (button != null)
        {
            TextMeshProUGUI[] tmpTexts = button.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI tmpText in tmpTexts)
            {
                tmpText.text = value;
            }
        }
    }

    public static void SetInteractable(this Button button, bool value)
    {
        if(button != null)
        {
            button.interactable = value;
        }
    }

    public static void Sort<T>(ref T[] array) where T : Object
    {
        List<T> list = new List<T>();
        int empty = 0;
        int length = array != null ? array.Length : 0;
        for (int i = 0; i < length; i++)
        {
            T value = array[i];
            if (value != null)
            {
                if (list.Contains(value) == false)
                {
                    list.Add(value);
                }
                else
                {
                    empty++;
                }
            }
            else
            {
                empty++;
            }
        }
        for (int i = 0; i < empty; i++)
        {
            list.Add(null);
        }
        array = list.ToArray();
    }
}
