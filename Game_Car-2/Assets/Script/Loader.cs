using FSM.App;
using UnityEngine;

public class Loader : MonoBehaviour
{
   
    private void Start()
    {
        var appSystem = PlayerDataManager.Instance.AppSystem;

           appSystem.Trigger(AppTriger.ToMainMenu);
        
    }

 
}
