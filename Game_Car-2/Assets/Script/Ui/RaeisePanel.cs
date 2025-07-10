using UnityEngine;

public class RaeisePanel : MonoBehaviour
{
    public void OnClickFinish()
    {

        PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToFinish);

    }
  
    public void OnClickExit()
    {

    }
}
