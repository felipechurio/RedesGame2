using System.Collections.Generic;
using UnityEngine;

public class NicknamesHandler : MonoBehaviour
{
    public static NicknamesHandler Instance { get; private set; }

    [SerializeField] NicknameItem _nicknamePrefab;

    List<NicknameItem> _nicknamesInUse;

    private void Awake()
    {
        Instance = this;

        _nicknamesInUse = new List<NicknameItem>();
    }

    public NicknameItem CreateNickname(NicknameComponent owner)
    {
        var newItem = Instantiate(_nicknamePrefab, transform);
        _nicknamesInUse.Add(newItem);

        newItem.Initialize(owner.transform);

        owner.OnLeft += () =>
        {
            _nicknamesInUse.Remove(newItem);
            Destroy(newItem.gameObject);
        };

        return newItem;
    }

    private void LateUpdate()
    {
        foreach (var nickname in _nicknamesInUse)
        {
            nickname.UpdatePosition();
        }
    }
}
