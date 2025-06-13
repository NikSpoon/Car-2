using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI Xp;
    [SerializeField] private int XP;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI Stats;
    [SerializeField] private GameObject Error;

    private RaceData raceData;

    [SerializeField] private GameSessionData sessionData;
    public void Sesect()
    {
        if (PlayerDataManager.Instance.PlayerProfile.Xp < XP)
        {
            if (!Error.activeSelf)
            {
                Error.SetActive(true);
            }

        }
        else
            OnSelectRace();
    }
    public void Setup(RaceData data)
    {
        raceData = data;
        nameText.text = data.RaceName;
       
        previewImage.sprite = data.PreviewImage;
    }

    private void OnSelectRace()
    {
        if (sessionData != null && raceData != null)
        {
            sessionData.SetRaceMap(raceData);
            Debug.Log($"Выбрана гонка: {raceData.RaceName}");
        }
    }
}
