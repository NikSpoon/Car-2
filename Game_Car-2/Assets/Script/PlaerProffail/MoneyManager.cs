
public class MoneyManager
{
  
    private PlayerProfile playerProfile;

    public void Init(PlayerProfile profile)
    {
        this.playerProfile = profile;
    }

    public void AddMoney(int money)
    {
        playerProfile.money += money;
    }

    public bool TrySend(int cost)
    {
        if(playerProfile.money >= cost)
        {
          SendMoney(cost);
            return true;
        }

        return false;
    }
    private void SendMoney(int money)
    {
        playerProfile.money -= money;
    }
   
}
