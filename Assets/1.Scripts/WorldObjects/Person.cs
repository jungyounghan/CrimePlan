using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Person : MonoBehaviourPunCallbacks
{
    private bool _hasTransform = false;

    private Transform _transform = null;

    private Transform getTransform
    {
        get
        {
            if(_hasTransform == false)
            {
                _hasTransform = TryGetComponent(out _transform);
            }
            return _transform;
        }
    }

    private bool _hasAnimator = false;

    private Animator _animator = null;

    private Animator getAnimator
    {
        get
        {
            if(_hasAnimator == false)
            {
                _hasAnimator = TryGetComponent(out _animator);
            }
            return _animator;
        }
    }

    [SerializeField]
    private Light _spotLight;
    [SerializeField]
    private Transform _canvasTransform;
    [SerializeField]
    private TMP_Text _playerText;
    [SerializeField]
    private Button _selectButton;

    [SerializeField]
    private bool _identification = false;

    public bool identification {
        get
        {
            return _identification;
        }
    }

    public bool alive
    {
        get
        {
            return getAnimator.GetCurrentAnimatorStateInfo(0).IsName(FallingTag) == false;
        }
    }

    public bool isVoted
    {
        get
        {
            return _voterList.Count > 0;
        }
    }

    public string owner
    {
        get;
        private set;
    }

    private List<string> _voterList = new List<string>();

    private static readonly string FallingTag = "Falling";

    public enum Form : byte
    {
        Capper,
        Lady,
        Strider
    }

    public static readonly int FormCount = (int)Form.Strider + 1;

    public static event Action<Person> createAction = null;

    public const bool Citizen = false;
    public const bool Criminal = true;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (photonView.ViewID > 0)
        {
            Initialize(name, owner, _identification);
        }
    }
#endif

    private void Start()
    {
        _spotLight.SetActive(false);
        _selectButton.SetActive(false);
    }

    [PunRPC]
    private void Set(string name, string owner, bool identification)
    {
        this.name = name;
        this.owner = owner;
        _identification = identification;
        if(PhotonNetwork.NickName == owner)
        {
            _playerText.Set(this.name + "(" + Translation.Get(Translation.Letter.Mine) + ")");
        }
        else
        {
            _playerText.Set(this.name);
        }
        createAction?.Invoke(this);
    }

    [PunRPC]
    private void SetTrigger(string tag)
    {
        getAnimator.SetTrigger(tag);
    }

    [PunRPC]
    private void SetPose(string tag)
    {
        getAnimator.Play(tag, 0, 1f); //�ش� ����� ��� ���
    }

    public void Initialize(string name, string owner, bool identification)
    {
        Set(name, owner, identification);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC("Set", RpcTarget.Others, name, owner, identification);
        }
    }

    public void ChangeText()
    {
        if (PhotonNetwork.NickName == owner)
        {
            _playerText.Set(name + "(" + Translation.Get(Translation.Letter.Mine) + ")");
        }
        else
        {
            _playerText.Set(name);
        }
        _selectButton.SetText(Translation.Get(Translation.Letter.Select));
    }

    public void SetListener(UnityAction unityAction)
    {
        _selectButton.SetListener(unityAction);
    }

    public void SetButton(bool value)
    {
        _selectButton.SetActive(value);
    }

    public void Die()
    {
        SetTrigger(FallingTag);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC("SetTrigger", RpcTarget.Others, FallingTag);
            StartCoroutine(DoAnimationUntilDone());
            IEnumerator DoAnimationUntilDone()
            {
                yield return new WaitUntil(() => getAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(FallingTag));
                yield return new WaitWhile(() => getAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
                SetPose(FallingTag);
                photonView.RPC("SetPose", RpcTarget.Others, FallingTag);
            }
        }
    }

    public void Vote(string key, object value)
    {
        if(_voterList.Contains(key) == true)
        {
            _voterList.Remove(key);
        }
        if(value != null && name == value.ToString())
        {
            _voterList.Add(key);
        }
    }

    public void SetLight(bool value)
    {
        _spotLight.SetActive(value);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (_canvasTransform != null)
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                Transform transform = camera.transform;
                Vector3 position = getTransform.position;
                _canvasTransform.position = position + ((transform.position - position).normalized * Vector3.Distance(position, _canvasTransform.position));
                _canvasTransform.rotation = transform.rotation;
            }
        }
        if (PhotonNetwork.InRoom == true)
        {
            AnimatorStateInfo animatorStateInfo = getAnimator.GetCurrentAnimatorStateInfo(0);
            if(animatorStateInfo.shortNameHash == Animator.StringToHash(FallingTag) && animatorStateInfo.normalizedTime < 1f)
            {
                StartCoroutine(DoAnimationUntilDone());
                IEnumerator DoAnimationUntilDone()
                {
                    yield return new WaitWhile(() => getAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
                    SetPose(FallingTag);
                    photonView.RPC("SetPose", RpcTarget.Others, FallingTag);
                }
            }
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            photonView.RPC("Set", player, name, owner, _identification);
            if (alive == false)
            {
                photonView.RPC("SetPose", player, FallingTag);
            }
        }
    }
}