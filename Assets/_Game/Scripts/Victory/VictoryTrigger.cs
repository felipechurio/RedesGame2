using Fusion;
using UnityEngine;

public class VictoryTrigger : NetworkBehaviour
{
    private bool gameEnded;

    private void OnTriggerEnter(Collider other)
    {
        if (gameEnded)
            return;

        if (!Object.HasStateAuthority)
            return;

        if (!other.CompareTag("Player") && !other.transform.root.CompareTag("Player"))
            return;


        NetworkObject playerObject = other.GetComponentInParent<NetworkObject>();

        if (playerObject == null)
            return;

        gameEnded = true;

        RPC_EndGame(playerObject.InputAuthority);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_EndGame(PlayerRef winner)
    {
        DisableMovement();

        bool iWon = Runner.LocalPlayer == winner;

        if (PlayerEndGameUI.Local != null)
        {
            PlayerEndGameUI.Local.ShowEndGameUI(iWon);
        }
    }

    private void DisableMovement()
    {
        Player[] allPlayers = FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach (Player movement in allPlayers)
        {
            movement.enabled = false;
        }
    }
}