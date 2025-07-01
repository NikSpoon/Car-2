using UnityEngine;
using TMPro;

using Mirror;
using System.Collections;


public class UIGameSession : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TextMeshProUGUI sessionId;
    [SerializeField] private TextMeshProUGUI sessionMapName;
    [SerializeField] private TextMeshProUGUI sessionPlayersValue;
    [SerializeField] private TextMeshProUGUI sessionMaxPlayersValue;

    [SerializeField] private TextMeshProUGUI startTimer;
    [SerializeField] private GameObject startPanel;

    [SerializeField] private RectTransform playersContainer;
    [SerializeField] private GameObject playerUIPrefab;


    private NetworkGameSession currentSession;

    private void OnEnable()
    {
        startPanel.SetActive(false);


    }

    public void SetSession(NetworkGameSession session)
    {
        if (currentSession != null)
            currentSession.syncedPlayers.Callback -= OnPlayersListChanged;


        currentSession = session;

        sessionNameText.text = $"Сессия: {currentSession.sessionName}";
        sessionId.text = $"ID: {currentSession.sessionId}";

        currentSession.syncedPlayers.Callback += OnPlayersListChanged;

        RefreshPlayersUI();
    }
    private void OnPlayersListChanged(SyncList<NetworkPlayerProfile>.Operation op, int index, NetworkPlayerProfile oldItem, NetworkPlayerProfile newItem)
    {
        //  Debug.Log($"📢 Игроки изменились: {op} в позиции {index}");
        RefreshPlayersUI();
    }
    public void RefreshPlayersUI()
    {
        // Debug.Log("🔁 Обновляем список игроков");

        if (playersContainer == null)
        {
            return;
        }

        if (currentSession == null)
        {
            Debug.LogWarning("❌ currentSession = null");
            return;
        }

        // Debug.Log($"📋 Игроков в сессии: {currentSession.syncedPlayers.Count}");
        if (playersContainer == null)
            return;

        foreach (Transform child in playersContainer)
            Destroy(child.gameObject);

        foreach (var player in currentSession.syncedPlayers)
        {
            var playerUIObj = Instantiate(playerUIPrefab, playersContainer);
            var playerUI = playerUIObj.GetComponent<UIOnePlayerOnSession>();

            if (playerUI != null  )
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
        sessionNameText.text = $"Сессия: {currentSession.sessionName}";
        sessionMapName.text = $"Карта: {currentSession.mapName}";
        sessionPlayersValue.text = $"Игроков: {currentSession.syncedPlayers.Count}";
        sessionMaxPlayersValue.text = $"Макс: {currentSession.maxPlayers}";
    }

    public void Update()
    {
        if (currentSession == null) return;
        RefreshSessionUI();
        // Проверяем, что этот объект управляется сервером и сервер активен
        if (!currentSession.isServer || !NetworkServer.active)
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

    public void UpdateTimer(int timeLeft)
    {
        if (!startPanel)
        {
            return;
        }
        startPanel.SetActive(true);

        if (timeLeft > 0)
        {
            startTimer.text = $"{timeLeft}";
        }
        else
        {
            startTimer.text = "Поехали!";
            StartCoroutine(HidePanelAfterDelay());
        }
    }

    private IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        startPanel.SetActive(false);
    }


    public void StartRace()
    {
        if (currentSession != null && currentSession.isServer)
        {
            currentSession.RequestStartRace();
            //PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToGameplay);
        }
    }
    public void AddBot()
    {
        if (currentSession != null && currentSession.isServer)
            currentSession.AddBot();
        else
            Debug.LogWarning("Добавлять бота может только хост!");
    }
    public void OnLeaveLobbyButtonClicked()
    {
       
        if (currentSession.SteamLobbyManager != null)
        {
            var player = FindFirstObjectByType<NetworkPlayerProfile>();
            currentSession.SteamLobbyManager.LeaveCurrentLobby();
            currentSession.RemovePlayer(player);

        }

    }
    private void OnDisable()
    {
        if (currentSession != null)
        {
            
            RefreshSessionUI();
            currentSession.uIGameSession = null;
        }
    }
}

