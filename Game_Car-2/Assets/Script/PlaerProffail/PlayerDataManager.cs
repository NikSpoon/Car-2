using FSM.App;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    [SerializeField] private CarDatabase _database;

    public static PlayerDataManager Instance; 

    public PlayerProfile PlayerProfile = new PlayerProfile();
    public IAppSystem AppSystem = new AppSystem();
    public MoneyManager Money = new MoneyManager();
    public ExperienceManager Experience = new ExperienceManager();
    public PlayerSessionData PlayerSessionData = new PlayerSessionData();
    public CarInventoryManager CarInventory = new CarInventoryManager();

    public LevelTableSO levelTableSO;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Money.Init(PlayerProfile);
            Experience.Init(PlayerProfile);
            CarInventory.Init(PlayerProfile, _database);
        }
        else
        {
            Destroy(gameObject); 
        }
         
    }

}
