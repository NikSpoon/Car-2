using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarUpgradeList
{
    public List<GameObject> upgrades; // апгрейды для одной машины
}

[CreateAssetMenu(fileName = "CarDatabase", menuName = "GameData/Car Database")]
public class CarDatabase : ScriptableObject
{
    public List<GameObject> carPrefabs; // сами машины
    public List<CarUpgradeList> carUpgrades; // вложенные списки
}
