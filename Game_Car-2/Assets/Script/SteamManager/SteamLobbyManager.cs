using Mirror;
using Steamworks;
using UnityEngine;
using Mirror.FizzySteam;

public class SteamLobbyManager : MonoBehaviour
{
    public FizzySteamworks steamTransport;
    public NetworkManager networkManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private CSteamID currentLobbyID;

    void Start()
    {
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void CreateLobby()
    {
        int maxPlayers = networkManager.maxConnections;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxPlayers);
    }

    private void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult == EResult.k_EResultOK)
        {
            Debug.Log("Лобби создано: " + result.m_ulSteamIDLobby);
            currentLobbyID = new CSteamID(result.m_ulSteamIDLobby);

            steamTransport.ServerStart();
            // Можно использовать currentLobbyID.ToString() как адрес для подключения клиентов
        }
        else
        {
            Debug.LogError("Ошибка создания лобби: " + result.m_eResult);
        }
    }

    private void OnLobbyEntered(LobbyEnter_t result)
    {
        currentLobbyID = new CSteamID(result.m_ulSteamIDLobby);
        Debug.Log("Вошли в лобби: " + currentLobbyID);

        // Подключаемся как клиент по SteamID лобби
        steamTransport.ClientConnect(currentLobbyID.ToString());
    }
}
