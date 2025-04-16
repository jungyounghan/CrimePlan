using System;
using System.Linq;
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

    private List<Person> _personList = new List<Person>();

    private Action<IEnumerable<Person>> _personAction = null;

    private static readonly int PersonWidthAlignmentCount = 3;
    private static readonly string PersonBotTag = "PersonBot";
    private static readonly Vector2 PersonSpaceInterval = new Vector2(2.5f, 3f);

#if UNITY_EDITOR
    [Header("테스트 모드")]
    [SerializeField]
    private Person.Form _personForm = Person.Form.Capper;
    [SerializeField]
    private bool _identity = Person.Citizen;

    private void OnValidate()
    {
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
        if (person != null)
        {
            person.transform.parent = getTransform;
            _personList.Add(person);
            _personAction?.Invoke(_personList);
        }
    }

    public void Initialize(Action<IEnumerable<Person>> action)
    {
        _personAction = action;
        Person.createAction += Create;
        if(PhotonNetwork.IsMasterClient == true && _personPrefabs.Length == Person.FormCount && _personPrefabs[((int)Person.Form.Capper)] != null && _personPrefabs[((int)Person.Form.Lady)] != null && _personPrefabs[((int)Person.Form.Strider)] != null)
        {
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            List<(byte, string, bool)> list = new List<(byte, string, bool)>();
            int citizenCount = 0;
            int criminalCount = 0;
            foreach (Player player in players.Values)
            {
                Hashtable hashtable = player.CustomProperties;
                byte form = hashtable != null && hashtable.ContainsKey(RoomManager.PersonFormKey) && hashtable[RoomManager.PersonFormKey] != null && byte.TryParse(hashtable[RoomManager.PersonFormKey].ToString(), out form) ? form : (byte)UnityEngine.Random.Range(0, Person.FormCount);
                if (form > (int)Person.Form.Strider)
                {
                    form = (byte)Person.Form.Strider;
                }
                bool identity = hashtable != null && hashtable.ContainsKey(RoomManager.IdentityKey) && hashtable[RoomManager.IdentityKey] != null && bool.TryParse(hashtable[RoomManager.IdentityKey].ToString(), out identity) ? identity : Person.Citizen;
                switch(identity)
                {
                    case Person.Citizen:
                        citizenCount++;
                        break;
                    case Person.Criminal:
                        criminalCount++;
                        break;
                }
                list.Add((form, player.UserId, identity));
            }
            int botIndex = 0;
            if(criminalCount == 0)
            {
                list.Add(((byte)UnityEngine.Random.Range(0, Person.FormCount), PersonBotTag + botIndex++, Person.Criminal));
                criminalCount++;
            }
            for (int i = citizenCount; i < criminalCount * 2 + 1; i++) //시민 부족 → 시민 보충
            {
                list.Add(((byte)UnityEngine.Random.Range(0, Person.FormCount), PersonBotTag + botIndex++, Person.Citizen));
            }
            for (int i = criminalCount; i < (citizenCount - 1) / 2; i++) //범인 부족 → 범인 보충
            {
                list.Add(((byte)UnityEngine.Random.Range(0, Person.FormCount), PersonBotTag + botIndex++, Person.Criminal));
            }
            list = list.OrderBy(value => UnityEngine.Random.value).ToList();//AI와 유저를 특정하지 못하도록 순서 섞어주기
            int totalRows = Mathf.CeilToInt(list.Count / (float)PersonWidthAlignmentCount);
            for (int i = 0; i < list.Count; i++)
            {
                int row = i / PersonWidthAlignmentCount;                
                float x = ((i % PersonWidthAlignmentCount) - (((Mathf.Min(PersonWidthAlignmentCount, list.Count - row * PersonWidthAlignmentCount)) - 1) / 2f)) * PersonSpaceInterval.x; // X축 정렬 (가운데 기준)               
                float y = (row - ((totalRows - 1) / 2f)) * PersonSpaceInterval.y; // Y축 정렬 (가운데 기준)
                GameObject gameObject = PhotonNetwork.InstantiateRoomObject(_personPrefabs[list[i].Item1].name, new Vector3(x, 0, y), Quaternion.identity);
                gameObject.transform.parent = getTransform;
                Person person = gameObject.GetComponent<Person>();
                person.Initialize(list[i].Item2, list[i].Item3);
            }
        }
    }

    public void UpdateInput(Camera camera)
    {
        if (camera != null && Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) == true)
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