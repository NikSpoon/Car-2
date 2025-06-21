using FSM.App;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public void StartMultiplayerScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("Scene name is empty");
            return;
        }

        ServerChangeScene(sceneName);
    }
}