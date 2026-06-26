using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelsController : MonoBehaviour
{
    [SerializeField] RunnerHandler _runnerHandler;

    [Header("Panels")]
    [SerializeField] GameObject _initialPanel;
    [SerializeField] GameObject _connectingPanel;
    [SerializeField] GameObject _sessionBrowserPanel;
    [SerializeField] GameObject _hostPanel;

    [Header("Buttons")]
    [SerializeField] Button _joinLobbyButton;
    [SerializeField] Button _goToHostPanelButton;
    [SerializeField] Button _hostButton;

    [Header("InputFields")]
    [SerializeField] TMP_InputField _sessionNameField;
    [SerializeField] TMP_InputField _nicknameField;

    private void Awake()
    {
        _joinLobbyButton.onClick.AddListener(ShowConnectingPanel);

        _runnerHandler.OnJoinedLobbySuccesfully += () =>
        {
            _connectingPanel.SetActive(false);
            _sessionBrowserPanel.SetActive(true);
        };

        _goToHostPanelButton.onClick.AddListener(ShowHostPanel);

        _hostButton.onClick.AddListener(HostGame);
    }

    void ShowConnectingPanel()
    {
        PlayerPrefs.SetString("Nickname", _nicknameField.text);

        _initialPanel.SetActive(false);
        _connectingPanel.SetActive(true);

        _runnerHandler.JoinLobby();
    }

    void ShowHostPanel()
    {
        _sessionBrowserPanel.SetActive(false);
        _hostPanel.SetActive(true);
    }

    void HostGame()
    {
        _hostButton.interactable = false;
        _runnerHandler.HostGame(_sessionNameField.text, "Gameplay");
    }
}
