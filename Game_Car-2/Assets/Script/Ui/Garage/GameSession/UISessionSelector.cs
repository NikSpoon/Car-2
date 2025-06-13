using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISessionSelector : MonoBehaviour
{
    [SerializeField] private Text sessionsCountText;  // UI элемент для показа количества сессий
    [SerializeField] private Text playersCountText;   // UI элемент для показа количества игроков (без ботов)

    private Dictionary<string, GameSession> currentSessions = new();

    private void Start()
    {
        RefreshUI();
    }

    // Вызывается, чтобы обновить UI данные
    public void RefreshUI()
    {
        currentSessions = SessionManager.Instance.GetAllSessions();

        int sessionsCount = currentSessions.Count;
        int playersCount = 0;

        foreach (var session in currentSessions.Values)
        {
            // Считаем только игроков, без ботов
            playersCount += CountRealPlayers(session);
        }

        sessionsCountText.text = $"Сессий: {sessionsCount}";
        playersCountText.text = $"Игроков: {playersCount}";
    }

    // Кнопка создания новой сессии
    public void OnCreateSessionButtonClicked()
    {
        string newSessionId = GenerateUniqueSessionId();
        string defaultMapName = "Map1"; // или получить из UI

        SessionManager.Instance.CreateSession(newSessionId, defaultMapName);
        RefreshUI();
    }

    // Кнопка присоединения к сессии по ID
    public void OnJoinSessionButtonClicked(string sessionId, PlayerProfile player)
    {
        if (SessionManager.Instance.TryGetSession(sessionId, out var session))
        {
            session.AddPlayer(player);
            Debug.Log($"Игрок {player.playerName} присоединился к сессии {sessionId}");
            RefreshUI();
        }
        else
        {
            Debug.LogWarning($"Сессия с ID {sessionId} не найдена.");
        }
    }

    // Подсчет реальных игроков в сессии (без ботов)
    private int CountRealPlayers(GameSession session)
    {
        // Тут предполагается, что имена ботов содержат "Bot" или у тебя есть другая логика
        int count = 0;
        foreach (var playerName in session.Players)
        {
            if (!playerName.Contains("Bot")) // Или более точная проверка
                count++;
        }
        return count;
    }

    // Генерация уникального ID для сессии (простой пример)
    private string GenerateUniqueSessionId()
    {
        return "Session" + System.DateTime.Now.Ticks;
    }
}
