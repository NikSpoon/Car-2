using UnityEngine;

public class PlayerSessionData 
{
    public string pendingSessionId { get; private set;}
    public string pendingMap { get; private set; }
    public RaceData raceData { get; private set; }

    public void GetInstansRaceData(RaceData race)
    {
        raceData = race;
    }
}
