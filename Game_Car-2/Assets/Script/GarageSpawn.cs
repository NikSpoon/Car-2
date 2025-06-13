using UnityEngine;

public class GarageSpawn : MonoBehaviour
{

    public CarDatabase carDatabase;
    [SerializeField] private Transform _start;

    private GameObject currentCarInstance;
    private PlayerProfile profile;

    private int lastSelectedCarIndex;
    private int lastSelectedBodyUpgradeIndex;

    private void Awake()
    {
        profile = PlayerDataManager.Instance.PlayerProfile;

        SpawnCar();
    }

    private void Update()
    {
        
        if (profile.selectedCarIndex != lastSelectedCarIndex || profile.selectedBodyUpgradeIndex != lastSelectedBodyUpgradeIndex)
        {
            
            SpawnCar();
        }
    }

    private void SpawnCar()
    {
        if (currentCarInstance != null)
        {
            Destroy(currentCarInstance);
        }

        GameObject upgradePrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex];
        currentCarInstance = Instantiate(upgradePrefab, _start.position, _start.rotation,transform);
      

        lastSelectedCarIndex = profile.selectedCarIndex;
        lastSelectedBodyUpgradeIndex = profile.selectedBodyUpgradeIndex;
    }

}
