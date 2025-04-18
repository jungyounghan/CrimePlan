using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

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
            return getAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash(FallingTag);
        }
    }

    public string owner
    {
        get;
        private set;
    }

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

    [PunRPC]
    private void Set(string name, string owner, bool identification)
    {
        this.name = name;
        _playerText.Set(this.name);
        this.owner = owner;
        _identification = identification;
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
        getAnimator.Play(tag, 0, 1f); //해당 포즈로 즉시 재생
    }

    public void Initialize(string name, string owner, bool identification)
    {
        Set(name, owner, identification);
        if (PhotonNetwork.InRoom == true)
        {
            photonView.RPC("Set", RpcTarget.OthersBuffered, name, owner, identification);
        }
    }

    public void ChangeText()
    {
        _selectButton.SetText(Translation.Get(Translation.Letter.Select));
    }

    public void SetInteractable(bool value)
    {
        _selectButton.SetInteractable(value);
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
                photonView.RPC("SetPose", RpcTarget.OthersBuffered, FallingTag);
            }
        }
    }

    public override void OnEnable()
    {
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
                    photonView.RPC("SetPose", RpcTarget.OthersBuffered, FallingTag);
                }
            }
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }
}