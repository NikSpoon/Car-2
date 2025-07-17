using Mirror;
using Steamworks;
using TMPro;

using UnityEngine;

public class UIGameSessionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sessionNameText;
    [SerializeField] private TextMeshProUGUI sessionId;
    [SerializeField] private TextMeshProUGUI sessionMapName;
    [SerializeField] private TextMeshProUGUI sessionPlayersValue;
    [SerializeField] private TextMeshProUGUI sessionMaxPlayersValue;

    private CSteamID lobbyIdUI;
    private UISessionPanel rootPanel;

    public void Init(UISessionPanel panel)
    {
        rootPanel = panel;
    }
    public void SetSessionData(string name, string id, string map, int players, int maxPlayers, CSteamID lobbyId)
    {
        sessionNameText.text = name;
        sessionId.text = id;
        sessionMapName.text = map;
        sessionPlayersValue.text = players.ToString();
        sessionMaxPlayersValue.text = maxPlayers.ToString();

        this.lobbyIdUI = lobbyId;
    }

    public void OnJoinClicked()
    {
        if (rootPanel != null)
        {
            rootPanel.JoinById(lobbyIdUI);
        }
        else
        {
            Debug.LogError("❌ rootPanel не установлен!");
        }
       
    }
  

}