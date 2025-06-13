
using System;
using System.Collections.Generic;

public class ExperienceManager
{
    private PlayerProfile playerProfile;
    private Dictionary<int, int> levelTable = new Dictionary<int, int>();

    public event Action<int> OnLevelUp;
    
    public int XpForLevlUp { get; private set; }
    public void Init(PlayerProfile profile)
    {
        this.playerProfile = profile;
        InitLevlTable();
        CheckLevelUp();
    }
    private void InitLevlTable()
    {
        
        levelTable.Clear();

        levelTable[0] =  0;
        levelTable[1] =  100;
        levelTable[2] =  500;
        levelTable[3] =  1000;
        levelTable[4] =  2000;
        levelTable[5] =  3500;
        levelTable[6] =  5000;
        levelTable[7] =  10000;
        levelTable[8] =  20000;
        levelTable[9] =  50000;
        levelTable[10] = 100000;

       
        // Добавляй уровни дальше по желанию
    }
    public void AddXP(int amount)
    {
        playerProfile.Xp += amount;

        CheckLevelUp();
    }

    public void AddLevel()
    {
        playerProfile.levl += 1;
        OnLevelUp?.Invoke(playerProfile.levl);
    }
    public bool TrySend(int cost)
    {
        if (playerProfile.Xp >= cost)
        {
            SendXp(cost);
            return true;
        }

        return false;
    }
    private void SendXp(int xp)
    {
        playerProfile.Xp -= xp;
    }
    private void CheckLevelUp()
    {

        while (true)
        {
            if (!levelTable.TryGetValue(playerProfile.levl, out int requiredXp))
                break;

            if (playerProfile.Xp >= requiredXp)
            {
                playerProfile.Xp -= requiredXp;
                AddLevel();
            }
            else
            {
                break;
            }
        }
        XpForLevlUp = levelTable.TryGetValue(playerProfile.levl, out int nextXp) ? nextXp : 0;
    }
    public Dictionary<int, int> GetLevelTable()
    {
        return new Dictionary<int, int>(levelTable); 
    }
    public int GetXpForLevel(int level)
    {
        if (levelTable.TryGetValue(level, out int xp))
            return xp;

        return -1; 
    }
  
}

