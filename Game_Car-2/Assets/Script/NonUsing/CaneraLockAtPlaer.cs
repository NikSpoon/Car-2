using Mirror;
using Mirror.BouncyCastle.Utilities.Encoders;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

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
         var p = PlayerDataManager.Instance.PlayerProfile.playerName;
        Debug.Log($"Камера привязана к локальному игроку: {p}");
    }
}