using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        NetworkPlayerProfile playerProfile = conn.identity.GetComponent<NetworkPlayerProfile>();

        // Здесь можно инициализировать профиль по умолчанию или по данным из базы
        PlayerProfile profile = new PlayerProfile();
        profile.GetNewProfile(profile.playerName + conn.connectionId, "defaultpass");

        playerProfile.Initialize(profile);
    }
}