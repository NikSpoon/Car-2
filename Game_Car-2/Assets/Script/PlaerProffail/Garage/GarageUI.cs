using UnityEngine;
using UnityEngine.SceneManagement;
public class GarageUI : MonoBehaviour
{
   public void SelectCar(int index )
    {
        PlayerDataManager.Instance.playerProfile.selectedCarIndex = index;
        Debug.Log("Выбрана машина: " + index);
    }

    public void SelectUpgraade(int index)
    {
        PlayerDataManager.Instance.playerProfile.selectedBodyUpgradeIndex = index;
    }
}
