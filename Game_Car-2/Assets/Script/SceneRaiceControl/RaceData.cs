using UnityEngine;

[CreateAssetMenu(fileName = "NewRaiseData", menuName = "Race/RaiseData")]
public class RaceData : ScriptableObject
{
    [SerializeField] private string sceneName;
    [SerializeField] private string raceName;
    [SerializeField] private string location;
    [SerializeField] private int maxCar;
    [SerializeField] private Sprite previewImage; // ?? ???????? ?????

    public string SceneName => sceneName;
    public string RaceName => raceName;
    public string Location => location;
    public int MaxCar => maxCar;
    public Sprite PreviewImage => previewImage;
}