using UnityEngine;

public class RaceSpawner : MonoBehaviour
{
    public CarDatabase carDatabase;
    [SerializeField] private Transform _start;
    private void Awake()
    {
            
        var profile = PlayerDataManager.Instance.playerProfile;

        GameObject carPrefab = carDatabase.carPrefabs[profile.selectedCarIndex];
        GameObject upgradePrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex];

        GameObject car = Instantiate(upgradePrefab, _start.position, _start.rotation);
    }
   
}