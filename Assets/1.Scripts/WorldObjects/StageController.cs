using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UIElements;
using System.Linq;

[DisallowMultipleComponent]
public class StageController : MonoBehaviour
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform {
        get
        {
            if(_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    [SerializeField]
    private Person[] _personPrefabs = new Person[Person.FormCount];

    private Dictionary<string, Person> _personDictionary = new Dictionary<string, Person>();

    private Action<bool> _identityAction = null;

    private static readonly int PersonSpacing = 3;

#if UNITY_EDITOR
    [Header("테스트 모드")]
    [SerializeField]
    private Person.Form _personForm = Person.Form.Capper;
    [SerializeField]
    private bool _identity = Person.Citizen;
    [SerializeField]
    private uint _citizenCount = 1;
    [SerializeField]
    private uint _policeCount = 0;
    [SerializeField]
    private uint _criminalCount = 0;

    private void OnValidate()
    {
        if (_citizenCount == 0)
        {
            _citizenCount = 1;
        }
        switch(_identity)
        {
            case Person.Citizen:
                if (_criminalCount == 0)
                {
                    _criminalCount = 1;
                }
                break;
            case Person.Criminal:
                if (_policeCount == 0)
                {
                    _policeCount = 1;
                }
                break;
        }
        int length = _personPrefabs.Length;
        int end = Person.FormCount;
        if (length > end)
        {
            _personPrefabs = new Person[] { _personPrefabs[((int)Person.Form.Capper)], _personPrefabs[((int)Person.Form.Lady)], _personPrefabs[((int)Person.Form.Strider)] };
        }
        else
        {
            if(length < end)
            {
                Person[] persons = new Person[end];
                for(int i = 0; i < length; i++)
                {
                    persons[i] = _personPrefabs[i];
                }
                _personPrefabs = persons;
            }
            ExtensionMethod.Sort(ref _personPrefabs);
        }
    }
#endif

    private void OnDestroy()
    {
        Person.createAction -= Create;
    }

    private void Create(Person person)
    {
        if(person != null)
        {
            _personDictionary[person.name] = person;
            _personDictionary[person.name].transform.parent = getTransform;
            if (PhotonNetwork.LocalPlayer.UserId == person.name)
            {

            }
        }
    }

    public void Initialize(Action<bool> action)
    {
        _identityAction = action;
        if (PhotonNetwork.IsMasterClient == false)
        {
            Person.createAction += Create;
        }
        else if(_personPrefabs.Length == Person.FormCount && _personPrefabs[((int)Person.Form.Capper)] != null && _personPrefabs[((int)Person.Form.Lady)] != null && _personPrefabs[((int)Person.Form.Strider)] != null)
        {
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            List<(string, byte, bool)> list = new List<(string, byte, bool)>();
            int citizenCount = 0;
            int criminalCount = 0;
            foreach (Player player in players.Values)
            {
                Hashtable hashtable = player.CustomProperties;
                byte form = hashtable != null && hashtable.ContainsKey(RoomManager.PersonFormKey) && hashtable[RoomManager.PersonFormKey] != null && byte.TryParse(hashtable[RoomManager.PersonFormKey].ToString(), out form) ? form : (byte)0;
                if (form > (int)Person.Form.Strider)
                {
                    form = (byte)Person.Form.Strider;
                }
                bool identity = hashtable != null && hashtable.ContainsKey(RoomManager.IdentityKey) && hashtable[RoomManager.IdentityKey] != null && bool.TryParse(hashtable[RoomManager.IdentityKey].ToString(), out identity) ? identity : false;
                switch(identity)
                {
                    case Person.Citizen:
                        citizenCount++;
                        break;
                    case Person.Criminal:
                        criminalCount++;
                        break;
                }
                list.Add((player.UserId, form, identity));
            }
            for(int i = citizenCount; i < criminalCount * 2 + 1; i++)
            {
                list.Add((null, (byte)UnityEngine.Random.Range(0, Person.FormCount), false));
            }
            for (int i = criminalCount; i < (citizenCount - 1) / 2; i++)
            {
                // 마피아 추가 로직
            }   
        }
    }

    public void UpdateInput(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit) == true)
        {
            //var comp = hit.collider.GetComponent<MyComponent>();
            //if (comp != null)
            //{
            //    comp.OnClicked();
            //}
        }
    }

    public void UpdateMove()
    {
        //플레이어 애니메이션의 변화가 생기면 콜백으로 GameManager에 호출하기
    }
}