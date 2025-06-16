using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Mirror;

public class UIGameSession : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TextMeshProUGUI sessionId;
    [SerializeField] private TextMeshProUGUI sessionMapNeme;
    [SerializeField] private TextMeshProUGUI sessionPlayersValue;
    [SerializeField] private TextMeshProUGUI sessionMaxPlayersValue;
    [SerializeField] private Transform playersContainer;
    [SerializeField] private GameObject playerUIPrefab;


    private GameSession currentSession;

    // Устанавливаем сессию и обновляем UI
    public void SetSession(GameSession session)
    {
        currentSession = session;
       

        sessionNameText.text = $"Сессия: {session.SessionName}";
        sessionId.text = $"ID: {session.SessionId}";
       
        RefreshPlayersUI();
       
    }

    // Обновляем список игроков в UI
    public void RefreshPlayersUI()
    {
        // Очищаем текущие элементы
        foreach (Transform child in playersContainer)
            Destroy(child.gameObject);

        // Создаем UI элементы для каждого игрока
        foreach (var player in currentSession.Players)
        {
            var playerUIObj = Instantiate(playerUIPrefab, playersContainer);
            var playerUI = playerUIObj.GetComponent<UIOnePlayerOnSession>();

            if (playerUI != null)
                playerUI.Set(player);
        }
    }

    // Добавляем игрока и обновляем UI
    public void AddPlayer(NetworkPlayerProfile player)
    {
        currentSession.AddPlayer(player);
        RefreshPlayersUI();
    }

    // Удаляем игрока и обновляем UI (если понадобится)
    public void RemovePlayer(NetworkPlayerProfile player)
    {
        currentSession.RemovePlayer(player);
        RefreshPlayersUI();
    }
    // Проверяем, является ли локальный игрок хостом сессии
   
    
    private bool IsHost()
    {
        if (currentSession == null)
            return false;

        // NetworkClient.connection — это локальная сеть игрока
        // currentSession.HostConnection — соединение хоста
        return NetworkClient.connection == currentSession.HostConnection;
    }

    public void Update()
    {
        if (!IsHost())
        {
            Debug.LogWarning("Только хост может изменять настройки!");
            return;
        }
        else
        {

        var localSessionData = PlayerDataManager.Instance.PlayerSessionData;
        currentSession.SetRaceData(localSessionData.raceData);
        RefreshSessionUI();

        }
    
    }
    private void RefreshSessionUI()
    {
        sessionMapNeme.text = $"Map Name: {currentSession.MapName}";
        
        sessionPlayersValue.text = $"Max Players: {currentSession.maxPlayer}";

    }
    public void StartRaise()
    {
        
    }
}