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
    private static readonly string PersonPrefix = "Person";
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
            int citizenCount = 0;
            int criminalCount = 0;
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            List<(byte, string, bool)> list = new List<(byte, string, bool)>();
            foreach (Player player in players.Values)
            {
                Hashtable hashtable = player.CustomProperties;
                byte form = hashtable != null && hashtable.ContainsKey(RoomManager.PersonFormKey) && hashtable[RoomManager.PersonFormKey] != null && byte.TryParse(hashtable[RoomManager.PersonFormKey].ToString(), out form) ? form : (byte)UnityEngine.Random.Range(0, Person.FormCount);
                if (form > (int)Person.Form.Strider)
                {
                    form = (byte)Person.Form.Strider;
                }
                bool identity = hashtable != null && hashtable.ContainsKey(RoomManager.IdentityKey) && hashtable[RoomManager.IdentityKey] != null && bool.TryParse(hashtable[RoomManager.IdentityKey].ToString(), out identity) ? identity : Person.Citizen;
                switch (identity)
                {
                    case Person.Citizen:
                        citizenCount++;
                        break;
                    case Person.Criminal:
                        criminalCount++;
                        break;
                }
                list.Add((form, player.NickName, identity));
            }
            if (criminalCount == 0)
            {
                list.Add(((byte)UnityEngine.Random.Range(0, Person.FormCount), null, Person.Criminal));
                criminalCount++;
            }
            for (; citizenCount < criminalCount * 2 + 1; citizenCount++) //시민 부족 → 시민 보충
            {
                list.Add(((byte)UnityEngine.Random.Range(0, Person.FormCount), null, Person.Citizen));
            }
            for (; criminalCount < (citizenCount - 1) / 2; criminalCount++) //범인 부족 → 범인 보충
            {
                list.Add(((byte)UnityEngine.Random.Range(0, Person.FormCount), null, Person.Criminal));
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
                gameObject.GetComponent<Person>().Initialize(PersonPrefix + (i + 1).ToString(), list[i].Item2, list[i].Item3);
            }
        }
    }

    public void UpdateTurn()
    {
        Room room = PhotonNetwork.CurrentRoom;
        Hashtable hashtable = room != null ? room.CustomProperties : null;
        if (hashtable != null)
        {
            byte turn = 0;
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (string key in hashtable.Keys)
            {
                if (hashtable[key] != null)
                {
                    switch (key)
                    {
                        case GameManager.TurnKey:
                            byte.TryParse(hashtable[key].ToString(), out turn);
                            break;
                        case GameManager.TimeKey:
                            continue;
                        default:
                            dictionary[key] = hashtable[key].ToString();
                            break;
                    }
                }
            }
            GameManager.Cycle cycle = (GameManager.Cycle)(turn % (int)GameManager.Cycle.End);
            List<string> list = new List<string>();
            foreach (Person person in _personList)
            {
                if (person != null && person.alive == true)
                {
                    string name = person.name;
                    if (dictionary.ContainsKey(name) == true)
                    {
                        switch (cycle)
                        {
                            case GameManager.Cycle.Evening:
                                if (person.identification == Person.Criminal)
                                {
                                    list.Add(dictionary[name]);
                                }
                                break;
                            case GameManager.Cycle.Morning:
                            case GameManager.Cycle.Midday:
                                list.Add(dictionary[name]);
                                break;
                        }
                    }
                }
            }
            List<IGrouping<string, string>> grouped = list.GroupBy(x => x).OrderByDescending(value => value.Count()).ToList();
            grouped = grouped.Where(value => value.Count() == grouped.First().Count()).ToList();
            hashtable = new Hashtable();
            foreach (Person person in _personList)
            {
                if (person != null)
                {
                    string name = person.name;
                    if (name == PhotonNetwork.LocalPlayer.NickName)
                    {
                        if (person.alive == true)
                        {
                            switch (cycle)
                            {
                                case GameManager.Cycle.Evening:
                                    if(grouped.Count == 1)
                                    {

                                    }
                                    break;
                                case GameManager.Cycle.Morning:
                                    break;
                                case GameManager.Cycle.Midday:
                                    break;
                            }
                        }
                        if(dictionary.ContainsKey(name) == true && dictionary[name] != null)
                        {
                            hashtable.Add(name, null);
                        }
                    }
                }
            }
            if(PhotonNetwork.IsMasterClient == true)
            {

            }
            if (hashtable.Count > 0)
            {
                room.SetCustomProperties(hashtable);
            }
        }
    }

    public Person GetUpdatePerson(Camera camera)
    {
        if (camera != null && Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) == true)
        {
            Debug.Log(hit.collider.gameObject);
            foreach(Person person in _personList)
            {
                if(person != null && person.gameObject == hit.collider.gameObject)
                {
                    return person;
                }
            }
        }
        return null;
    }
}