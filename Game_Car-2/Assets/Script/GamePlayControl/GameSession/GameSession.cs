using System;
using System.Collections.Generic;
using System.Linq;

public class GameSession
{
    public string SessionId { get; private set; }
    public string MapName { get; private set; }
    
    public List<string> Players = new();

    public GameSession(string sessionId, string mapName)
    {
        SessionId = sessionId;
        MapName = mapName;
    }

    public void AddPlayer(PlayerProfile playerProfile)
    {
        if (!Players.Contains(playerProfile.playerName))
            Players.Add(playerProfile.playerName);
    }

    public void RemovePlayer(PlayerProfile playerProfile)
    {
        Players.Remove(playerProfile.playerName);
    }
    public void AddBot(AIProfile bot)
    {
        if (!Players.Contains(bot.playerName))
            Players.Add(bot.playerName);
    }

    public void RemoveBot(AIProfile bot)
    {
        Players.Remove(bot.playerName);
    }
    public bool IsEmpty => Players.Count == 0;
}
