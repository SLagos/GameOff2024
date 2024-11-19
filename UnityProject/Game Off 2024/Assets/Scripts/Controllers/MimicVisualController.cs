using Unity.Netcode;
using UnityEngine;

public class MimicVisualController : NetworkBehaviour
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private Animator _animator;
    private PlayerMovement _playerMovement;

    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if(!IsOwner) return;
        _playerMovement = parentNetworkObject.GetComponentInParent<PlayerMovement>();
        if(_playerMovement == null) return;
        _playerMovement.SetFollowTarget(_followTarget);
        if (_animator != null)
            _playerMovement.SetAnimator(_animator);
    }

    protected override void OnNetworkPostSpawn()
    {
        
    }

}