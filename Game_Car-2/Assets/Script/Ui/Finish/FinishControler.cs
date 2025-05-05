using FSM.App;
using UnityEngine;

public class FinishControler : MonoBehaviour
{
    [SerializeField] private GameObject _finish;
    public void OnClickPlay()
    {

         PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToGerage);
               
    }
}
