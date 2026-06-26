using System.Collections.Generic;
using UnityEngine;

public class LifeBarsHandler : MonoBehaviour
{
    public static LifeBarsHandler Instance { get; private set; }

    [SerializeField] LifeBarItem _lifeBarPrefab;

    List<LifeBarItem> _lifeBarsInUse;

    private void Awake()
    {
        Instance = this;

        _lifeBarsInUse = new List<LifeBarItem>();
    }

    public LifeBarItem CreateLifeBar(PlayerHealth owner)
    {
        var newItem = Instantiate(_lifeBarPrefab, transform);
        _lifeBarsInUse.Add(newItem);

        newItem.Initialize(owner.transform);

        owner.OnLeft += () =>
        {
            _lifeBarsInUse.Remove(newItem);
            Destroy(newItem.gameObject);
        };

        return newItem;
    }

    private void LateUpdate()
    {
        foreach (var lifeBar in _lifeBarsInUse)
        {
            lifeBar.UpdatePosition();
        }
    }
}
