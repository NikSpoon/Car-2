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
  
    public void Sesect()
    {
        if (PlayerDataManager.Instance.playerProfile.Xp < XP)
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(raceData.SceneName);
    }
}
