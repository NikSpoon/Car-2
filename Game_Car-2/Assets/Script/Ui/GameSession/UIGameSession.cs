using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Mirror;

public class UIGameSession : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TextMeshProUGUI sessionId;
    [SerializeField] private TextMeshProUGUI sessionMapName;
    [SerializeField] private TextMeshProUGUI sessionPlayersValue;
    [SerializeField] private TextMeshProUGUI sessionMaxPlayersValue;

    [SerializeField] private RectTransform playersContainer;
    [SerializeField] private GameObject playerUIPrefab;

    private NetworkGameSession currentSession;

    public void SetSession(NetworkGameSession session)
    {
        
        currentSession = session;

        sessionNameText.text = $"Сессия: {currentSession.sessionName}";
        sessionId.text = $"ID: {currentSession.sessionId}";

        RefreshPlayersUI();
        RefreshSessionUI();
    }

    public void RefreshPlayersUI()
    {
        Debug.Log("🔁 Обновляем список игроков");

        if (playersContainer == null)
        {
            Debug.LogWarning("❌ playersContainer не назначен");
            return;
        }

        if (currentSession == null)
        {
            Debug.LogWarning("❌ currentSession = null");
            return;
        }

        Debug.Log($"📋 Игроков в сессии: {currentSession.syncedPlayers.Count}");
        if (playersContainer == null)
            return;
        
        foreach (Transform child in playersContainer)
            Destroy(child.gameObject);

        foreach (var player in currentSession.syncedPlayers)
        {
            var playerUIObj = Instantiate(playerUIPrefab, playersContainer);
            var playerUI = playerUIObj.GetComponent<UIOnePlayerOnSession>();

            if (playerUI != null)
                playerUI.Set(player);
        }

    }

    public void RefreshSessionUI()
    {
        if (currentSession == null)
        {
            Debug.LogWarning("RefreshSessionUI: currentSession is null");
            return;
        }
        sessionMapName.text = $"Карта: {currentSession.mapName}";
        sessionPlayersValue.text = $"Игроков: {currentSession.syncedPlayers.Count}";
        sessionMaxPlayersValue.text = $"Макс: {currentSession.maxPlayers}";
    }

    public void Update()
    {
        if (currentSession == null)
        {
            TryAttachToExistingSession();
            Debug.LogWarning("currentSession is NULL");
            return;
        }

        if (!currentSession.isServer)
            return;

        var localSessionData = PlayerDataManager.Instance?.PlayerSessionData;

        if (localSessionData == null)
        {
            Debug.LogWarning("PlayerSessionData is null");
            return;
        }

        if (localSessionData.raceData == null)
        {
            Debug.LogWarning("RaceData is null. Вероятно, карта ещё не выбрана");
            return;
        }

        // Всё готово — можно безопасно устанавливать
        currentSession.SetRaceData(localSessionData.raceData);

        RefreshPlayersUI();
        RefreshSessionUI();
    }


    public void StartRace()
    {
        if (currentSession != null && currentSession.isServer)
            currentSession.StartRace();
    }
    private void OnEnable()
    {
        TryAttachToExistingSession();
    }

    private void TryAttachToExistingSession()
    {
        var session = FindFirstObjectByType<NetworkGameSession>(FindObjectsInactive.Include);
        if (session != null && session.sessionId != null)
        {
            SetSession(session);
           
        }
        else
        {
            Debug.LogWarning("❌ Не удалось найти активную NetworkGameSession при активации панели");
        }
    }
}
