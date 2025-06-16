
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class GameSession
{
    public string SessionName { get; set; }
    public string SessionId { get; set; }
    public int maxPlayer { get; set; }
    public string MapName { get;  set; }
    public RaceData CurrentRaceData { get;  set; }
 

    public List<NetworkPlayerProfile> Players = new List<NetworkPlayerProfile>();

    public bool RaceStarted = false;

    public NetworkConnection HostConnection; // Кто создал сессию

    public GameSession(string sessionId, string mapName)
    {
        SessionId = sessionId;
        SessionName = mapName;
    }

    public void AddPlayer(NetworkPlayerProfile player)
    {
        if (!Players.Contains(player))
            Players.Add(player);
    }

    public void RemovePlayer(NetworkPlayerProfile player)
    {
        if (Players.Contains(player))
            Players.Remove(player);
    }

    public bool AreAllPlayersReady()
    {
        return Players.Count > 0 && Players.All(p => p.isReady);
    }

    public void SetRaceData(RaceData raceData)
    {
        CurrentRaceData = raceData;
        MapName = raceData.SceneName;
        maxPlayer = raceData.MaxCar;
        
    }
}
