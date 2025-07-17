using FSM.App;
using UnityEngine;

public class Loader : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 100; // или 120 — сколько нужно
        QualitySettings.vSyncCount = 1;   // 1 = VSync включён
    }
    private void Start()
    {
        var Instance = PlayerDataManager.Instance.AppSystem;
      
    }
    public void Loading()
    {
        PlayerDataManager.Instance.AppSystem.Trigger(AppTriger.ToMainMenu);
    }

}
