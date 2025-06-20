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
    private void OnEnable()
    {
        TryAttachToExistingSession();
    }

    public void SetSession(NetworkGameSession session)
    {
        
        currentSession = session;

        sessionNameText.text = $"Сессия: {currentSession.sessionName}";
        sessionId.text = $"ID: {currentSession.sessionId}";

        currentSession.syncedPlayers.Callback += OnPlayersListChanged;

        RefreshPlayersUI();
    }
    private void OnPlayersListChanged(SyncList<NetworkPlayerProfile>.Operation op, int index, NetworkPlayerProfile oldItem, NetworkPlayerProfile newItem)
    {
        Debug.Log($"📢 Игроки изменились: {op} в позиции {index}");
        RefreshPlayersUI();
    }
    public void RefreshPlayersUI()
    {
        Debug.Log("🔁 Обновляем список игроков");

        if (playersContainer == null)
        {
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
        
        RefreshSessionUI();

        if (!currentSession.isServer)
            return;

        // Всё готово — можно безопасно устанавливать
        var localSessionData = PlayerDataManager.Instance?.PlayerSessionData;

        if (localSessionData == null)
        {
            Debug.LogWarning("PlayerSessionData is null");
            return;
        }
        currentSession.SetRaceData(localSessionData.raceData);

        if (localSessionData.raceData == null)
        {
            Debug.LogWarning("RaceData is null. Вероятно, карта ещё не выбрана");
            return;
        }

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


    public void StartRace()
    {
        if (currentSession != null && currentSession.isServer)
            currentSession.StartRace();
    }
    public void AddBot()
    {
        if (currentSession != null && currentSession.isServer)
            currentSession.AddBot();
        else
            Debug.LogWarning("Добавлять бота может только хост!");
    }
}
