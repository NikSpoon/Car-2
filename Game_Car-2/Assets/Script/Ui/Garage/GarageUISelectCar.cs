using UnityEngine;

public class GarageUISelectCar : MonoBehaviour
{
    [SerializeField] private GarageMenuControl _garageMenuControl;
    private CarInventoryManager inventoryManager;
    public int carIndex { get; private set; }
    public int bodyIndex { get; private set; }
    public int cost { get; private set; }

    private string text;

    public bool CarOrBody { get; private set; }
    private void Start()
    {
        inventoryManager = PlayerDataManager.Instance.CarInventory;
    }
    public void SelectCar(int carindex)
    {
        CarOrBody = true;
        cost =  inventoryManager.TrySelectCarAndUpgrade(carindex, 0);
        SelectUpgrade(0);
        carIndex = carindex;
        if (cost > 0)
        {
            text = $"Машина не куплена! Стоимость: {cost}";
            _garageMenuControl.OnClickBuy();
            
        }
    }

    public void SelectUpgrade(int index)
    {
        CarOrBody = false;
        cost = inventoryManager.TrySelectCarAndUpgrade(carIndex, index);
        bodyIndex = index;
        if (cost > 0)
        {
            text = ($"Апгрейд не куплен! Стоимость: {cost}");
            _garageMenuControl.OnClickBuy();
            
        }
    }
    public string SendText()
    {
        return text;
    }
    public void RefreshSelected()
    {
        cost = inventoryManager.TrySelectCarAndUpgrade(carIndex, bodyIndex);
    }
}