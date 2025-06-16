using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    private Dictionary<string, GameSession> sessions = new Dictionary<string, GameSession>();
    
   
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Создать новую сессию с уникальным ID и именем карты
    public GameSession CreateSession(string sessionId, string mapName)
    {
        if (sessions.ContainsKey(sessionId))
        {
            Debug.LogWarning($"Сессия с ID {sessionId} уже существует.");
            return sessions[sessionId];
        }

        var newSession = new GameSession(sessionId, mapName);
        sessions.Add(sessionId, newSession);
        Debug.Log($"Создана сессия {sessionId} с картой {mapName}");
        return newSession;
    }

    // Получить сессию по ID
    public bool TryGetSession(string sessionId, out GameSession session)
    {
        return sessions.TryGetValue(sessionId, out session);
    }

    // Закрыть (удалить) сессию
    public void CloseSession(string sessionId)
    {
        if (sessions.ContainsKey(sessionId))
        {
            sessions.Remove(sessionId);
            Debug.Log($"Сессия {sessionId} закрыта и удалена.");
        }
        else
        {
            Debug.LogWarning($"Попытка закрыть несуществующую сессию с ID {sessionId}");
        }
    }

    // Получить количество активных сессий
    public int GetActiveSessionCount()
    {
        return sessions.Count;
    }
    public Dictionary<string, GameSession> GetAllSessions()
    {
        return sessions;
    }
}