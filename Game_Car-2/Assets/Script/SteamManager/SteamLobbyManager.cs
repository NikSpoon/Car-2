
using Mirror;
using Steamworks;
using Mirror.FizzySteam;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SteamLobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject networkGameSessionPrefab; // Префаб объекта с NetworkGameSession

    private NetworkManager networkManager; // Менеджер сети Mirror
    private NetworkGameSession networkGameSession; // Текущая сетевая сессия игры

    protected Callback<LobbyCreated_t> lobbyCreated; // Колбэк на создание лобби в Steam
    protected Callback<LobbyEnter_t> lobbyEntered;   // Колбэк на вход в лобби Steam
    protected Callback<LobbyChatUpdate_t> lobbyChatUpdate;

    public CSteamID CurrentLobbyID { get; private set; } // Steam ID текущего лобби

    public System.Action<CSteamID> OnLobbyCreatedUI; // Событие для UI: передаем ID лобби
    public System.Action<CSteamID> OnLobbyEmpty;// Событие для UI: передаем ID лобби
    public Dictionary<CSteamID, NetworkGameSession> Lobbies { get; private set; } = new();

    // Текущий колбэк на список лобби
    protected Callback<LobbyMatchList_t> lobbyMatchList;

    // Локальный список лобби от Steam (SteamID)
    private List<CSteamID> availableLobbies = new List<CSteamID>();
    private List<CSteamID> filteredLobbies = new List<CSteamID>();

    // Ссылка на UI менеджер, чтобы обновлять список
    [SerializeField] private SteamLobbyUIManager lobbyUIManager;
    private bool host = false;
    private void Start()
    {

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam не инициализирован");
            return;
        }

        var myltiServer = GameObject.FindGameObjectWithTag("NetworkManager");
        networkManager = myltiServer.GetComponent<NetworkManager>();


        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);

        RequestLobbies();
       
    }
    private void OnEnable()
    {
        SteamNetworkingUtils.InitRelayNetworkAccess();
        RequestLobbies();
    }

    // Метод запроса публичных лобби
    public void RequestLobbies()
    {
        Debug.Log("Запрос списка лобби...");
        SteamMatchmaking.AddRequestLobbyListStringFilter("Game", "Mitrix", ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyMatchList(LobbyMatchList_t result)
    {
        Debug.Log($"Получено {result.m_nLobbiesMatching} лобби от Steam");

        availableLobbies.Clear();

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            availableLobbies.Add(lobbyId);
        }

        FilterLobbies();
        // Теперь обновим UI, получив данные по каждому лобби
        UpdateLobbyListUI();
    }
    private void FilterLobbies()
    {
        filteredLobbies.Clear();
        foreach (var lobbyId in availableLobbies)
        {
            string gameKey = SteamMatchmaking.GetLobbyData(lobbyId, "Game");
            if (gameKey == "Mitrix")
            {
                filteredLobbies.Add(lobbyId);
            }
        }
    }
    public void CreateLobby()
    {
        if (NetworkServer.active)
        {
            Debug.LogWarning("Сервер уже запущен!");
            return;
        }

        int maxPlayers = 20;

        // Создаем публичное лобби Steam с максимальным количеством игроков
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxPlayers);
    }

    private void OnLobbyCreated(LobbyCreated_t result)
    {

        if (NetworkServer.active) 
        {
            ForceDisconnect();

        }
        // Проверяем, успешно ли создано лобби
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Ошибка создания лобби: " + result.m_eResult);
            return;
        }
        CSteamID hostAddress = SteamUser.GetSteamID();
        // Сохраняем Steam ID созданного лобби
        CurrentLobbyID = new CSteamID(result.m_ulSteamIDLobby);
        Debug.Log("Лобби создано: " + CurrentLobbyID);

        // Устанавливаем в данных лобби адрес хоста — SteamID пользователя, который создал лобби
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "HostAddress", SteamUser.GetSteamID().ToString());


        // Запускаем хост-сервер Mirror (сервер + клиент на одном ПК)
        StartCoroutine(StartHostAndSpawnSession());
    }
    private IEnumerator StartHostAndSpawnSession()
    {
        if (!networkManager.isNetworkActive)
        {
            networkManager.StartHost();
            Debug.Log("Host started, SteamID: " + SteamUser.GetSteamID());
        }

        // Ждём, пока сервер не станет активным
        while (!NetworkServer.active)
        {
            yield return null;
        }

        SetInfo();

        networkGameSession = SpawnNetworkGameSession();

        networkGameSession.uIGameSession = FindFirstObjectByType<UIGameSession>();
        networkGameSession.uIGameSession.SetSession(networkGameSession);

        Lobbies.Add(CurrentLobbyID, networkGameSession);

        SteamDebugMonitor.Instance.SetLobbyId(CurrentLobbyID);
        SteamDebugMonitor.Instance.SetConnectionAddress(SteamUser.GetSteamID().ToString());
        SteamDebugMonitor.Instance.SetHostSteamId(SteamUser.GetSteamID());
        SteamDebugMonitor.Instance.networkManager = this.networkManager;

        StartCoroutine(InvokeLobbyCreatedUIDelayed(CurrentLobbyID));
    }
    private void OnLobbyEntered(LobbyEnter_t result)
    {
        if (NetworkServer.active) return;
        ForceDisconnect();


        // Получаем Steam ID лобби, в которое вошли
        CurrentLobbyID = new CSteamID(result.m_ulSteamIDLobby);


        // Если это сервер, выходим, так как сервер не должен подключаться как клиент
        if (NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(CurrentLobbyID, "HostAddress");
      
        // Конвертируем строку в CSteamID
        CSteamID hostSteamId = new CSteamID(ulong.Parse(hostAddress));


        Debug.Log($"[OnLobbyEntered] Host SteamID: {hostAddress}");
        
        var fizzy = (FizzySteamworks)NetworkManager.singleton.transport;

        fizzy.ClientConnect(hostSteamId.ToString());

        // ВАЖНО: вот это всё что нужно — FizzySteamworks возьмет networkAddress!
        networkManager.StartClient();

        SteamDebugMonitor.Instance.SetLobbyId(CurrentLobbyID);
        SteamDebugMonitor.Instance.SetConnectionAddress(hostAddress);
        SteamDebugMonitor.Instance.SetHostSteamId(hostSteamId);
        SteamDebugMonitor.Instance.networkManager = this.networkManager;

        Debug.Log($"[OnLobbyEntered] Joining host at address: {hostAddress}");
     
        
    }
    public void ForceDisconnect()
    {
        if (networkManager.isNetworkActive)
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                Debug.Log("[ForceDisconnect] Stopping Host");
                networkManager.StopHost();
            }
            else if (NetworkServer.active)
            {
                Debug.Log("[ForceDisconnect] Stopping Server");
                networkManager.StopServer();
            }
            else if (NetworkClient.isConnected || NetworkClient.isConnecting)
            {
                Debug.Log("[ForceDisconnect] Stopping Client");
                networkManager.StopClient();
            }
        }
        else
        {
            Debug.Log("[ForceDisconnect] Nothing to stop");
        }
    }
    public void SetInfo()
    {
        string sessionName = PlayerDataManager.Instance.PlayerProfile.playerName;
        string mapName = "Map1"; // или выбери динамически, если нужно
        string maxPlayers = "20";
        string currentPlayers = "1"; // Хост всегда первый участник

        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "Game", "Mitrix");
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "SessionName", sessionName);
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "MapName", mapName);
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "MaxPlayers", maxPlayers);
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "CurrentPlayers", currentPlayers);
    }


    // Метод для присоединения к лобби по ID (например, введенному в UI)
    public void JoinLobbyById(string lobbyId)
    {
        // Пытаемся преобразовать строку в ulong (SteamID)
        if (ulong.TryParse(lobbyId, out ulong parsedId))
        {
            CSteamID lobbySteamID = new CSteamID(parsedId);
            SteamMatchmaking.JoinLobby(lobbySteamID);
        }
        else
        {
            Debug.LogError("Неверный формат Lobby ID");
        }
    }

    // Создаем объект NetworkGameSession на сервере и запускаем его в сети
    public NetworkGameSession SpawnNetworkGameSession()
    {

        // 1) Инстансиируем префаб
        GameObject sessionObj = Instantiate(networkGameSessionPrefab);
        var session = sessionObj.GetComponent<NetworkGameSession>();

        // 2) Формируем короткий ID из SteamLobbyID
        string shortId = (CurrentLobbyID.m_SteamID % 100000).ToString("D5");
        // Имя сессии можно взять, например, из профиля хоста
        string sessionName = PlayerDataManager.Instance.PlayerProfile.playerName;

        // 4) Устанавливаем lobbyId
        session.SetLobbyId(CurrentLobbyID, shortId);

        // 5) Спауним объект
        NetworkServer.Spawn(sessionObj);


        return session;
    }

    private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
    {
        CSteamID lobbyID = (CSteamID)callback.m_ulSteamIDLobby;
        CSteamID userChanged = (CSteamID)callback.m_ulSteamIDUserChanged;
        EChatMemberStateChange stateChange = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;

        if (stateChange == EChatMemberStateChange.k_EChatMemberStateChangeLeft ||
            stateChange == EChatMemberStateChange.k_EChatMemberStateChangeDisconnected ||
            stateChange == EChatMemberStateChange.k_EChatMemberStateChangeKicked ||
            stateChange == EChatMemberStateChange.k_EChatMemberStateChangeBanned)
        {
            Debug.Log($"Игрок {userChanged} покинул лобби {lobbyID}");

            // Проверяем, остался ли кто-то в лобби
            int memberCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
            if (memberCount == 0)
            {
                Debug.Log("‼️ Лобби опустело — можно считать его закрытым.");
                // Вызови тут своё событие, если нужно
                OnLobbyEmpty?.Invoke(lobbyID);
            }
        }
    }
    public void UpdateLobbyInfo(string mapName, int maxPlayers, int currentPlayers, int exp)
    {
        if (CurrentLobbyID == CSteamID.Nil)
        {
            Debug.LogError("❌ Нет активного лобби для обновления данных!");
            return;
        }

        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "MapName", mapName);
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "MaxPlayers", maxPlayers.ToString());
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "CurrentPlayers", currentPlayers.ToString());
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "Experience", exp.ToString());

        Debug.Log("✅ Лобби данные обновлены: Map, MaxPlayers, CurrentPlayers, Exp");
    }
    public void UpdateLobbyData(string key, string value)
    {
        if (CurrentLobbyID == CSteamID.Nil)
        {
            Debug.LogError("❌ Нет активного лобби для обновления данных!");
            return;
        }

        SteamMatchmaking.SetLobbyData(CurrentLobbyID, key, value);
        Debug.Log($"🔄 Лобби обновлено: {key} = {value}");
    }
    private IEnumerator InvokeLobbyCreatedUIDelayed(CSteamID lobbyId)
    {
        yield return null; // подождать 1 кадр
        OnLobbyCreatedUI?.Invoke(lobbyId);
    }
    private void UpdateLobbyListUI()
    {
        foreach (var lobbyId in filteredLobbies)
        {
            string sessionName = SteamMatchmaking.GetLobbyData(lobbyId, "SessionName"); // например
            string mapName = SteamMatchmaking.GetLobbyData(lobbyId, "MapName");
            string maxPlayersStr = SteamMatchmaking.GetLobbyData(lobbyId, "MaxPlayers");
            string currentPlayersStr = SteamMatchmaking.GetLobbyData(lobbyId, "CurrentPlayers");
            string expStr = SteamMatchmaking.GetLobbyData(lobbyId, "Experience");
            int exp = 0;
            int.TryParse(expStr, out exp);

            int maxPlayers = 0;
            int currentPlayers = 0;

            int.TryParse(maxPlayersStr, out maxPlayers);
            int.TryParse(currentPlayersStr, out currentPlayers);

            // Если нужно — передай exp в твой UI:
            lobbyUIManager.UpdateOrCreateLobbyUI(lobbyId, sessionName, mapName, currentPlayers, maxPlayers);

         
        }
    }
    public void LeaveCurrentLobby()
    {
        if (networkManager == null)
        {
            Debug.LogWarning("NetworkManager не назначен");
            return;
        }

        if (NetworkServer.active)
        {
            networkManager.StopHost();
            Debug.Log("Остановлен хост");
        }
        else if (NetworkClient.isConnected)
        {
            networkManager.StopClient();
            Debug.Log("Остановлен клиент");
        }

        if (CurrentLobbyID.IsValid())
        {
            SteamMatchmaking.LeaveLobby(CurrentLobbyID);
            Debug.Log($"Покинул лобби {CurrentLobbyID}");
            CurrentLobbyID = CSteamID.Nil;
        }
        Lobbies.Clear();
        networkGameSession = null;
    }
}
