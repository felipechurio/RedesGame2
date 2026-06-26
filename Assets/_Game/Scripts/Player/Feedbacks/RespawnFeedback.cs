using UnityEngine;

public class RespawnFeedback : MonoBehaviour
{
    [SerializeField] GameObject _visual;
    [SerializeField] PlayerHealth _health;

    private void Awake()
    {
        _health.OnRespawnUpdate += Refresh;
    }

    void Refresh(bool isDead)
    {
        _visual.SetActive(!isDead);
    }
}
