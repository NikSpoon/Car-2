using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelTable", menuName = "Game/Level Table")]
public class LevelTableSO : ScriptableObject
{
    public List<LevelData> levels;
}
[System.Serializable]
public class LevelData
{
    public int level;
    public int requiredXp;
}