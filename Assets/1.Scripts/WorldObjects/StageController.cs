using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
            if (_hasTransform == false)
            {
                _hasTransform = true;
                _transform = transform;
            }
            return _transform;
        }
    }

    [SerializeField]
    private bool _pressable = false;
    [SerializeField]
    private GameObject _spotLightObject;

    [SerializeField]
    private Person[] _personPrefabs = new Person[Person.FormCount];
    private List<Person> _personList = new List<Person>();
    private Action<IEnumerable<Person>> _personAction = null;

    private static readonly int PersonWidthAlignmentCount = 3;
    private static readonly string PersonPrefix = "Person";
    private static readonly Vector2 PersonSpaceInterval = new Vector2(-2.5f, 3f);
    private static readonly Vector3 PersonCenterPosition = new Vector3(0, 0, 3f);

#if UNITY_EDITOR
    [Header("�׽�Ʈ ���")]
    [SerializeField]
    private Person.Form _personForm = Person.Form.Capper;
    [SerializeField]
    private bool _identity = Person.Citizen;

    private void OnValidate()
    {
        _spotLightObject.Set(false);
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
            for (; citizenCount < criminalCount * 2 + 1; citizenCount++) //�ù� ���� �� �ù� ����
            {
                list.Add(((byte)UnityEngine.Random.Range(0, Person.FormCount), null, Person.Citizen));
            }
            for (; criminalCount < (citizenCount - 1) / 2; criminalCount++) //���� ���� �� ���� ����
            {
                list.Add(((byte)UnityEngine.Random.Range(0, Person.FormCount), null, Person.Criminal));
            }
            list = list.OrderBy(value => UnityEngine.Random.value).ToList();//AI�� ������ Ư������ ���ϵ��� ���� �����ֱ�
            int totalRows = Mathf.CeilToInt(list.Count / (float)PersonWidthAlignmentCount);
            for (int i = 0; i < list.Count; i++)
            {
                int row = i / PersonWidthAlignmentCount;
                float x = ((i % PersonWidthAlignmentCount) - (((Mathf.Min(PersonWidthAlignmentCount, list.Count - row * PersonWidthAlignmentCount)) - 1) / 2f)) * PersonSpaceInterval.x; // X�� ���� (��� ����)               
                float y = (row - ((totalRows - 1) / 2f)) * PersonSpaceInterval.y; // Y�� ���� (��� ����)
                GameObject gameObject = PhotonNetwork.InstantiateRoomObject(_personPrefabs[list[i].Item1].name, PersonCenterPosition + new Vector3(x, 0, y), Quaternion.identity);
                gameObject.transform.parent = getTransform;
                gameObject.GetComponent<Person>().Initialize(PersonPrefix + (i + 1).ToString(), list[i].Item2, list[i].Item3);
            }
        }
    }

    public void ChangeText()
    {
        foreach(Person person in _personList)
        {
            person?.ChangeText();
        }
    }

    public void UpdateTurn()
    {
        _pressable = false;
        if (PhotonNetwork.IsMasterClient == false)
        {
            foreach (Person person in _personList)
            {
                //person?.SetInteractable(false);
            }
        }
        else
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
                foreach (Person person in _personList)
                {
                    if (person != null)
                    {
                        if (person.alive == true)
                        {

                        }
                        //person.SetInteractable(false);
                    }
                }
                room.SetCustomProperties(new Hashtable() { { GameManager.TimeKey, PhotonNetwork.Time + GameManager.TimeLimitValue }, { GameManager.TurnKey, turn + 1 } });
            }
        }
    }

    public Person GetPerson()
    {
        Camera camera = Camera.main;
        if (camera != null && Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) == true)
        {
            foreach(Person person in _personList)
            {
                if (person != null && person.gameObject == hit.collider.gameObject)
                {
                    // UI ��ư ���� Ŭ���ߴٸ� ���� �ݶ��̴� Ŭ�� ����
                    if (EventSystem.current.IsPointerOverGameObject())
                    {
                        // UI�� Ŭ�������Ƿ� 3D ������Ʈ ó�� ��ŵ
                        Debug.Log("UI");
                    }
                    else
                    {
                        Debug.Log("ĸ��");
                    }
                    return person;
                }
            }
        }
        return null;
    }
}