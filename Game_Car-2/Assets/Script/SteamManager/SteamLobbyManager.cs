
using Mirror;
using Steamworks;
using UnityEngine;

public class SteamLobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject networkGameSessionPrefab; // Префаб объекта с NetworkGameSession

   
    private NetworkManager networkManager; // Менеджер сети Mirror
    private NetworkGameSession currentSession; // Текущая сетевая сессия игры

    protected Callback<LobbyCreated_t> lobbyCreated; // Колбэк на создание лобби в Steam
    protected Callback<LobbyEnter_t> lobbyEntered;   // Колбэк на вход в лобби Steam

    private CSteamID currentLobbyID; // Steam ID текущего лобби

    public System.Action<string> OnLobbyCreatedUI; // Событие для UI: передаем ID лобби

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
    }

    public void CreateLobby()
    {
        
        int maxPlayers = 20;
        
        // Создаем публичное лобби Steam с максимальным количеством игроков
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxPlayers);
    }

    private void OnLobbyCreated(LobbyCreated_t result)
    {
        // Проверяем, успешно ли создано лобби
        if (result.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Ошибка создания лобби: " + result.m_eResult);
            return; 
        }

        // Сохраняем Steam ID созданного лобби
        currentLobbyID = new CSteamID(result.m_ulSteamIDLobby);
        Debug.Log("Лобби создано: " + currentLobbyID);

        // Устанавливаем в данных лобби адрес хоста — SteamID пользователя, который создал лобби
        SteamMatchmaking.SetLobbyData(currentLobbyID, "HostAddress", SteamUser.GetSteamID().ToString());

        // Вызываем событие для UI, чтобы показать ID лобби игрокам
        OnLobbyCreatedUI?.Invoke(currentLobbyID.ToString());

        // Запускаем хост-сервер Mirror (сервер + клиент на одном ПК)
        networkManager.StartHost();

        // Создаем и запускаем объект с NetworkGameSession (синхронизированная сессия)
        SpawnNetworkGameSession();
    }

    private void OnLobbyEntered(LobbyEnter_t result)
    {
        // Получаем Steam ID лобби, в которое вошли
        currentLobbyID = new CSteamID(result.m_ulSteamIDLobby);
        Debug.Log("Вошли в лобби: " + currentLobbyID);

        // Если это сервер, выходим, так как сервер не должен подключаться как клиент
        if (NetworkServer.active) return;

        // Получаем адрес хоста (SteamID) из данных лобби
        string hostAddress = SteamMatchmaking.GetLobbyData(currentLobbyID, "HostAddress");

        // Устанавливаем адрес для подключения клиента Mirror (SteamID хоста)
        networkManager.networkAddress = hostAddress;

        // Запускаем клиентское подключение Mirror к серверу
        networkManager.StartClient();
    }

    // Метод для присоединения к лобби по ID (например, введенному в UI)
    public void JoinLobbyById(string lobbyId)
    {
        // Пытаемся преобразовать строку в ulong (SteamID)
        if (ulong.TryParse(lobbyId, out ulong parsedId))
        {
            // Присоединяемся к лобби с этим SteamID
            SteamMatchmaking.JoinLobby(new CSteamID(parsedId));
        }
        else
        {
            Debug.LogError("Неверный формат Lobby ID");
        }
    }

    // Создаем объект NetworkGameSession на сервере и запускаем его в сети
    void SpawnNetworkGameSession()
    {
        // Создаем объект из префаба
        GameObject sessionObj = Instantiate(networkGameSessionPrefab);

        // Получаем компонент NetworkGameSession на объекте
        currentSession = sessionObj.GetComponent<NetworkGameSession>();

        // Инициализируем данные сессии (например, ID и имя)
        currentSession.sessionId = SteamMatchmaking.GetLobbyData(currentLobbyID, "LobbyID"); // Здесь можно задать свой ID
        currentSession.sessionName = "Steam Lobby Session";

        // Запускаем объект в сети — теперь он синхронизируется между игроками
        NetworkServer.Spawn(sessionObj);
    }
}
