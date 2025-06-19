using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class NetworkGameSession : NetworkBehaviour
{
    [SyncVar] public string sessionId;
    [SyncVar] public string sessionName;
    [SyncVar] public string mapName;
    [SyncVar] public int maxPlayers;
    [SyncVar] public bool raceStarted;


    [SyncVar] public NetworkPlayerProfile hostPlayer;
    
    private string pendingSessionId;
    private string pendingSessionName;

    public SyncList<NetworkPlayerProfile> syncedPlayers = new SyncList<NetworkPlayerProfile>();

    public RaceData currentRaceData;
    [Server]
    public void PrepareSession(string id, string name)
    {
        pendingSessionId = id;
        sessionName = name;
    }

    [Server]
    public void UpdateHost()
    {
        hostPlayer = GetHost();
    }
    public NetworkPlayerProfile GetHost()
    {
        return syncedPlayers.Count > 0 ? syncedPlayers[0] : null;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        sessionId = pendingSessionId;
        sessionName = pendingSessionName;

        

        var uiPanel = FindObjectsByType <UISessionPanel>(FindObjectsSortMode.None);
        foreach (var panel in uiPanel)
        {
            if (panel != null)
            {
                panel.AttachToNetworkSession(this);
                
            }

        }



        sessionName = "New Steam Session";
    }

    [Server]
    public void AddPlayer(NetworkPlayerProfile profile)
    {
        if (!syncedPlayers.Contains(profile))
            syncedPlayers.Add(profile);
        
        if (hostPlayer == null)
        {
            hostPlayer = profile;
            Debug.Log($"⭐ Назначен новый хост: {profile.playerName}");
        }
    }

    [Server]
    public void RemovePlayer(NetworkPlayerProfile profile)
    {
        syncedPlayers.Remove(profile);
       
        if(profile == hostPlayer)
        UpdateHost();
    }

    [Server]
    public void SetRaceData(RaceData data)
    {
        if (data == null)
        {
            Debug.LogError("RaceData is null в SetRaceData!");
            return;
        }

        currentRaceData = data;
        mapName = data.SceneName;
        maxPlayers = data.MaxCar; ;
    }

    [Server]
    public void StartRace()
    {
        raceStarted = true;
        NetworkManager.singleton.ServerChangeScene(mapName);
    }
}
