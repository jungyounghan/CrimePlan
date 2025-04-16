using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
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
            return getAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash(FallingTag);
        }
    }

    public static event Action<Person> createAction = null;

    private static readonly string FallingTag = "Falling";

    public const bool Citizen = false;
    public const bool Criminal = true;

#if UNITY_EDITOR
    private void OnValidate()
    {
        Set(name, _identification);
    }
#endif

    private void Awake()
    {
        createAction?.Invoke(this);
    }

    [PunRPC]
    private void Set(string name, bool identification)
    {
        this.name = name;
        _identification = identification;
    }

    [PunRPC]
    private void SetTrigger(string tag)
    {
        Debug.Log("SetTrigger");
        getAnimator.SetTrigger(tag);
    }

    [PunRPC]
    private void SetPose(string tag)
    {
        Debug.Log("SetPose");
        getAnimator.Play(tag, 0, 1f); //해당 포즈로 즉시 재생
    }

    public void Initialize(string name, bool identification)
    {
        Set(name, identification);
        if (PhotonNetwork.IsMasterClient == true)
        {
            photonView.RPC("Set", RpcTarget.OthersBuffered, name, identification);
        }
    }

    public void Kill()
    {
        SetTrigger(FallingTag);
        if (PhotonNetwork.IsMasterClient == true)
        {
            photonView.RPC("SetTrigger", RpcTarget.OthersBuffered, FallingTag);
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
        if (PhotonNetwork.IsMasterClient == true)
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