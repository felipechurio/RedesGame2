using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private List<MonoBehaviour> scriptsToDisable = new();

    [SerializeField] private GameObject waitingForPlayersText;
    [SerializeField] private GameObject countdown3;
    [SerializeField] private GameObject countdown2;
    [SerializeField] private GameObject countdown1;
    [SerializeField] private Transform countdownTransform;
    [SerializeField] private GameObject[] canvasObjectsToDisable;

    [SerializeField] private float waitBeforeCountdown = 2f;
    [SerializeField] private float zoomScale = 1.4f;
    [SerializeField] private float zoomSpeed = 6f;

    [SerializeField] private Player _player;

    private bool countdownStarted;
    private bool gameStarted;

    private void Start()
    {
        if (scriptsToDisable.Count == 0)
        {
            scriptsToDisable.AddRange(GetComponents<MonoBehaviour>());
            scriptsToDisable.Remove(this);
        }

        SetGameplayScripts(false);
        HideCountdown();
    }

    public override void Spawned()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();

        if (!Object.HasInputAuthority && canvas != null)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameStarted)
            return;

        if (Object.HasStateAuthority && !countdownStarted && Runner != null && Runner.ActivePlayers.Count() >= 2)
        {
            countdownStarted = true;
            RPC_StartCountdown();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StartCountdown()
    {
        if (!countdownStarted)
            countdownStarted = true;

        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine()
    {
        SetGameplayScripts(false);

        if (waitingForPlayersText != null)
            waitingForPlayersText.SetActive(true);

        yield return new WaitForSeconds(waitBeforeCountdown);

        if (waitingForPlayersText != null)
            waitingForPlayersText.SetActive(false);

        yield return ShowNumber(countdown3);
        yield return ShowNumber(countdown2);
        yield return ShowNumber(countdown1);

        HideCountdown();

        foreach (GameObject obj in canvasObjectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        SetGameplayScripts(true);
        gameStarted = true;
        _player.SnapVisualToPlayer();
    }

    private IEnumerator ShowNumber(GameObject numberObject)
    {
        HideCountdown();

        if (numberObject != null)
            numberObject.SetActive(true);

        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;

            float scale = Mathf.Lerp(1f, zoomScale, timer);

            if (countdownTransform != null)
                countdownTransform.localScale = Vector3.one * scale;

            yield return null;
        }

        if (numberObject != null)
            numberObject.SetActive(false);
    }

    private void SetGameplayScripts(bool enabled)
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script == null || script == this)
                continue;

            script.enabled = enabled;
        }
    }

    private void HideCountdown()
    {
        if (countdown3 != null) countdown3.SetActive(false);
        if (countdown2 != null) countdown2.SetActive(false);
        if (countdown1 != null) countdown1.SetActive(false);

        if (countdownTransform != null)
            countdownTransform.localScale = Vector3.one;
    }
}