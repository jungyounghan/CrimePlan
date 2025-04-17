using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class Person : MonoBehaviourPunCallbacks
{
    public enum Form: byte
    {
        Capper,
        Lady,
        Strider
    }

    public static readonly int FormCount = (int)Form.Strider + 1;

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

    public static event Action<Person> createAction = null;

    private static readonly string FallingTag = "Falling";

    public const bool Citizen = false;
    public const bool Criminal = true;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (PhotonNetwork.InRoom == true)
        {
            Initialize(name, owner, _identification);
        }
    }
#endif

    [PunRPC]
    private void Set(string name, string owner, bool identification)
    {
        this.name = name;
        this.owner = owner;
        _identification = identification;
        Player player = PhotonNetwork.LocalPlayer;
        if(this.owner == player.NickName)
        {
            photonView.TransferOwnership(player);
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
        getAnimator.Play(tag, 0, 1f); //해당 포즈로 즉시 재생
    }

    public void Initialize(string name, string owner, bool identification)
    {
        Set(name, owner, identification);
        if (photonView.IsMine == true)
        {
            photonView.RPC("Set", RpcTarget.OthersBuffered, name, owner, identification);
        }
    }

    public void Kill()
    {
        SetTrigger(FallingTag);
        if (photonView.IsMine == true)
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
        if (photonView.IsMine == true)
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