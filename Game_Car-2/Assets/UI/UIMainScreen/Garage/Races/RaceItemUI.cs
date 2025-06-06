using UnityEngine;
using UnityEngine.UI;

public class RaceItemUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text locationText;
    [SerializeField] private Image previewImage;
    private RaceData raceData;

    public void Setup(RaceData data)
    {
        raceData = data;
        nameText.text = data.RaceName;
        locationText.text = data.Location;
        previewImage.sprite = data.PreviewImage;
    }

    public void OnSelectRace()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(raceData.SceneName);
    }
}
