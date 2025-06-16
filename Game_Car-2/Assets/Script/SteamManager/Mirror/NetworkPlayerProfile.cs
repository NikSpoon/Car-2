using Mirror;
using UnityEngine;

public class NetworkPlayerProfile : NetworkBehaviour
{
    [SyncVar] public string playerName;
    [SyncVar] public int level;
    [SyncVar] public int money;
    [SyncVar] public int xp;
    [SyncVar] public int selectedCarIndex;
    [SyncVar] public int playerID;
    [SyncVar] public bool isReady;
    [SyncVar] public bool isOnline;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"Игрок подключился: {playerName}, уровень: {level}");

        // Если это наш локальный игрок, копируем данные с сервера в локальный профиль
        if (isLocalPlayer)
        {
            CopyToLocalProfile(PlayerDataManager.Instance.PlayerProfile);

        }
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
    void CmdUpdateProfile(string name, int newMoney, int newXp, int newLevel, int newCarIndex)
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
}
