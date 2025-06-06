using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRaceDatabase", menuName = "Race/RaceDatabase")]
public class RaceDatabase : ScriptableObject
{
    [SerializeField] private List<RaceData> races;

    public List<RaceData> Races => races;
}