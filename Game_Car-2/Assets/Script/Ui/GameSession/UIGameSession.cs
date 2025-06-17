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

    [SerializeField] private Transform playersContainer;
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
        sessionMapName.text = $"Карта: {currentSession.mapName}";
        sessionPlayersValue.text = $"Игроков: {currentSession.syncedPlayers.Count}";
        sessionMaxPlayersValue.text = $"Макс: {currentSession.maxPlayers}";
    }

    public void Update()
    {
        if (currentSession == null || !currentSession.isServer)
            return;

        var localSessionData = PlayerDataManager.Instance.PlayerSessionData;
        currentSession.SetRaceData(localSessionData.raceData);
        RefreshSessionUI();
    }

    public void StartRace()
    {
        if (currentSession != null && currentSession.isServer)
            currentSession.StartRace();
    }
}
