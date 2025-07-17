using Mirror;
using UnityEngine;

public class Checkpoint : NetworkBehaviour
{
    [SerializeField] private string _targetTag = "Player";
    public int checkpointIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_targetTag))
        {
            var player = other.GetComponent<NetworkIdentity>();

            if (player != null && player.connectionToClient != null)
            {
                RaiseChekpoint manager = FindFirstObjectByType<RaiseChekpoint>();
                if (manager != null)
                {
                    manager.UpdateCheckpoint(transform);
                }

                TargetHideCheckpoint(player.connectionToClient);
            }
        }
    


        if (other.CompareTag("Enemy"))
        {
            var aiController = other.GetComponent<BaiseCar>();
            if (aiController != null)
            {
                
                var car = other.GetComponent<BaiseCar>();
                if (car != null)
                {
                   

                }
            }
        }
    }

    [TargetRpc]
    private void TargetHideCheckpoint(NetworkConnection target)
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }
}



