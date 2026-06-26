using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform _target;

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    void LateUpdate()
    {
        if (!_target) return;

        FollowTarget();
    }

    void FollowTarget()
    {
        //var newPos = _target.position;
        //newPos.z = transform.position.z;

        //_target.position = newPos;

        transform.position = new Vector3(_target.position.x, _target.position.y, transform.position.z);
    }
}
