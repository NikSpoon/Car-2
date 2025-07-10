using UnityEngine;

public class FinishControler : MonoBehaviour
{
    public void OnClicGagage()
    {

        PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToGerage);

    }
}
