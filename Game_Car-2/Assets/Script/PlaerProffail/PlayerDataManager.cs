using FSM.App;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance; 

    public PlayerProfile playerProfile = new PlayerProfile();
    public IAppSystem AppSystem = new AppSystem();

    private void Awake()
    {
        AppSystem.Trigger(AppTriger.ToMainMenu);
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
   
    public void AddMoney(int amount)
    {
        playerProfile.money += amount;
    }
    public bool SpendMoney(int amount)
    {
        if (playerProfile.money >= amount)
        {
            playerProfile.money -= amount;
            return true;
        }
        return false;
    }
}
