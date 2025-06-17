using Mirror;
using System.Collections.Generic;

public class NetworkGameSession : NetworkBehaviour
{
    [SyncVar] public string sessionId;
    [SyncVar] public string sessionName;
    [SyncVar] public string mapName;
    [SyncVar] public int maxPlayers;
    [SyncVar] public bool raceStarted;

    public SyncList<NetworkPlayerProfile> syncedPlayers = new SyncList<NetworkPlayerProfile>();

    public RaceData currentRaceData;

    public override void OnStartServer()
    {
        base.OnStartServer();
        // Можешь здесь инициализировать данные, например:
        sessionId = System.Guid.NewGuid().ToString();
        sessionName = "New Steam Session";
    }

    [Server]
    public void AddPlayer(NetworkPlayerProfile profile)
    {
        if (!syncedPlayers.Contains(profile))
            syncedPlayers.Add(profile);
    }

    [Server]
    public void RemovePlayer(NetworkPlayerProfile profile)
    {
        syncedPlayers.Remove(profile);
    }

    [Server]
    public void SetRaceData(RaceData data)
    {
        currentRaceData = data;
        mapName = data.SceneName;
        maxPlayers = data.MaxCar;
    }

    [Server]
    public void StartRace()
    {
        raceStarted = true;
        NetworkManager.singleton.ServerChangeScene(mapName);
    }
}
