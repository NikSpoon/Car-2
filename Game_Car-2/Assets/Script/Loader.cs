using FSM.App;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private void Start()
    {
        PlayerDataManager.Instance.AppSystem.Trigger(AppTriger.ToMainMenu);
        
    }

}
