using Steamworks;
using Mirror;
using UnityEngine;

using System.Collections.Generic;
using TMPro;

public class UISessionSelector : MonoBehaviour
{
    private int maxLobbyPlayer = 10;

    [SerializeField] private TextMeshProUGUI sessionsCountText;
    [SerializeField] private TextMeshProUGUI playersCountText;

    [SerializeField] private TMP_InputField sessionNameInput; 

    private Dictionary<string, GameSession> currentSessions = new();

    private Callback<LobbyCreated_t> lobbyCreatedCallback;
    private Callback<LobbyEnter_t> lobbyEnteredCallback;
    
    [SerializeField] private GameObject roomWindow; // Панель комнаты
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private Transform playersListContainer;
    [SerializeField] private GameObject playerNamePrefab; // Префаб с UI элементом для игрока (текст)

    private void ShowRoomWindow(string sessionName, List<NetworkPlayerProfile> players)
    {
        roomWindow.SetActive(true);
        roomNameText.text = $"SessionName: {sessionName}";

        // Очистка старых записей
        foreach (Transform child in playersListContainer)
            Destroy(child.gameObject);

        // Заполнение списка игроков
        foreach (var player in players)
        {
            var go = Instantiate(playerNamePrefab, playersListContainer);
            go.GetComponent<TextMeshProUGUI>().text = player.playerName;
        }
    }

    private void HideRoomWindow()
    {
        roomWindow.SetActive(false);
    }

    private void Start()
    {
        lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEnteredCallback = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        RefreshUI();
    }

    public void RefreshUI()
    {
        currentSessions = SessionManager.Instance.GetAllSessions();

        int sessionsCount = currentSessions.Count;
        int playersCount = 0;

        foreach (var session in currentSessions.Values)
        {
            playersCount += session.Players.Count;
        }

        sessionsCountText.text = $"Сессий: {sessionsCount}";
        playersCountText.text = $"Игроков: {playersCount}";
    }

    public void OnCreateSessionButtonClicked()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxLobbyPlayer);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult == EResult.k_EResultOK)
        {
            string lobbyID = callback.m_ulSteamIDLobby.ToString();
            Debug.Log("Лобби создано с ID: " + lobbyID);

            string sessionName = string.IsNullOrEmpty(sessionNameInput.text) ? "Map1" : sessionNameInput.text;

            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "session_name", sessionName);

            var session = SessionManager.Instance.CreateSession(lobbyID, sessionName);

            NetworkManager.singleton.StartHost();
           
            ShowRoomWindow(session.SessionName, session.Players);
            
            RefreshUI();
        }
        else
        {
            Debug.LogError("Ошибка при создании лобби: " + callback.m_eResult);
        }
    }

    public void OnJoinSessionButtonClicked(string lobbyID)
    {
        if (ulong.TryParse(lobbyID, out ulong steamLobbyID))
        {
            CSteamID lobbyCSteamID = new CSteamID(steamLobbyID);
            SteamMatchmaking.JoinLobby(lobbyCSteamID);
        }
        else
        {
            Debug.LogError("Неверный формат LobbyID");
        }
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        Debug.Log("Присоединились к лобби: " + callback.m_ulSteamIDLobby);

        string lobbyID = callback.m_ulSteamIDLobby.ToString();

        // Получаем имя сессии из Steam лобби (если нужно)
        string sessionName = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "session_name");

        if (!SessionManager.Instance.TryGetSession(lobbyID, out var session))
        {
            session = SessionManager.Instance.CreateSession(lobbyID, sessionName);
        }

        NetworkManager.singleton.StartClient();

        ShowRoomWindow(session.SessionName, session.Players);
        
        RefreshUI();
    }
}
