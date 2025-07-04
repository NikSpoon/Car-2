using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarUpgradeList
{
    public List<CarUpgrade> upgrades; 
}

[System.Serializable]
public class CarUpgrade
{
    public GameObject upgradePrefab;  
    public int price;                 
}
[CreateAssetMenu(fileName = "CarDatabase", menuName = "GameData/Car Database")]
public class CarDatabase : ScriptableObject
{
    public List<GameObject> carPrefabs; // сами машины
    public List<CarUpgradeList> carUpgrades; // вложенные списки
}
