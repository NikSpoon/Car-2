using UnityEngine;

[CreateAssetMenu(fileName = "GameSessionData", menuName = "Game/Session Data")]
public class GameSessionData : ScriptableObject
{
    public RaceData selectedRace;
    public int selectedMusicIndex;
    public int selectedCarIndex;

    public void Clear()
    {
        selectedRace = null;
        selectedMusicIndex = -1;
        selectedCarIndex = -1;
    }
}