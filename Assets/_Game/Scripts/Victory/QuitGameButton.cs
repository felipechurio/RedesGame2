using UnityEngine;
using Fusion;

public class QuitGameButton : MonoBehaviour
{
    private NetworkRunner runner;

    private void Start()
    {
        runner = FindFirstObjectByType<NetworkRunner>();
    }

    public async void QuitGame()
    {
        if (runner != null)
        {
            await runner.Shutdown();
        }
        Application.Quit();
    }
}