using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Unity.VisualScripting;

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

    private Action _loadAction = null;

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
            Room room = PhotonNetwork.CurrentRoom;
            person.transform.parent = getTransform;
            person.SetListener(() => {
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
            if (room != null)
            {
                Hashtable hashtable = room.CustomProperties;
                if(hashtable.ContainsKey(RoomManager.MembersKey) == true && hashtable[RoomManager.MembersKey] != null)
                {
                    string[] list = hashtable[RoomManager.MembersKey].ToString().Split(" ");
                    int length = list != null ? list.Length : 0;
                    if (length > 0)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            if (_personList.Any(person => person.owner == list[i]) == false)
                            {
                                return;
                            }
                        }
                        _loadAction?.Invoke();
                    }
                }
            }
        }
    }

    public void Initialize(Action action)
    {
        _loadAction = action;
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
                if (form > (byte)Person.Form.Strider)
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
                player.SetCustomProperties(new Hashtable() { {RoomManager.PersonFormKey, null }, {RoomManager.IdentityKey, null}});
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
            int totalRows = Mathf.CeilToInt(list.Count / (float)PersonAlignmentCount);
            for (int i = 0; i < list.Count; i++)
            {
                int row = i / PersonAlignmentCount;
                float x = ((i % PersonAlignmentCount) - (((Mathf.Min(PersonAlignmentCount, list.Count - row * PersonAlignmentCount)) - 1) / 2f)) * PersonSpaceInterval.x; // X축 정렬 (가운데 기준)               
                float y = (row - ((totalRows - 1) / 2f)) * PersonSpaceInterval.y; // Y축 정렬 (가운데 기준)
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
                List<string> list = new List<string>(); //지목할 대상들 목록
                foreach (Person person in _personList)
                {
                    if (person != null)
                    {
                        person.SetButton(false);
                        if (person.alive == true)       //살아있어야 투표가 가능하다.
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
                            switch(cycle)
                            {
                                case GameManager.Cycle.Evening:
                                    if (dictionary.ContainsKey(person.name) == true && person.identification == Person.Criminal)
                                    {
                                        list.Add(dictionary[person.name]);
                                    }
                                    break;
                                case GameManager.Cycle.Morning:
                                    if (dictionary.ContainsKey(person.name) == true)
                                    {
                                        list.Add(dictionary[person.name]);
                                    }
                                    break;
                                case GameManager.Cycle.Midday:
                                    if (dictionary.ContainsKey(person.name) == true)
                                    {
                                        list.Add(dictionary[person.name]);
                                    }
                                    else
                                    {
                                        list.Add(null);
                                    }
                                    break;
                            }
                        }
                    }
                }
                List<IGrouping<string, string>> grouped = list.GroupBy(x => x).OrderByDescending(value => value.Count()).ToList();
                grouped = grouped.Where(value => value.Count() == grouped.First().Count()).ToList();
                if (cycle == GameManager.Cycle.Morning && grouped.Count != 1)//만약 아침인데 지목할 대상이 한 명으로 모이지 않았다면 바로 저녁으로
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
                                hashtable.Add(key, null);
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
                if (cycle == GameManager.Cycle.Morning && grouped.Count == 1)
                {
                    hashtable.Add(GameManager.TargetKey, grouped.First().Key);
                }
                if (criminalCount == 0)                     //시민 승리
                {
                    hashtable.Add(GameManager.EndKey, Person.Citizen);
                    hashtable.Add(RoomManager.MembersKey, null);
                }
                else if (citizenCount <= criminalCount)     //범인 승리
                {
                    hashtable.Add(GameManager.EndKey, Person.Criminal);
                    hashtable.Add(RoomManager.MembersKey, null);
                }
                else                                        //승패가 안 남
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
        bool dead = false;
        foreach (Person person in _personList) //선택을 할 수 있는 경우
        {
            if (person != null && person.owner == PhotonNetwork.NickName)
            {
                if(person.alive == false)
                {
                    dead = true;
                }
                else if (_pressable == false && (cycle == GameManager.Cycle.Morning || (cycle == GameManager.Cycle.Evening && person.identification == Person.Criminal)))
                {
                    _pressable = true;
                }
                break;
            }
        }
        if(_pressable == true || dead == true)
        {
            foreach (Person person in _personList)
            {
                if (person != null)
                {
                    person.SetLight(person.isVoted);
                }
            }
        }
    }

    public void OnRoomPropertiesUpdate(string target)
    {
        if (string.IsNullOrEmpty(target) == false)
        {
            foreach (Person person in _personList)
            {
                if (person != null)
                {
                    person.SetLight(person.name == target);
                }
            }
        }
    }

    public void OnRoomPropertiesUpdate(string key, object value)
    {
        bool dead = false; //죽은 플레이어는 관전 모드
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
                person.Vote(key, value);
                if (_pressable || dead)
                {
                    person.SetLight(person.isVoted);
                }
            }
        }
    }

    public (Person, bool) GetSelectInfo()
    {
        bool identity = Person.Citizen;
        Person person = null;
        if (Camera.main != null && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit) == true)
        {
            bool dead = false; //죽은 플레이어는 관전 모드
            for (int i = 0; i < _personList.Count; i++)
            {
                if (person != null && person.owner == PhotonNetwork.NickName && person.alive == false)
                {
                    dead = true;
                    break;
                }
            }
            for (int i = 0; i < _personList.Count; i++)
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
                        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())//ui 버튼 위를 클릭했다면 뒤의 콜라이더 클릭 무시
                        {
                            continue;
                        }
                        else if (_pressable == true && _personList[i].owner != PhotonNetwork.NickName && _personList[i].alive == true && dead == false)
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

    public IEnumerable<Person> GetPersons()
    {
        return _personList;
    }
}