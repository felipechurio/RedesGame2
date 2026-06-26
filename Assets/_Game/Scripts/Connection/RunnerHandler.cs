using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkRunner _runnerPrefab;
    NetworkRunner _currentRunner;

    public event Action OnJoinedLobbySuccesfully;
    public event Action<List<SessionInfo>> OnSessionsUpdated;

    //private void Awake()
    //{
    //    JoinLobby();
    //}

    public async void JoinLobby()
    {
        if (_currentRunner)
            Destroy(_currentRunner.gameObject);

        _currentRunner = Instantiate(_runnerPrefab);

        _currentRunner.AddCallbacks(this);

        Debug.Log("Trying to connect to lobby");

        await JoinLobbyAsync();
    }

    async Task JoinLobbyAsync()
    {
        var result = await _currentRunner.JoinSessionLobby(SessionLobby.Custom, "Custom Lobby");
    
        if (result.Ok)
        {
            Debug.Log("Connected to Lobby");
            OnJoinedLobbySuccesfully?.Invoke();
        }
        else
        {
            Debug.Log($"Failed joining Lobby, Message: {result.ErrorMessage}");
        }
    }

    public async void JoinGame(SessionInfo sessionInfo)
    {
        await CreateGame(GameMode.Client, sessionInfo.Name);
    }

    public async void HostGame(string sessionName, string sceneName)
    {
        await CreateGame(GameMode.Host, sessionName, SceneUtility.GetBuildIndexByScenePath($"_Game/Scenes/{sceneName}"));
    }

    async Task CreateGame(GameMode gameMode, string sessionName, int sceneIndex = 0)
    {
        _currentRunner.ProvideInput = true;

        var result = await _currentRunner.StartGame(new StartGameArgs()
        {
            GameMode = gameMode,
            SessionName = sessionName,
            Scene = SceneRef.FromIndex(sceneIndex),
            PlayerCount = 2
        });

        if (result.Ok)
        {
            Debug.Log("Connected to Game");
        }
        else
        {
            Debug.Log($"Failed joining/creating game, Message: {result.ErrorMessage}");
        }
    }

    //bool _connectingToGame = false;

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //if (_connectingToGame) return;

        //if (sessionList.Count > 0)//Si hay alguna sesion
        //{
        //    //nos conectamos como Client a ella
        //    JoinGame(sessionList[0]);
        //}
        //else //Sino, creamos nuestra sala
        //{
        //    HostGame("Bla", "Gameplay");
        //}

        //_connectingToGame = true;

        OnSessionsUpdated?.Invoke(sessionList);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    { }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    { }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    { }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    { }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    { }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    { }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    { }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    { }

    public void OnConnectedToServer(NetworkRunner runner)
    { }

    

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    { }

    public void OnSceneLoadDone(NetworkRunner runner)
    { }

    public void OnSceneLoadStart(NetworkRunner runner)
    { }
}
