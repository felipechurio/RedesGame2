using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionItem : MonoBehaviour
{
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _playersText;
    [SerializeField] Button _joinButton;

    RunnerHandler _runnerHandler;
    SessionInfo _sessionInfo;

    public void Initialize(RunnerHandler runnerHandler, SessionInfo sessionInfo)
    {
        _sessionInfo = sessionInfo;
        _runnerHandler = runnerHandler;

        _nameText.text = _sessionInfo.Name;
        _playersText.text = $"{_sessionInfo.PlayerCount}/{_sessionInfo.MaxPlayers}";

        _joinButton.interactable = _sessionInfo.PlayerCount < _sessionInfo.MaxPlayers;
        _joinButton.onClick.AddListener(JoinGame);
    }

    void JoinGame()
    {
        _joinButton.interactable = false;
        _runnerHandler.JoinGame(_sessionInfo);
    }
}
