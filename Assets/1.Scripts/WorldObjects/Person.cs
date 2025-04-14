using System;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonAnimatorView))]
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
            return GetAnimationName() != FallingTag;
        }
    }

    public static event Action<Person> createAction = null;

    private static readonly string FallingTag = "Falling";

    public const bool Citizen = false;
    public const bool Criminal = true;

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

    private string GetAnimationName()
    {
        AnimatorClipInfo[] animatorClipInfos = getAnimator.GetCurrentAnimatorClipInfo(0);
        if (animatorClipInfos.Length > 0 && animatorClipInfos[0].clip != null)
        {
            return animatorClipInfos[0].clip.name;
        }
        return null;
    }

    public void Initialize(string name, bool identification)
    {
        Set(name, identification);
        if (photonView.IsMine == true)
        {
            photonView.RPC("Set", RpcTarget.OthersBuffered, name, identification);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {

    }
}