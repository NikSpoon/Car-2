using UnityEngine;

public class GarageUISelectCar : MonoBehaviour
{
   public void SelectCar(int index )
    {
        PlayerDataManager.Instance.PlayerProfile.selectedCarIndex = index;
       // Debug.Log("Выбрана машина: " + index);
    }

    public void SelectUpgraade(int index)
    {
        PlayerDataManager.Instance.PlayerProfile.selectedBodyUpgradeIndex = index;
       // Debug.Log("Выбрана машина: " + index);
    }
}
