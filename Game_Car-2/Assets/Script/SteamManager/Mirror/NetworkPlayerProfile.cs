using Mirror;
using Steamworks;
using System;
using System.Collections;
using UnityEngine;

public class NetworkPlayerProfile : NetworkBehaviour
{
    [SyncVar] public string playerName;
    [SyncVar] public int level;
    [SyncVar] public int money;
    [SyncVar] public int xp;
    [SyncVar] public int selectedCarIndex;
    [SyncVar] public int selectedBodyUpgradeIndex;
    [SyncVar] public int playerID;
    [SyncVar] public bool isReady;
    [SyncVar] public bool isOnline;

    [SyncVar] public bool isBot;

    [SyncVar] public bool isBotRandom;

    private GameObject carInstance;

    private int _cachedCarIndex = -1;
    private int _cachedCarUpgradeIndex = -1;

    private bool _isReadyToSendCommands = false;
    private bool _isWaitingForReady = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public override void OnStartClient()
    {
        base.OnStartClient();

        var session = FindFirstObjectByType<NetworkGameSession>();
        if (session != null)
        {
            if (isLocalPlayer)
            {
                CmdRegisterPlayer(session);
            }
        }
    }

    [Command]
    void CmdRegisterPlayer(NetworkGameSession session)
    {

        if (session != null)
        {
            Initialize(PlayerDataManager.Instance.PlayerProfile);
            // Debug.Log($"👤 Игрок добавлен в сессию: {playerName}");
            session.AddPlayer(this);
        }
    }
    public override void OnStopServer()
    {
        base.OnStopServer();

        var session = FindFirstObjectByType<NetworkGameSession>();
        if (session != null)
        {
            session.RemovePlayer(this);

        }
    }
    public override void OnStartLocalPlayer()
    {
        var steamLobbyManager = FindFirstObjectByType<SteamLobbyManager>();
        base.OnStartLocalPlayer();
        
        CmdJoinLobbyById(steamLobbyManager.CurrentLobbyID);
   
        Debug.Log("✅ OnStartLocalPlayer вызван для " + SteamUser.GetSteamID());
    }
    // Вызывается на сервере при создании игрока
    [Server]
    public void Initialize(PlayerProfile profile)
    {
        playerName = profile.playerName;
        level = profile.levl;
        money = profile.money;
        xp = profile.Xp;
        selectedCarIndex = profile.selectedCarIndex;
        playerID = profile.playerID;
        isOnline = profile.isOnline;
    }

    // Вызывается на клиенте, чтобы обновить сервер
    public void SendProfileToServer(PlayerProfile profile)
    {
        CmdUpdateProfile(profile.playerName, profile.money, profile.Xp, profile.levl, profile.selectedCarIndex);
    }

    [Command]
    private void CmdUpdateProfile(string name, int newMoney, int newXp, int newLevel, int newCarIndex)
    {
        playerName = name;
        money = newMoney;
        xp = newXp;
        level = newLevel;
        selectedCarIndex = newCarIndex;

        Debug.Log($"[SERVER] Обновлён профиль: {name} | ₽: {money}, XP: {xp}, Уровень: {level}");
    }

    // Полезный метод: обновляет локальный профиль по данным с сервера
    public void CopyToLocalProfile(PlayerProfile local)
    {
        local.playerName = playerName;
        local.levl = level;
        local.money = money;
        local.Xp = xp;
        local.selectedCarIndex = selectedCarIndex;
        local.playerID = playerID;
        local.isOnline = isOnline;
    }
    [Command]
    public void CmdSetReady(bool value)
    {
        isReady = value;
    }
    void Update()
    {
        if (!isLocalPlayer) return;

        // Если ещё не готов — запускаем ожидание один раз
        if (!_isReadyToSendCommands)
        {
            if (!_isWaitingForReady)
            {
                StartCoroutine(WaitUntilClientReady());
                _isWaitingForReady = true;
            }
            return;
        }
        int currentCarIndex = PlayerDataManager.Instance.PlayerProfile.selectedCarIndex;
        int currentselectedBodyUpgradeIndex = PlayerDataManager.Instance.PlayerProfile.selectedBodyUpgradeIndex;

        if (currentCarIndex != _cachedCarIndex || currentselectedBodyUpgradeIndex != _cachedCarUpgradeIndex)
        {
            CmdSetCarIndex(currentCarIndex, currentselectedBodyUpgradeIndex);
            _cachedCarIndex = currentCarIndex;
            _cachedCarUpgradeIndex = currentselectedBodyUpgradeIndex;
        }
    }
    [Command]
    void CmdSetCarIndex(int index, int index2)
    {
        selectedCarIndex = index;
        selectedBodyUpgradeIndex = index2;
        //Debug.Log($"[SERVER] {playerName} выбрал машину #{index},{index2}");
    }
    [Server]
    public void SpawnSelectedCar(NetworkConnectionToClient conn, Transform spawnPoint, GameObject carPref, out bool Bot, out bool Random)
    {
        Bot = isBot;
        Random = isBotRandom;
        if (Bot)
        {

            return;
        }
        var car = Instantiate(carPref, spawnPoint.position, spawnPoint.rotation);
        NetworkServer.Spawn(car, conn); // Владение — у игрока
    }
    private IEnumerator WaitUntilClientReady()
    {
        // Безопасная задержка
        yield return new WaitForSeconds(0.5f);

        while (!NetworkClient.ready)
        {
            yield return null;
        }

        _isReadyToSendCommands = true;
        // Debug.Log($"✅ Клиент {playerName} теперь готов к командам.");
    }

    [Command]
    public void CmdJoinLobbyById(CSteamID Id)
    {
        Initialize(PlayerDataManager.Instance.PlayerProfile);
        var session = FindFirstObjectByType<NetworkGameSession>();
        if (session != null && session.lobbyId == Id)
        {
            session.AddPlayer(this); // твой метод добавления
        }
    }
}
