using FSM.App;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private void Start()
    {
        var Instance = PlayerDataManager.Instance.AppSystem;
        PlayerDataManager.Instance.AppSystem.Trigger(AppTriger.ToMainMenu);
        
    }

}
