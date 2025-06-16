using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
    private PlayerProfile profile;
    private ExperienceManager experienceManager;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _levl;
    [SerializeField] private TextMeshProUGUI _xp;
    [SerializeField] private TextMeshProUGUI _id;
    [SerializeField] private TextMeshProUGUI _money;
    [SerializeField] private Image _xpImage;

    private void Start()
    {
        profile = PlayerDataManager.Instance.PlayerProfile;
        experienceManager = PlayerDataManager.Instance.Experience;

        UpdatePlayerInfo();

    }

    public void Update()
    {
        _name.text = $"{profile.playerName}";
        _levl.text = $"Level - {profile.levl}";
        _id.text = $"ID: {profile.playerID}";
        _money.text = $"{profile.money}";

       
        // Обновляем текст и прогрессбар
        _xp.text = $"XP: {profile.Xp} / {experienceManager.XpForLevlUp}";

      //  _xpImage.fillAmount = Mathf.Clamp01( profile.Xp / experienceManager.XpForLevlUp);
       
    }
    public void UpdatePlayerInfo()
    {
    }

}


