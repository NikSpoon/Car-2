using System.Collections.Generic;
using UnityEngine;

public class BotCreator : BotNameRegistry
{
    private HashSet<int> usedIds = new(); // Храним уже использованные ID

    public AIProfile CreateUniqueBot()
    {
        if (usedIds.Count >= idToName.Count)
        {
            Debug.Log("Все доступные имена уже использованы.");
            ResetUsedNames();
        }

        int randomId;
        do
        {
            randomId = Random.Range(0, idToName.Count);
        } while (usedIds.Contains(randomId));

        usedIds.Add(randomId);
        string name = idToName[randomId];

        var bot = new AIProfile();
        bot.GetNewBotProfile(name, randomId);

        Debug.Log($"Создан бот: {name} (ID: {randomId})");

        return bot;
    }

    public void ResetUsedNames()
    {
        usedIds.Clear();
        Debug.Log("Список использованных имен сброшен.");
    }
}
