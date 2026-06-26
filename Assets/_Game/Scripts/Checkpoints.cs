using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class Checkpoints : NetworkBehaviour
{
    private GameObject _currentCheckpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority)
            return;

        if (other.CompareTag("Check"))
        {
            _currentCheckpoint = other.gameObject;
        }
    }

    public void SpawnCheckpoint(GameObject player, NetworkRigidbody3D networkRigidbody)
    {
        if (!Object.HasStateAuthority)
            return;

        if (_currentCheckpoint == null)
        {
            return;
        }

        Vector3 respawnPosition = _currentCheckpoint.transform.position;

        Rigidbody rb = networkRigidbody.Rigidbody;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = respawnPosition;
        player.transform.position = respawnPosition;

        Player playerScript = player.GetComponent<Player>();

        if (playerScript != null)
        {
            playerScript.SnapVisualToPlayer();
        }
    }
}