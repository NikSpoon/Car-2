using Mirror;
using UnityEngine;

public class NetworkBotProfile : NetworkPlayerProfile
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        isBot = true;
    }

    [Server]
    public void InitializeBot(AIProfile botData)
    {
        playerName = botData.playerName;
        money = botData.money;
        xp = botData.Xp;
        level = 1;
        selectedCarIndex = botData.selectedCarIndex;
        selectedBodyUpgradeIndex = botData.selectedBodyUpgradeIndex;
        playerID = botData.AI_ID;
        isOnline = true;
        isReady = false;
        isBot = true;

        Debug.Log($"[SERVER] Инициализирован бот: {playerName}");
    }

    [Server]
    public void RemoveFromSession(NetworkGameSession session)
    {
        if (session == null)
        {
            Debug.LogWarning("RemoveFromSession: сессия равна null");
            return;
        }

        session.RemovePlayer(this);

        // Уничтожаем объект бота в сети
        NetworkServer.Destroy(gameObject);

        Debug.Log($"[SERVER] Бот {playerName} удалён из сессии и уничтожен");
    }
}
