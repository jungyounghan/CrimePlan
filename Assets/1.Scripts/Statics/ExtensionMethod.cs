using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Cinemachine;

public static class ExtensionMethod
{
    public static void Set(this GameObject gameObject, bool value)
    {
        if(gameObject != null)
        {
            gameObject.SetActive(value);
        }
    }

    public static void SetActive(this Light light, bool value)
    {
        if(light != null)
        {
            light.gameObject.SetActive(value);
        }
    }

    public static void Set(this Light light, Color color, float intensity)
    {
        if (light != null)
        {
            light.color = color;
            light.intensity = intensity;
        }
    }

    public static void Set(this CinemachineVirtualCamera cinemachineVirtualCamera, Transform transform)
    {
        if (cinemachineVirtualCamera != null)
        {
            cinemachineVirtualCamera.LookAt = transform;
            cinemachineVirtualCamera.Follow = transform;
        }
    }

    public static void Set(this TMP_Text tmpText, string value)
    {
        if (tmpText != null)
        {
            tmpText.text = value;
        }
    }

    public static void SetText(this TMP_InputField tmpInputField, string value, bool placeholder = true)
    {
        if(tmpInputField != null)
        {
            if(placeholder == false)
            {
                tmpInputField.text = value;
            }
            else if(tmpInputField.placeholder is TextMeshProUGUI placeholderText)
            {
                placeholderText.text = value;
            }
        }
    }

    public static void SetInteractable(this TMP_InputField tmpInputField, bool value)
    {
        if(tmpInputField != null)
        {
            tmpInputField.interactable = value;
        }
    }

    public static string GetText(this TMP_InputField tmpInputField)
    {
        if (tmpInputField != null)
        {
            return tmpInputField.text;
        }
        return null;
    }

    public static void SetListener(this Slider slider, UnityAction<float> action)
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.AddListener(action);
        }
    }

    public static void SetActive(this Button button, bool value)
    {
        if (button != null)
        {
            button.gameObject.Set(value);
        }
    }

    public static void SetActive(this Button button, bool value, string text)
    {
        if (button != null)
        {
            button.gameObject.Set(value);
            TextMeshProUGUI[] tmpTexts = button.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI tmpText in tmpTexts)
            {
                tmpText.text = text;
            }
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

    public static void SetText(this Button button, string value, bool interactable)
    {
        if (button != null)
        {
            TextMeshProUGUI[] tmpTexts = button.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI tmpText in tmpTexts)
            {
                tmpText.text = value;
            }
            button.interactable = interactable;
        }
    }

    public static void SetInteractable(this Button button, bool value)
    {
        if(button != null)
        {
            button.interactable = value;
        }
    }

    public static void SetListener(this Button button, UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
        }
    }

    public static void SetListener(this Button button, UnityAction action, string value, bool active = true)
    {
        if (button != null)
        {
            TextMeshProUGUI[] tmpTexts = button.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI tmpText in tmpTexts)
            {
                tmpText.text = value;
            }
            switch (active)
            {
                case true:
                    if (button.gameObject.activeSelf == false)
                    {
                        button.gameObject.SetActive(true);
                    }
                    break;
                case false:
                    if (button.gameObject.activeSelf == true)
                    {
                        button.gameObject.SetActive(false);
                    }
                    break;
            }
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
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