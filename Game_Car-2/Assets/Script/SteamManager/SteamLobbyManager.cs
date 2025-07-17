
using Mirror;
using Steamworks;
using Mirror.FizzySteam;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Edgegap;

public class SteamLobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject networkGameSessionPrefab; // Префаб объекта с NetworkGameSession

    private NetworkManager networkManager; // Менеджер сети Mirror
    private NetworkGameSession networkGameSession; // Текущая сетевая сессия игры

    protected Callback<LobbyCreated_t> lobbyCreated; // Колбэк на создание лобби в Steam
    protected Callback<LobbyEnter_t> lobbyEntered;   // Колбэк на вход в лобби Steam
    protected Callback<LobbyDataUpdate_t> lobbyDataUpdate;

    public CSteamID CurrentLobbyID { get; private set; } // Steam ID текущего лобби

    public System.Action<CSteamID> OnLobbyCreatedUI; // Событие для UI: передаем ID лобби
    public System.Action<CSteamID> OnLobbyEmpty;// Событие для UI: передаем ID лобби
    public Dictionary<CSteamID, NetworkGameSession> Lobbies { get; private set; } = new();

    // Текущий колбэк на список лобби
    protected Callback<LobbyMatchList_t> lobbyMatchList;

    // Локальный список лобби от Steam (SteamID)
    private List<CSteamID> availableLobbies = new List<CSteamID>();
    private List<CSteamID> filteredLobbies  =  new List<CSteamID>();

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

        if (networkManager == null)
        {
            var myltiServer = GameObject.FindGameObjectWithTag("NetworkManager");
            networkManager = myltiServer.GetComponent<NetworkManager>();
        }


        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);


    }
    private void OnEnable()
    {
        SteamNetworkingUtils.InitRelayNetworkAccess();
        RequestLobbies();
    }

    // Метод запроса публичных лобби
    public void RequestLobbies()
    {
        //Debug.Log("Запрос списка лобби...");
        SteamMatchmaking.AddRequestLobbyListStringFilter("Game", "Mitrix", ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnLobbyMatchList(LobbyMatchList_t result)
    {
       //  Debug.Log($"Получено {result.m_nLobbiesMatching} лобби от Steam");

        availableLobbies.Clear();
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
            availableLobbies.Add(lobbyId);
        }
       
        filteredLobbies.Clear();
        foreach (var lobbyId in availableLobbies)
        {
            string gameKey = SteamMatchmaking.GetLobbyData(lobbyId, "Game");
            if (gameKey == "Mitrix") // или твой фильтр по ключу "Game"
            {
                filteredLobbies.Add(lobbyId);
            }
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
            ForceDisconnect();

        }

        int maxPlayers = 20;

        // Создаем публичное лобби Steam с максимальным количеством игроков
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxPlayers);
    }

    private void OnLobbyCreated(LobbyCreated_t result)
    {
        host = true;
        if (NetworkServer.active)
        {
            Debug.LogWarning("Сервер уже запущен!");
            return;
        }
        // Проверяем, успешно ли создано лобби
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Ошибка создания лобби: " + result.m_eResult);
            return;
        }

        DestroySessionAndCloseLobby();

        CSteamID hostAddress = SteamUser.GetSteamID();
        // Сохраняем Steam ID созданного лобби
        CurrentLobbyID = new CSteamID(result.m_ulSteamIDLobby);
     
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
           // Debug.Log("Host started, SteamID: " + SteamUser.GetSteamID());
        }

        // Ждём, пока сервер не станет активным
        while (!NetworkServer.active)
        {
            if (NetworkServer.active)
            {
                break;
            }
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
        Debug.Log("[OnLobbyEntered] Triggered");

        var enteredLobby = new CSteamID(result.m_ulSteamIDLobby);

        // Если ты хост и вошёл в своё же лобби — не трогай ничего!
        if (host && enteredLobby == CurrentLobbyID)
        {
            Debug.Log("[OnLobbyEntered] Это моё лобби — игнорирую.");
            return;
        }

        if (NetworkServer.active && host)
        {
            Debug.Log("[OnLobbyEntered] Был хостом — переключаюсь в клиентский режим");
            StartCoroutine(StopHostAndJoin(result));
            return;
        }

        CurrentLobbyID = enteredLobby;


        Debug.Log("[OnLobbyEntered] Mirror остановлен, подключаемся как клиент");
        StartCoroutine(JoinAsClientRoutine(CurrentLobbyID));

    }
    private IEnumerator StopHostAndJoin(LobbyEnter_t result)
    {
        // Останови всё
        ForceDisconnect();
        host = false; // больше не хост

        // Подожди пока сеть Mirror реально остановится
        while (networkManager.isNetworkActive)
        {
            yield return null;
        }

        Debug.Log("[StopHostAndJoin] Mirror остановлен, подключаемся как клиент");
        yield return JoinAsClientRoutine(CurrentLobbyID);
    }
    private IEnumerator JoinAsClientRoutine(CSteamID lobbyId)
    {
        Debug.Log("[JoinAsClientRoutine] Force disconnect if needed");

        // Отключаем, если что-то запущено
        ForceDisconnect();

        // Ждём полного отключения
        while (NetworkServer.active || NetworkClient.active || networkManager.isNetworkActive)
        {
            Debug.Log("[JoinAsClientRoutine] Waiting for full disconnect...");
            yield return null;
        }

        Debug.Log("[JoinAsClientRoutine] Disconnected, connecting as client");

        string hostAddress = SteamMatchmaking.GetLobbyData(lobbyId, "HostAddress");
        CSteamID hostSteamId = new CSteamID(ulong.Parse(hostAddress));

        var fizzy = (FizzySteamworks)NetworkManager.singleton.transport;
        fizzy.ClientConnect(hostSteamId.ToString());

        networkManager.StartClient();

        Debug.Log($"[JoinAsClientRoutine] Joining host at address: {hostAddress}");
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
        string mapName = "Map1"; 
        string maxPlayers = "20";
        string currentPlayers = "1";
        string smoalId = "1";

        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "Game", "Mitrix");
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "SessionName", sessionName);
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "MapName", mapName);
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "MaxPlayers", maxPlayers);
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "CurrentPlayers", currentPlayers);
        SteamMatchmaking.SetLobbyData(CurrentLobbyID, "ID", smoalId);
    }


    // Метод для присоединения к лобби по ID (например, введенному в UI)
    public void JoinLobbyById(string lobbyId)
    {
        // Пытаемся преобразовать строку в ulong (SteamID)
        if (ulong.TryParse(lobbyId, out ulong parsedId))
        {
            if (CurrentLobbyID.IsValid())
            {
                Debug.Log($"[JoinLobbyById] Покидаю текущее лобби: {CurrentLobbyID}");
                SteamMatchmaking.LeaveLobby(CurrentLobbyID);
                CurrentLobbyID = CSteamID.Nil;
            }

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

    private void OnLobbyDataUpdate(LobbyDataUpdate_t callback)
    {
        CSteamID lobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        //Debug.Log($"Обновлены данные лобби {lobbyId}");

        if (filteredLobbies.Contains(lobbyId))
        {
            string sessionName = SteamMatchmaking.GetLobbyData(lobbyId, "SessionName");
            string mapName = SteamMatchmaking.GetLobbyData(lobbyId, "MapName");
            string currentPlayersStr = SteamMatchmaking.GetLobbyData(lobbyId, "CurrentPlayers");
            string maxPlayersStr = SteamMatchmaking.GetLobbyData(lobbyId, "MaxPlayers");
            string id = SteamMatchmaking.GetLobbyData(lobbyId, "ID");

          int currentPlayers = 0;
            int maxPlayers = 0;

            if (!int.TryParse(currentPlayersStr, out currentPlayers))
            {
                Debug.LogWarning($"Не удалось преобразовать CurrentPlayers '{currentPlayersStr}' в число для лобби {lobbyId}");
            }

            if (!int.TryParse(maxPlayersStr, out maxPlayers))
            {
                Debug.LogWarning($"Не удалось преобразовать MaxPlayers '{maxPlayersStr}' в число для лобби {lobbyId}");
            }

            lobbyUIManager.UpdateOrCreateLobbyUI( lobbyId, sessionName, mapName, currentPlayers, maxPlayers, id);
        } 
    }
    private IEnumerator InvokeLobbyCreatedUIDelayed(CSteamID lobbyId)
    {
        yield return null; // подождать 1 кадр
        OnLobbyCreatedUI?.Invoke(lobbyId);
    }
    public void UpdateLobbyListUI()
    {
        foreach (var lobbyId in filteredLobbies)
        {
            string sessionName = SteamMatchmaking.GetLobbyData(lobbyId, "SessionName"); 
            string mapName = SteamMatchmaking.GetLobbyData(lobbyId, "MapName");
            string maxPlayersStr = SteamMatchmaking.GetLobbyData(lobbyId, "MaxPlayers");
            string currentPlayersStr = SteamMatchmaking.GetLobbyData(lobbyId, "CurrentPlayers");
            string expStr = SteamMatchmaking.GetLobbyData(lobbyId, "Experience");
            string id = SteamMatchmaking.GetLobbyData(lobbyId, "ID");
            int exp = 0;
            int.TryParse(expStr, out exp);

            int maxPlayers = 0;
            int currentPlayers = 0;

            int.TryParse(maxPlayersStr, out maxPlayers);
            int.TryParse(currentPlayersStr, out currentPlayers);

            // Если нужно — передай exp в твой UI:
            lobbyUIManager.UpdateOrCreateLobbyUI(lobbyId, sessionName, mapName, currentPlayers, maxPlayers, id);


        }
    }
    public void UpdateMyLobbyData()
    {
        if (CurrentLobbyID.IsValid())
        {
            SteamMatchmaking.SetLobbyData(CurrentLobbyID, "SessionName", networkGameSession.sessionName);
            SteamMatchmaking.SetLobbyData(CurrentLobbyID, "MapName", networkGameSession.mapName);
            SteamMatchmaking.SetLobbyData(CurrentLobbyID, "ID", networkGameSession.sessionId);
            SteamMatchmaking.SetLobbyData(CurrentLobbyID, "MaxPlayers", networkGameSession.maxPlayers.ToString());
            SteamMatchmaking.SetLobbyData(CurrentLobbyID, "CurrentPlayers", networkGameSession.syncedPlayers.Count.ToString());
         
           
        }
    }
    public void CloseLobby() 
    {
        SteamMatchmaking.LeaveLobby(CurrentLobbyID);
        OnLobbyEmpty?.Invoke(CurrentLobbyID);
        CurrentLobbyID = CSteamID.Nil;
    }

    public void DestroySessionAndCloseLobby()
    {
        // 1️⃣ Уничтожаем сетевую сессию, если есть
        if (networkGameSession != null)
        {
            if (networkGameSession.gameObject != null)
            {
                NetworkServer.Destroy(networkGameSession.gameObject);
                Debug.Log("🧹 NetworkGameSession уничтожена");
            }
            networkGameSession = null;
        }

        // 2️⃣ Закрываем Steam лобби, если есть активный
        if (CurrentLobbyID != CSteamID.Nil)
        {
            SteamMatchmaking.LeaveLobby(CurrentLobbyID);
            Debug.Log("🚪 Вышли из Steam лобби: " + CurrentLobbyID);

            OnLobbyEmpty?.Invoke(CurrentLobbyID); // 🔑 ВАЖНО!
            CurrentLobbyID = CSteamID.Nil;
        }

        // 3️⃣ Остановим Mirror сервер, если он активен
        if (NetworkServer.active || NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
            Debug.Log("⛔ Сервер и клиент остановлены");
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
    private void OnApplicationQuit()
    {
        LeaveCurrentLobby();
        DestroySessionAndCloseLobby();
    }
    public void OnUserClickedLeaveLobbyButton()
    {
        DestroySessionAndCloseLobby();
    }
  
}
