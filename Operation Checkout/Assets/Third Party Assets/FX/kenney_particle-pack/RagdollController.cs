using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Rigidbody[] _ragdollBones;
    [SerializeField] private Collider[] _ragdollColliders;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void ActivateRagdoll()
    {
        foreach (var bone in _ragdollBones)
        {
            bone.isKinematic = false;
        }

        foreach (var collider in _ragdollColliders)
        {
            collider.enabled = true;
        }

        _animator.enabled = false;
    }

    public void DeactivateRagdoll()
    {
        foreach (var bone in _ragdollBones)
        {
            bone.isKinematic = true;
        }

        foreach (var collider in _ragdollColliders)
        {
            collider.enabled = false;
        }

        _animator.enabled = true;
    }
}