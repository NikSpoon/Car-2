using TMPro;
using UnityEngine;

public class SteamLobbyUICreator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyIdText;
    [SerializeField] private TMP_InputField sessionNameInput;
    [SerializeField] private TMP_InputField lobbyIdInput;

    [SerializeField] private SteamLobbyManager steamLobbyManager;

    private void Start()
    {
        steamLobbyManager.OnLobbyCreatedUI += UpdateLobbyIdUI;
    }

    public void OnClickCreate()
    {
        steamLobbyManager.CreateLobby();
    }

    public void OnClickJoin()
    {
        steamLobbyManager.JoinLobbyById(lobbyIdInput.text);
    }

    private void UpdateLobbyIdUI(string lobbyId)
    {
        lobbyIdText.text = $"Lobby ID: {lobbyId}";
    }
}
