using UnityEngine;

public class FinishRaeiseStasistic : MonoBehaviour
{
    [SerializeField] private GameObject _finish;
    public void OnClickPlay()
    {

        PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToFinish);

    }
}
