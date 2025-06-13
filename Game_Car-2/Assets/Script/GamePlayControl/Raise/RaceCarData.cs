using UnityEngine;

[System.Serializable]
public class RaceCarData
{
    public GameObject carPrefab;   
    public CarStats stats;         

    public RaceCarData(GameObject carPrefab)
    {
        this.carPrefab = carPrefab;
        this.stats = new CarStats();
    }
}