using FSM.App;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance; 

    public PlayerProfile playerProfile = new PlayerProfile();
    public IAppSystem AppSystem = new AppSystem();

    private void Awake()
    {
        
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

}
