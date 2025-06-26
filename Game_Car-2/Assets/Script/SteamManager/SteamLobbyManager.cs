
using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
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
        

        // Если это сервер, выходим, так как сервер не должен подключаться как клиент
        if (NetworkServer.active) return;

        // Получаем адрес хоста (SteamID) из данных лобби
        string hostAddress = SteamMatchmaking.GetLobbyData(currentLobbyID, "HostAddress");

        // Устанавливаем адрес для подключения клиента Mirror (SteamID хоста)
        networkManager.networkAddress = hostAddress;

        // Запускаем клиентское подключение Mirror к серверу
        networkManager.StartClient();

        StartCoroutine(WaitForSessionOnClient());

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

        // 1) Инстансиируем префаб
        GameObject sessionObj = Instantiate(networkGameSessionPrefab);
        var session = sessionObj.GetComponent<NetworkGameSession>();

        // 2) Формируем короткий ID из SteamLobbyID
        string shortId = (currentLobbyID.m_SteamID % 100000).ToString("D5");
        // Имя сессии можно взять, например, из профиля хоста
        string sessionName = PlayerDataManager.Instance.PlayerProfile.playerName;

        // 3) Готовим данные до спауна
        session.PrepareSession(shortId, sessionName);

        // 4) Спауним сетевой объект (SyncVar будет передан клиентам автоматически)
        NetworkServer.Spawn(sessionObj);
      
        // Сохраняем для ссылок в этом классе
        currentSession = session;
    }
    private IEnumerator ShowMySession()
    {
        
        float logInterval = 0.5f;
        float timeSinceLastLog = 0f;
        int attemptCount = 0;

        while (true)
        {
            var session = Object.FindFirstObjectByType<NetworkGameSession>();

            if (session != null)
            {
                if (!string.IsNullOrEmpty(session.sessionId))
                {
                    Debug.Log($"✅ NetworkGameSession найден и проинициализирован. sessionId = {session.sessionId}");

                    var sessionPanel = Object.FindFirstObjectByType<UISessionPanel>();
                    if (sessionPanel != null)
                    {
                        sessionPanel.ShowSessions(new List<NetworkGameSession> { session });
                    }
                    else
                    {
                        Debug.LogError("❌ UISessionPanel не найден в сцене!");
                    }

                    yield break; // Завершаем корутину
                }
                else
                {
                    if (timeSinceLastLog >= logInterval)
                    {
                        Debug.LogWarning($"⏳ Найден NetworkGameSession, но sessionId ещё пустой. Ожидание... [{++attemptCount}]");
                        timeSinceLastLog = 0f;
                    }
                }
            }
            else
            {
                if (timeSinceLastLog >= logInterval)
                {
                    Debug.LogWarning($"🔍 NetworkGameSession пока не найден... [{++attemptCount}]");
                    timeSinceLastLog = 0f;
                }
            }

            timeSinceLastLog += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator WaitForSessionOnClient()
    {
        float timeSinceLastLog = 0f;
        int attemptCount = 0;

        while (true)
        {
            var session = Object.FindFirstObjectByType<NetworkGameSession>();

            if (session != null && !string.IsNullOrEmpty(session.sessionId))
            {
                Debug.Log($"✅ [CLIENT] Получена сетевая сессия: {session.sessionId}");

                var sessionPanel = Object.FindFirstObjectByType<UISessionPanel>();
                if (sessionPanel != null)
                {
                    sessionPanel.ShowSessions(new List<NetworkGameSession> { session });
                }
                else
                {
                    Debug.LogError("❌ [CLIENT] UISessionPanel не найден!");
                }

                yield break;
            }

            if (timeSinceLastLog > 0.5f)
            {
                Debug.LogWarning($"🔍 [CLIENT] Ожидание появления NetworkGameSession... попытка [{++attemptCount}]");
                timeSinceLastLog = 0f;
            }

            timeSinceLastLog += Time.deltaTime;
            yield return null;
        }
        
    }
    
}
