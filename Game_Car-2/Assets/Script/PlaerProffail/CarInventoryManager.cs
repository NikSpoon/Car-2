
using UnityEngine;

public class CarInventoryManager
{
    private PlayerProfile _profile;
    private CarDatabase _database;


    public void Init (PlayerProfile profile, CarDatabase database)
    {
        _profile = profile;
        _database = database;
    }

    public int TrySelectCarAndUpgrade(int carIndex, int upgradeIndex)
    {
        // Проверяем, куплена ли машина
        bool carPurchased = _profile.purchasedCarsUpgrades.ContainsKey(carIndex);

        if (!carPurchased)
        {
            return GetPrice(carIndex, upgradeIndex); 
        }

        // Машина куплена — проверим апгрейд
        bool[] upgrades = _profile.purchasedCarsUpgrades[carIndex];
        if (upgradeIndex < 0 || upgradeIndex >= upgrades.Length)
        {
            Debug.LogWarning($"Некорректный индекс апгрейда: {upgradeIndex}");
           return 0; ;
        }

        if (!upgrades[upgradeIndex])
        {
            return GetPrice(carIndex, upgradeIndex);
        }

        _profile.ProfileSelectedCar(carIndex, upgradeIndex);
        
        return 0;
    }
    
    private int GetPrice(int carIndex, int upgradeIndex)
    {
        return _database.carUpgrades[carIndex].upgrades[upgradeIndex].price;
    }
    public void UnlockCar(int carIndex)
    {
        if (!_profile.purchasedCarsUpgrades.ContainsKey(carIndex))
        {
            _profile.purchasedCarsUpgrades[carIndex] = new bool[4];
            _profile.purchasedCarsUpgrades[carIndex][0] = true; 
        }
    }

    public void UnlockUpgrade(int carIndex, int upgradeIndex)
    {
        if (!_profile.purchasedCarsUpgrades.ContainsKey(carIndex))
        {
            _profile.purchasedCarsUpgrades[carIndex] = new bool[4];
        }

        _profile.purchasedCarsUpgrades[carIndex][upgradeIndex] = true;
    }
}
