
using System.Collections.Generic;

[System.Serializable]
public class PlayerProfile
{

    public string playerName;
    public string password;
    public int playerID;
    public int levl;
    public bool isOnline;

    public int money = 0; 
    public int Xp = 0;

    // Для хранения доступности машины (ключ - индекс машины)
    // Значение - массив bool для 4 апгрейдов (true - доступен)
    public Dictionary<int, bool[]> purchasedCarsUpgrades = new Dictionary<int, bool[]>();

    public int selectedCarIndex { get; private set; }
    public int selectedBodyUpgradeIndex { get; private set; }


    public void GetNewProfile(string name, string pass)
    {
        playerName = name;
        password = pass;
        playerID = 00000000;
        money = 500;
        Xp = 100;
        levl = 1;
        InitDefaultCars();

        selectedCarIndex = 0;
        selectedBodyUpgradeIndex = 0;
    }
    private void InitDefaultCars()
    {
        purchasedCarsUpgrades.Clear();
        purchasedCarsUpgrades[0] = new bool[4] { true, false, false, false };
    }
    public void ProfileSelectedCar(int selectedCar, int selectedBodyUpgrade)
    {
        ProfileSelectedCarIndex(selectedCar);
        ProfileSelectedBodyIndex(selectedBodyUpgrade);
    }
    public void ProfileSelectedCarIndex(int selectedCar)
    {
        selectedCarIndex = selectedCar;
    }
    public void ProfileSelectedBodyIndex( int selectedBodyUpgrade)
    {
        selectedBodyUpgradeIndex = selectedBodyUpgrade;
    }
}

