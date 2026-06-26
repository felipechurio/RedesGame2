using TMPro;
using UnityEngine;

public class NicknameItem : MonoBehaviour
{
    [SerializeField] TMP_Text _nickname;
    [SerializeField] float _yOffsetPosition;

    Transform _owner;

    public void Initialize(Transform owner)
    {
        _owner = owner;
    }

    public void UpdateNickname(string newName)
    {
        _nickname.text = newName;
    }

    public void UpdatePosition()
    {
        transform.position = _owner.position + Vector3.up * _yOffsetPosition;
    }
}
