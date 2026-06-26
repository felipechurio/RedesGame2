using Fusion;
using UnityEngine;

public class MovementAnimation : MonoBehaviour
{
    [SerializeField] PlayerMovement _movement;
    [SerializeField] NetworkMecanimAnimator _mecanim;

    private void Awake()
    {
        _movement.OnMovement += UpdateAnimation;
    }

    void UpdateAnimation(float xAxi)
    {
        _mecanim.Animator.SetFloat("xAxi", Mathf.Abs(xAxi));
    }
}
