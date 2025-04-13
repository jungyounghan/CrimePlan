using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;
using System;

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

    private Action<Transform> _watchAction = null;

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

    private void Awake()
    {
        Person.createAction += Create;
    }

    private void OnDestroy()
    {
        Person.createAction -= Create;
    }

    private void Create(Person person)
    {

    }

    public void Initialize(Action<Transform> watchAction)
    {
        _watchAction = watchAction;
        if (PhotonNetwork.IsMasterClient == true)
        {
            Debug.Log("방장");
        }
        else
        {
#if UNITY_EDITOR
            if(_personPrefabs[(int)_personForm] != null)
            {
                Person person = Instantiate(_personPrefabs[(int)_personForm], getTransform);
                _watchAction?.Invoke(person.transform);
            }
#endif
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

    public void OnRoomPropertiesUpdate(Hashtable hashtable)
    {
    }
}