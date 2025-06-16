
[System.Serializable]
public class PlayerProfile
{

    public string playerName;
    public string password;
    public int playerID;
    public int levl;
    public bool isOnline;


    public int selectedCarIndex = 0; // номер выбранной машины (например, 0 или 1)
    public int selectedBodyUpgradeIndex = 0; // номер апгрейда кузова (0-3)
    public int money = 0; // стартовое количество денег
    public int Xp = 0;


    public void GetNewProfile(string name, string pass)
    {
        playerName = name;
        password = pass;
        playerID = 00000000;

        selectedCarIndex = 0;
        selectedBodyUpgradeIndex = 0;
        money = 500;
        Xp = 100;
        levl = 1;
    }
}

