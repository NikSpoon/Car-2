

using Unity.Cinemachine;
using UnityEngine;
public class CaneraLockAtPlaer : MonoBehaviour
{

    public CinemachineCamera Camera;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && Camera != null)
        {
           Camera.Follow = player.transform;
            Camera.LookAt = player.transform;
        }
        else
        {
            Debug.LogError("Player или VirtualCamera не найдены!");
        }
    }


}
