using UnityEngine;
using UnityEngine.UI;

public class LifeBarItem : MonoBehaviour
{
    [SerializeField] Image _lifeBar;
    [SerializeField] float _yOffsetPosition;

    Transform _owner;

    public void Initialize(Transform owner)
    {
        _owner = owner;
    }

    public void UpdateLifeBar(int currentHealth, int maxHealth)
    {
        _lifeBar.fillAmount = currentHealth / (float)maxHealth;
    }

    public void UpdatePosition()
    {
        transform.position = _owner.position + Vector3.up * _yOffsetPosition;
    }
}
