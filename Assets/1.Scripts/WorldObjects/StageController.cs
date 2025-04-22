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

    private bool _pressable = false;

    [SerializeField]
    private Light _solarLight;
    [SerializeField]
    private Person[] _personPrefabs = new Person[Person.FormCount];
    private List<Person> _personList = new List<Person>();

    private static readonly int PersonAlignmentCount = 3;
    private static readonly string PersonPrefix = "Person";
    private static readonly Vector2 PersonSpaceInterval = new Vector2(-2.5f, 3f);
    private static readonly Vector3 PersonCenterPosition = new Vector3(0, 0, 3f);
    private static readonly (Color, float)[] LightTypes = new (Color, float)[]{(new Color(0.5f, 0.55f, 0.7f), 0.6f), (new Color(0.8f, 0.9f, 1f), 1), (new Color(1f, 0.7f, 0.5f), 0.6f) };

#if UNITY_EDITOR
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
            person.SetListener(() => {
                Room room = PhotonNetwork.CurrentRoom;
                if(room != null)
                {
                    for(int i = 0; i < _personList.Count; i++)
                    {
                        if (_personList[i] != null && _personList[i].owner == PhotonNetwork.NickName)
                        {
                            string key = _personList[i].name;
                            string value = person.name;
                            Hashtable hashtable = room.CustomProperties;
                            if(hashtable != null && hashtable.ContainsKey(key) == true && hashtable[key] != null && hashtable[key].ToString() == value)
                            {
                                room.SetCustomProperties(new Hashtable() { { key, null } });
                            }
                            else
                            {
                                room.SetCustomProperties(new Hashtable() { { key, value } });
                            }
                        }
                    }
                }
            });
            _personList.Add(person);
        }
    }

    public void Initialize()
    {
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
                byte form = hashtable != null && hashtable.ContainsKey(RoomManager.PersonFormKey) && hashtable[RoomManager.PersonFormKey] != null && byte.TryParse(hashtable[RoomManager.PersonFormKey].ToString(), out form) ? form : (byte)Random.Range(0, Person.FormCount);
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
                list.Add(((byte)Random.Range(0, Person.FormCount), null, Person.Criminal));
                criminalCount++;
            }
            for (; citizenCount < criminalCount * 2 + 1; citizenCount++) //�ù� ���� �� �ù� ����
            {
                list.Add(((byte)Random.Range(0, Person.FormCount), null, Person.Citizen));
            }
            for (; criminalCount < (citizenCount - 1) / 2; criminalCount++) //���� ���� �� ���� ����
            {
                list.Add(((byte)Random.Range(0, Person.FormCount), null, Person.Criminal));
            }
            list = list.OrderBy(value => Random.value).ToList();//AI�� ������ Ư������ ���ϵ��� ���� �����ֱ�
            int totalRows = Mathf.CeilToInt(list.Count / (float)PersonAlignmentCount);
            for (int i = 0; i < list.Count; i++)
            {
                int row = i / PersonAlignmentCount;
                float x = ((i % PersonAlignmentCount) - (((Mathf.Min(PersonAlignmentCount, list.Count - row * PersonAlignmentCount)) - 1) / 2f)) * PersonSpaceInterval.x; // X�� ���� (��� ����)               
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

    public void SelectTarget(string target, bool agree)
    {
        if (string.IsNullOrEmpty(target) == false)
        {
            Room room = PhotonNetwork.CurrentRoom;
            if (room != null)
            {
                foreach (Person person in _personList)
                {
                    if(person.owner == PhotonNetwork.NickName)
                    {
                        if (agree == true)
                        {
                            room.SetCustomProperties(new Hashtable() { { person.name, target } });
                        }
                        else
                        {
                            room.SetCustomProperties(new Hashtable() { { person.name, null } });
                        }
                    }
                }
            }
        }
    }

    public void UpdateTurn()
    {
        _pressable = false;
        if (PhotonNetwork.IsMasterClient == false)
        {
            foreach (Person person in _personList)
            {
                person?.SetButton(false);
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
                int citizenCount = 0;
                int criminalCount = 0;
                List<string> list = new List<string>(); //������ ���� ���
                foreach (Person person in _personList)
                {
                    if (person != null)
                    {
                        person.SetButton(false);
                        if (person.alive == true) //����־�� ��ǥ�� �����ϴ�.
                        {
                            switch(person.identification)
                            {
                                case Person.Citizen:
                                    citizenCount++;
                                    break;
                                case Person.Criminal:
                                    criminalCount++;
                                    break;
                            }
                            string key = person.name;
                            if (dictionary.ContainsKey(key) == true) //�� �÷��̾ �����ϴ� ����� �ִٸ�
                            {
                                switch (cycle)
                                {
                                    case GameManager.Cycle.Evening:
                                        if (person.identification == Person.Criminal)
                                        {
                                            list.Add(dictionary[key]);
                                        }
                                        break;
                                    case GameManager.Cycle.Morning:
                                    case GameManager.Cycle.Midday:
                                        list.Add(dictionary[key]);
                                        break;
                                }
                            }
                        }
                    }
                }
                List<IGrouping<string, string>> grouped = list.GroupBy(x => x).OrderByDescending(value => value.Count()).ToList();
                grouped = grouped.Where(value => value.Count() == grouped.First().Count()).ToList();
                if (cycle == GameManager.Cycle.Morning && grouped.Count != 1)//���� ��ħ�ε� ������ ����� �� ������ ������ �ʾҴٸ� �ٷ� ��������
                {
                    if (turn + 2 == byte.MaxValue)
                    {
                        turn = 0;
                    }
                    else
                    {
                        turn += 2;
                    }
                }
                else
                {
                    if (turn + 1 == byte.MaxValue)
                    {
                        turn = 0;
                    }
                    else
                    {
                        turn += 1;
                    }
                }
                hashtable = new Hashtable() { { GameManager.TurnKey, turn } };
                if (cycle == GameManager.Cycle.Morning && grouped.Count == 1)
                {
                    hashtable.Add(GameManager.TargetKey, grouped.First().Key);
                }
                foreach (Person person in _personList)
                {
                    if (person != null)
                    {
                        string key = person.name;
                        switch (cycle)
                        {
                            case GameManager.Cycle.Midday:
                            case GameManager.Cycle.Evening:
                                if (grouped.Count == 1 && key == grouped.First().Key && person.alive == true)
                                {
                                    switch (person.identification)
                                    {
                                        case Person.Citizen:
                                            citizenCount--;
                                            break;
                                        case Person.Criminal:
                                            criminalCount--;
                                            break;
                                    }
                                    person.Die();
                                }
                                if(dictionary.ContainsKey(key) == true)
                                {
                                    hashtable.Add(key, null);
                                }
                                break;
                            case GameManager.Cycle.Morning:
                                if (dictionary.ContainsKey(key) == true && (grouped.Count != 1 || dictionary[key] != grouped.First().Key))
                                {
                                    hashtable.Add(key, null);
                                }
                                break;
                        }
                    }
                }
                if (criminalCount == 0)                     //�ù� �¸�
                {
                    hashtable.Add(GameManager.EndKey, Person.Citizen);
                    hashtable.Add(RoomManager.MembersKey, null);
                }
                else if (citizenCount <= criminalCount)     //���� �¸�
                {
                    hashtable.Add(GameManager.EndKey, Person.Criminal);
                    hashtable.Add(RoomManager.MembersKey, null);
                }
                else                                        //���а� �� ��
                {
                    switch((GameManager.Cycle)(turn % (int)GameManager.Cycle.End))
                    {
                        case GameManager.Cycle.Evening:
                        case GameManager.Cycle.Midday:
                            hashtable.Add(GameManager.TimeKey, PhotonNetwork.Time + GameManager.TimeLimitValue);
                            break;
                        case GameManager.Cycle.Morning:
                            hashtable.Add(GameManager.TimeKey, PhotonNetwork.Time + GameManager.TimeLimitValue * (byte)_personList.Count(x => x.owner != null));
                            break;
                    }
                }
                room.SetCustomProperties(hashtable);
            }
        }
    }

    public void OnRoomPropertiesUpdate(byte turn)
    {
        GameManager.Cycle cycle = (GameManager.Cycle)(turn % (int)GameManager.Cycle.End);
        _solarLight.Set(LightTypes[(int)cycle].Item1, LightTypes[(int)cycle].Item2);
        foreach (Person person in _personList) //������ �� �� �ִ� ���
        {
            if (person != null)
            {
                person.SetLight(false);
                if (_pressable == false && person.owner == PhotonNetwork.NickName && person.alive == true && (cycle == GameManager.Cycle.Morning || (cycle == GameManager.Cycle.Evening && person.identification == Person.Criminal)))
                {
                    _pressable = true;
                }
            }
        }
    }

    public void OnRoomPropertiesUpdate(string target)
    {
        foreach (Person person in _personList)
        {
            if (person != null && person.name == target)
            {
                person.SetLight(true);
            }
        }
    }

    public void OnRoomPropertiesUpdate(string key, object value)
    {
        bool dead = false; //���� �÷��̾�� ���� ���
        foreach (Person person in _personList)
        {
            if(person != null && person.owner == PhotonNetwork.NickName && person.alive == false)
            {
                dead = true;
                break;
            }
        }
        foreach (Person person in _personList)
        {
            if (person != null)
            {
                if (_pressable == true)
                {
                    person.Remove(key);
                }
                if ((_pressable == true || dead == true) && value != null && person.name == value.ToString())
                {
                    person.Add(key);
                }
            }
        }
    }

    public (byte, byte) GetRemainsCount()
    {
        byte survivor = 0;
        byte criminal = 0;
        foreach (Person person in _personList)
        {
            if (person != null && person.alive == true)
            {
                survivor++;
                if (person.identification == Person.Criminal)
                {
                    criminal++;
                }
            }
        }
        return (survivor, criminal);
    }

    public (Person, bool) GetSelectInfo()
    {
        bool identity = Person.Citizen;
        Person person = null;
        if (Camera.main != null && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) == true)
        {
            for(int i = 0; i < _personList.Count; i++)
            {
                if (_personList[i] != null)
                {
                    if(_personList[i].owner == PhotonNetwork.NickName && _personList[i].identification == Person.Criminal)
                    {
                        identity = Person.Criminal;
                    }
                    if(_personList[i].gameObject == hit.collider.gameObject)
                    {
                        person = _personList[i];
                        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())//ui ��ư ���� Ŭ���ߴٸ� ���� �ݶ��̴� Ŭ�� ����
                        {
                            continue;
                        }
                        else if (_pressable == true && _personList[i].owner != PhotonNetwork.NickName)
                        {
                            _personList[i].SetButton(true);
                        }
                    }
                    else
                    {
                        _personList[i].SetButton(false);
                    }
                }
            }
        }
        return (person, identity);
    }
}