using UnityEngine;
using System.Collections.Generic;
using Steamworks;
using Edgegap;

public class SteamLobbyUIManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform context;               // Контейнер в ScrollView
    [SerializeField] private GameObject oneSessionPrefab;     // Префаб UI-панели лобби
    [SerializeField] private UISessionPanel rootPanel;
    public Dictionary<CSteamID, UIGameSessionPanel> activeSessions { get; set; } = new();
    public void Init(UISessionPanel panel)
    {
        rootPanel = panel;
    }
    /// <summary>
    /// Создаёт UI-префаб для новой сессии и добавляет его в список
    /// </summary>
    public void AddLobbyToUI(NetworkGameSession session)
    {
        if (activeSessions.ContainsKey(session.lobbyId))
            return;

        GameObject uiObj = Instantiate(oneSessionPrefab, context);
        UIGameSessionPanel panel = uiObj.GetComponent<UIGameSessionPanel>();

        panel.SetSessionData(session.sessionName, session.sessionId, session.mapName,
            session.syncedPlayers.Count, session.maxPlayers, session.lobbyId);

        panel.Init(rootPanel); // ВСЕГДА ИСПОЛЬЗУЕМ ПОЛЕ!

        activeSessions.Add(session.lobbyId, panel);
    }

    /// <summary>
    /// Обновляет UI сессии по ID
    /// </summary>
    public void UpdateLobbyUI(NetworkGameSession session)
    {
        if (activeSessions.TryGetValue(session.lobbyId, out UIGameSessionPanel panel))
        {
            panel.SetSessionData(session.sessionName, session.sessionId, session.mapName,
            session.syncedPlayers.Count, session.maxPlayers, session.lobbyId);
            
        }
        else
        {
            // СОЗДАЁМ НОВЫЙ!
            GameObject uiObj = Instantiate(oneSessionPrefab, context);
            panel = uiObj.GetComponent<UIGameSessionPanel>();
            panel.Init(rootPanel);

            panel.SetSessionData(session.sessionName, session.sessionId, session.mapName,
                session.syncedPlayers.Count, session.maxPlayers, session.lobbyId);


            activeSessions.Add(session.lobbyId, panel);

            // ❌ А `panel.Init(...)` ТУТ НЕТ!
        }
    }

    /// <summary>
    /// Удаляет UI-сессию по lobbyId
    /// </summary>
    public void RemoveLobby(CSteamID lobbyId)
    {
        if (!activeSessions.TryGetValue(lobbyId, out UIGameSessionPanel panel))
            return;

        Destroy(panel.gameObject);
        activeSessions.Remove(lobbyId);
    }
    public void UpdateOrCreateLobbyUI(CSteamID lobbyId, string sessionName, string mapName, int currentPlayers, int maxPlayers)
    {
        if (activeSessions.TryGetValue(lobbyId, out UIGameSessionPanel panel))
        {
            // Обновляем UI
            panel.SetSessionData(sessionName, lobbyId.m_SteamID.ToString(), mapName, currentPlayers, maxPlayers, lobbyId);
        }
        else
        {
            // Создаем новый UI элемент
            GameObject uiObj = Instantiate(oneSessionPrefab, context);
            panel = uiObj.GetComponent<UIGameSessionPanel>();
            panel.Init(rootPanel);
            panel.SetSessionData(sessionName, lobbyId.m_SteamID.ToString(), mapName, currentPlayers, maxPlayers, lobbyId);
            activeSessions.Add(lobbyId, panel);
        }
    }
}