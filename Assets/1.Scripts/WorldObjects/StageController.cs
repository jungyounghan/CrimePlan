using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

[DisallowMultipleComponent]
public class StageController : MonoBehaviour
{
    [SerializeField]
    private Person[] _personPrefabs = new Person[(int)Person.Form.End];

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
        int end = (int)Person.Form.End;
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
        _personForm = (Person.Form)Mathf.Clamp((int)_personForm, (int)Person.Form.Start, (int)Person.Form.End - 1);
    }
#endif

    public void Initialize()
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            Debug.Log("방장");
        }
        else
        {
#if UNITY_EDITOR
            if(_personForm >= Person.Form.Start && (int)_personForm < _personPrefabs.Length)
            {

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