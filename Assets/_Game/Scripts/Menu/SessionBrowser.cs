using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionBrowser : MonoBehaviour
{
    [SerializeField] RunnerHandler _runnerHandler;

    [SerializeField] SessionItem _sessionItemPrefab;
    [SerializeField] VerticalLayoutGroup _contentLayout;

    [SerializeField] TMP_Text _statusText;

    private void OnEnable()
    {
        _runnerHandler.OnSessionsUpdated += UpdateList;
    }

    private void OnDisable()
    {
        _runnerHandler.OnSessionsUpdated -= UpdateList;
    }

    void UpdateList(List<SessionInfo> sessions)
    {
        ClearBrowser();

        if (sessions.Count == 0)
        {
            _statusText.gameObject.SetActive(true);
            return;
        }

        _statusText.gameObject.SetActive(false);

        foreach (var session in sessions)
        {
            var sessionItem = Instantiate(_sessionItemPrefab, _contentLayout.transform);
            sessionItem.Initialize(_runnerHandler, session);
        }
    }

    void ClearBrowser()
    {
        foreach (Transform child in _contentLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
