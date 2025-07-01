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
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var player = Instantiate(playerPrefab);

        // Привязывает соединение и ставит connectionToClient внутри NetworkIdentity
        NetworkServer.AddPlayerForConnection(conn, player);

        // НЕ НУЖНО: profile.connectionToClient = conn;

        var profile = player.GetComponent<NetworkPlayerProfile>();

        var session = FindFirstObjectByType<NetworkGameSession>();
        session.AddPlayer(profile);

        Debug.Log($"✅ Новый игрок добавлен: {profile.playerName}");
    }
    public override void OnClientSceneChanged()
    {
        base.OnClientSceneChanged();

        var gameSession = FindFirstObjectByType<NetworkGameSession>();
        if (gameSession != null && gameSession.raceStarted)
        {
            PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToGameplay);
        }
    }
}