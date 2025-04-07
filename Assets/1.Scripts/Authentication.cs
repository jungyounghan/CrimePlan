using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Net;
using UnityEngine.UI;

public class Authentication : MonoBehaviour
{
    private DatabaseReference databaseReference;
    private FirebaseUser firebaseUser;

    public InputField moneyField;
    public InputField experienceField;
    public InputField levelField;

    private List<string> list = new List<string>();
    private Dictionary<string, int> dictionary = new Dictionary<string, int>();

    public FirebaseAuth authentication;                 //���� ������ ���� ����

    private void Awake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                authentication = FirebaseAuth.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.Log("���̾�̽� ����");
            }
        });
    }

    private void SaveInventory()
    {
        databaseReference.Child("users").Child(firebaseUser.UserId).Child("list").SetValueAsync(list);
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        foreach (var kvp in this.dictionary)
        {
            dictionary[kvp.Key] = kvp.Value;
        }
        databaseReference.Child("users").Child(firebaseUser.UserId).Child("dictionary").SetValueAsync(dictionary);
    }

    public void SaveToDataBase()
    {
        if (int.TryParse(moneyField.text, out int money) && int.TryParse(experienceField.text, out int experience) && int.TryParse(levelField.text, out int level))
        {
            StartCoroutine(UpdateMoney(money));
            StartCoroutine(UpdateExperience(experience));
            StartCoroutine(UpdateLevel(level));
        }
        SaveInventory();
    }

    IEnumerator UpdateMoney(int money)
    {
        var task = databaseReference.Child("users").Child(firebaseUser.UserId).Child("money").SetValueAsync(money);
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.Log("�� ���ε� ����: ����:" + task.Exception);
        }
        else
        {
            //�� 
        }
    }

    IEnumerator UpdateExperience(int experience)
    {
        var task = databaseReference.Child("users").Child(firebaseUser.UserId).Child("exp").SetValueAsync(experience);
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.Log("����ġ ���ε� ����: ����:" + task.Exception);
        }
        else
        {
            //����ġ 
        }
    }

    IEnumerator UpdateLevel(int level)
    {
        var task = databaseReference.Child("users").Child(firebaseUser.UserId).Child("lvl").SetValueAsync(level);
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Exception != null)
        {
            Debug.Log("���� ���ε� ����: ����:" + task.Exception);
        }
        else
        {
            //���� 
        }
    }

    public void LoadFromDataBase()
    {
        StartCoroutine(LoadUserData());
        StartCoroutine(LoadListData());
        StartCoroutine(LoadDictionaryData());
        IEnumerator LoadUserData()
        {
            var task = databaseReference.Child("users").Child(firebaseUser.UserId).GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.Exception != null)
            {
                Debug.LogWarning("������ �ҷ����� ����" + task.Exception);
            }
            else if (task.Result.Value == null)
            {
                Debug.LogWarning("����� �����Ͱ� �����ϴ�");
            }
            else
            {
                DataSnapshot snapShot = task.Result;
                moneyField.text = snapShot.Child("money").Exists ? snapShot.Child("money").Value.ToString() : "0";
                experienceField.text = snapShot.Child("exp").Exists ? snapShot.Child("exp").Value.ToString() : "0";
                levelField.text = snapShot.Child("lvl").Exists ? snapShot.Child("lvl").Value.ToString() : "1";
                Debug.Log("�ε� ����");
            }
        }
        IEnumerator LoadListData()
        {
            var task = databaseReference.Child("users").Child(firebaseUser.UserId).Child("list").GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.Exception != null)
            {
                Debug.Log("�κ��丮 �ҷ����� ����");
            }
            else if (task.Result.Value == null)
            {
                Debug.Log("�κ��丮�� ������ϴ�.");
            }
            else
            {
                list.Clear();
                foreach (DataSnapshot item in task.Result.Children)
                {
                    list.Add(item.Value.ToString());
                }
                Debug.Log("�ҷ��� �κ��丮:" + string.Join(",", list));
            }
        }
        IEnumerator LoadDictionaryData()
        {
            var task = databaseReference.Child("users").Child(firebaseUser.UserId).Child("dictionary").GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.Exception != null)
            {
                Debug.Log("�κ��丮 �ҷ����� ����");
            }
            else if (task.Result.Value == null)
            {
                Debug.Log("�κ��丮�� ������ϴ�.");
            }
            else
            {
                dictionary.Clear();
                foreach (DataSnapshot item in task.Result.Children)
                {
                    string key = item.Key;
                    if (int.TryParse(item.Value.ToString(), out int value))
                    {
                        dictionary[key] = value;
                    }
                    else
                    {
                        Debug.LogWarning("�񤧼ųʸ� ������ ����");
                    }
                }
                Debug.Log("�ҷ��� �κ��丮:" + string.Join(",", dictionary));
            }
        }
    }
}
