using Mirror;
using Unity.Cinemachine;
using UnityEngine;

public class CameraLockAtPlayer : MonoBehaviour
{
    public CinemachineCamera Camera;
    private bool isCameraSet = false;

    private void Start()
    {
        if (Camera == null)
        {
            Debug.LogError("VirtualCamera не назначена!");
            return;
        }

    }


    private void Update()
    {
        if (!isCameraSet && NetworkClient.localPlayer != null)
        {
            SetupCamera(NetworkClient.localPlayer.gameObject);
            isCameraSet = true;
        }
    }
    private void SetupCamera(GameObject player)
    {
        Camera.Follow = player.transform;
        Camera.LookAt = player.transform;
        Camera.gameObject.SetActive(true);
        Debug.Log($"Камера привязана к локальному игроку: {player.name}");
    }
}