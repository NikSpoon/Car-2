using UnityEngine;

[CreateAssetMenu(fileName = "GameSessionData", menuName = "Game/SessionData")]
public class GameSessionData : ScriptableObject
{
    public string raceSceneName;

    public void SetRaceMap(RaceData race)
    {
        if (race == null)
        {
            Debug.LogError("Попытка установить гонку, но RaceData — null.");
            
            return;
        }
        PlayerDataManager.Instance.PlayerSessionData.GetInstansRaceData(race);
        raceSceneName = race.SceneName; 
    }

    public void Clear()
    {
        raceSceneName = string.Empty;
    }
}