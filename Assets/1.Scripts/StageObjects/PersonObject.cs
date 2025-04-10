using System;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonAnimatorView))]
public class PersonObject : MonoBehaviourPunCallbacks
{
    public enum Type: byte
    {
        Capper,
        Lady,
        Police,
        Strider,
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

    private bool _hasRigidbody = false;

    private Rigidbody _rigidbody = null;

    private Rigidbody getRigidbody
    {
        get
        {
            if (_hasRigidbody == false)
            {
                _hasRigidbody = TryGetComponent(out _rigidbody);
            }
            return _rigidbody;
        }
    }

    private bool _hasSphereCollider = false;

    private SphereCollider _sphereCollider = null;

    private SphereCollider getSphereCollider
    {
        get
        {
            if (_hasSphereCollider == false)
            {
                _hasSphereCollider = TryGetComponent(out _sphereCollider);
            }
            return _sphereCollider;
        }
    }

    [SerializeField]
    private Renderer[] _renderers = new Renderer[2];

    public bool alive
    {
        get
        {
            return GetAnimationName() == FallingMotion;
        }
    }

    public static event Action<PersonObject> createAction = null;

    private static readonly string FallingMotion = "Falling";
    private static readonly Vector3 ColliderPoint = new Vector3(0, 0.5f, 0);

#if UNITY_EDITOR
    private void OnValidate()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        getRigidbody.useGravity = true;
        getRigidbody.freezeRotation = true;
        getSphereCollider.center = ColliderPoint;
        getSphereCollider.isTrigger = false;
    }
#endif

    private void Awake()
    {
        createAction?.Invoke(this);
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

    public override void OnRoomPropertiesUpdate(Hashtable hashtable)
    {

    }
}