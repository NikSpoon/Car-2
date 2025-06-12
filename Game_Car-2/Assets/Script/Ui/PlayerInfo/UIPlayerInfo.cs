using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
    private PlayerProfile profile;

    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _levl;
    [SerializeField] private TextMeshProUGUI _xp;
    [SerializeField] private TextMeshProUGUI _id;
    [SerializeField] private TextMeshProUGUI _money;
    [SerializeField] private Image _xpImage;

    private void Start()
    {
        profile = PlayerDataManager.Instance.playerProfile;
        UpdatePlayerInfo();
    }

    public void Update()
    {
        _name.text = $"{profile.playerName}";
        _levl.text = $"Levl - {profile.levl}";
        _xp.text = $"XP - {profile.Xp}";
        _xpImage.fillAmount = profile.Xp;
        _id.text = $"ID:{profile.playerID}";
        _money.text = $"{profile.money}";

    }
    public void UpdatePlayerInfo()
    {
    }
}


