using UnityEngine;

public class AIProfile
{
    public string botName;
 
    public int AI_ID;
   
    public int selectedCarIndex = 0; 
    public int selectedBodyUpgradeIndex = 0;
   
    
    public int money = 0; // стартовое количество денег
    public int Xp = 0;

    public void GetNewBotProfile(string name, int ID)
    {
        botName = name;

        AI_ID = ID;

        selectedCarIndex = 0;
        selectedBodyUpgradeIndex = 0;
       
    }

}
