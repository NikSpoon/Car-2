using UnityEngine;
using UnityEngine.UI;

public class RaceListUI : MonoBehaviour
{
    [SerializeField] private RaceDatabase raceDatabase;
    [SerializeField] private GameObject raceItemPrefab;
    [SerializeField] private Transform contentParent;
        
    private void Start()
    {
        foreach (var race in raceDatabase.Races)
        {
            var item = Instantiate(raceItemPrefab, contentParent);
            item.GetComponent<RaceItemUI>().Setup(race);
        }
    }
}