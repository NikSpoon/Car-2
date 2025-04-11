using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance; // глобальный доступ

    public PlayerProfile playerProfile = new PlayerProfile();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // сохраняем при смене сцен
        }
        else
        {
            Destroy(gameObject); // если уже есть — удаляем дубликат
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
