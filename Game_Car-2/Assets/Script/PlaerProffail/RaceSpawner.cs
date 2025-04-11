using UnityEngine;

public class RaceSpawner : MonoBehaviour
{
    public CarDatabase carDatabase;
    [SerializeField] private Transform _start;

    void Start()
    {
        var profile = PlayerDataManager.Instance.playerProfile;

        GameObject carPrefab = carDatabase.carPrefabs[profile.selectedCarIndex];
        GameObject upgradePrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex];

        GameObject car = Instantiate(carPrefab, _start.position, _start.rotation);
        //GameObject upgrade = Instantiate(upgradePrefab, car.transform);
    }
}