using Mirror;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FSM.App;


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


    private BotCreator botCreator;

    [SerializeField] private GameObject botPrefab;

    public bool onStart;
    public int timeToStart;

    public List<UIGameSession> uIGameSessions = new List<UIGameSession>();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        maxPlayers = 10;
        botCreator = new BotCreator();
    }

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



        var uiPanel = FindObjectsByType<UISessionPanel>(FindObjectsSortMode.None);
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
        if (syncedPlayers.Count == maxPlayers)
        {
            ErrorLog("AddPlayer");
            return;
        }

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

        if (profile == hostPlayer)
            UpdateHost();
    }

    [Server]
    public void SetRaceData(RaceData data)
    {
        if (data == null)
        {
            return;
        }

        currentRaceData = data;
        mapName = data.SceneName;
        maxPlayers = data.MaxCar; ;
    }

    [Server]
    public void StartRace()
    {
        if (!currentRaceData)
        {
            ErrorLog("StartRace");
            return;
        }

        raceStarted = true;
        StartCoroutine(StartTimer());
    }
    private IEnumerator StartTimer()
    {
        int time = 10;
        timeToStart = time;
        onStart = true;

        while (timeToStart > 0)
        {
            RpcUpdateTimer(timeToStart); 
            yield return new WaitForSeconds(1f);
            timeToStart--;
        }

        RpcUpdateTimer(0); 

        onStart = false;
        PlayerDataManager.Instance.AppSystem.Trigger(AppTriger.ToGameplay);
        NetworkManager.singleton.ServerChangeScene(mapName);
    }

    [ClientRpc]
    private void RpcUpdateTimer(int timeLeft)
    {
        foreach (var t in uIGameSessions)
        {
            if (t != null)
            {
                t.UpdateTimer(timeLeft);
            }

        }
    }

    [Server]
    public void AddBot()
    {
        // Проверяем, что вызывающий — хост (сервер)
        if (!isServer)
        {
            Debug.LogWarning("Добавлять бота может только сервер (хост)");
            return;
        }

        if (botCreator == null || botPrefab == null)
        {
            Debug.LogError("BotCreator или botPrefab не назначены!");
            return;
        }

        AIProfile aiProfile = botCreator.CreateUniqueBot();

        GameObject botObj = Instantiate(botPrefab);
        NetworkServer.Spawn(botObj);

        NetworkBotProfile botProfile = botObj.GetComponent<NetworkBotProfile>();

        if (botProfile == null)
        {
            Debug.LogError("У префаба бота нет компонента NetworkBotProfile!");
            Destroy(botObj);
            return;
        }

        botProfile.InitializeBot(aiProfile);

        AddPlayer(botProfile);

        Debug.Log($"🤖 Добавлен бот: {botProfile.playerName}");
    }

    private void Update()
    {
        if (currentRaceData == null) return;

        mapName = currentRaceData.SceneName;
        maxPlayers = currentRaceData.MaxCar;
    }

    [Command]
    public void CmdRequestStartRace()
    {
        if (!isServer) return;

        if (!raceStarted)
        {
            StartRace();
        }
    }

    private void ErrorLog(string context)
    {
        return;
    }

}