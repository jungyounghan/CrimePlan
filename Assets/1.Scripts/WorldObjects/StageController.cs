using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

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

    public int columns = 3;
    public float cellWidth = 2f;
    public float cellHeight = 2f;
    public Vector3 startOffset = Vector3.zero;

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
            Debug.Log("생성");
            if (PhotonNetwork.LocalPlayer.UserId == person.name)
            {

            }
        }
    }

    public void Initialize(Action<bool> action)
    {
        _identityAction = action;
        if (PhotonNetwork.IsMasterClient == true)
        {
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            int count = 0;
            for (int i = 0; i < 5; i++)
            //foreach(Player player in players.Values)
            {
                int row = count / columns;
                int col = count % columns;

                Vector3 position = startOffset + new Vector3(col * cellWidth, 0, row * cellHeight);
                //GameObject gameObject = PhotonNetwork.InstantiateRoomObject(_personPrefabs[0].name, position, Quaternion.identity);
                //gameObject.GetComponent<Person>().Initialize();
                count++;
            }
        }
        else
        {
            Person.createAction += Create;
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