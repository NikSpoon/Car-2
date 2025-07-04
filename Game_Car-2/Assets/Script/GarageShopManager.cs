using UnityEngine;

public class GarageShopManager : MonoBehaviour
{
    [SerializeField] private GarageUISelectCar _uISelectCar;

    private string text;
    private CarInventoryManager _inventory;
    private MoneyManager _money;
    private void Awake()
    {

        _inventory = PlayerDataManager.Instance.CarInventory;
        _money = PlayerDataManager.Instance.Money;

    }

    public bool TryBuyCar()
    {
        var cost = _uISelectCar.cost;
        var carIndex = _uISelectCar.carIndex;
        if (_money.TrySend(cost))
        {
            _inventory.UnlockCar(carIndex);
            text = $"Куплена машина {carIndex} за {cost}";
            return true;
        }

        text = $"Не хватает денег на машину {carIndex}";
        return false;
    }

    public bool TryBuyUpgrade()
    {
        int cost = _uISelectCar.cost;
        var carIndex = _uISelectCar.carIndex;
        var upgradeIndex = _uISelectCar.bodyIndex;
        if (_money.TrySend(cost))
        {
            _inventory.UnlockUpgrade(carIndex, upgradeIndex);
            text = $"Куплен апгрейд {upgradeIndex} для машины {carIndex} за {cost}";
            return true;
        }

        text = $"Не хватает денег на апгрейд {upgradeIndex} машины {carIndex}";
        return false;
    }
    public string SendText()
    {
        return text;
    }
}
