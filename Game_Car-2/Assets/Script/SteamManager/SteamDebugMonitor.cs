using Mirror;
using Steamworks;
using UnityEngine;


public class SteamDebugMonitor : MonoBehaviour
{
    public static SteamDebugMonitor Instance;
    public NetworkManager networkManager;

    private string mySteamId = "None";
    private string currentLobbyId = "None";
    private string connectionStatus = "Idle";
    private string connectionAddress = "None";
    private string hostSteamId = "None";
    private string transportInfo = "None";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("[SteamDebugMonitor] Initialized and will persist between scenes.");
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Debug.Log($"[SteamDebugMonitor] Steam initialized: {SteamManager.Initialized}");
        if (SteamManager.Initialized)
        {
            mySteamId = SteamUser.GetSteamID().ToString();
            Debug.Log($"[SteamDebugMonitor] My SteamID: {mySteamId}");
        }
    }

    void Update()
    {
        SteamAPI.RunCallbacks(); // Чтобы Steam callbacks всегда работали

        if (NetworkClient.active)
        {
            connectionStatus = "Client Connecting/Connected";
        }
        else if (NetworkServer.active)
        {
            connectionStatus = "Server Hosting";
        }
        else
        {
            connectionStatus = "Idle";
        }

        if (NetworkManager.singleton != null && NetworkManager.singleton.transport != null)
        {
            transportInfo = NetworkManager.singleton.transport.GetType().Name;
        }
        else
        {
            transportInfo = "None";
        }
    }

    public void SetLobbyId(CSteamID lobbyId)
    {
        currentLobbyId = lobbyId.ToString();
        Debug.Log($"[SteamDebugMonitor] Joined Lobby: {currentLobbyId}");
    }

    public void SetConnectionAddress(string address)
    {
        connectionAddress = address;
        Debug.Log($"[SteamDebugMonitor] Trying to connect to: {connectionAddress}");
    }

    public void SetHostSteamId(CSteamID steamId)
    {
        hostSteamId = steamId.ToString();
        Debug.Log($"[SteamDebugMonitor] Host SteamID set to: {hostSteamId}");
    }

    void OnGUI()
    {
        GUI.color = Color.black; // Чёрный цвет текста

        GUI.Label(new Rect(10, 10, 700, 25), $"[SteamDebugMonitor] My SteamID: {mySteamId}");
        GUI.Label(new Rect(10, 35, 700, 25), $"Status: {connectionStatus}");
        GUI.Label(new Rect(10, 60, 700, 25), $"Lobby ID: {currentLobbyId}");
        GUI.Label(new Rect(10, 85, 700, 25), $"Host SteamID: {hostSteamId}");
        GUI.Label(new Rect(10, 110, 700, 25), $"Host Address from Lobby: {connectionAddress}");
        GUI.Label(new Rect(10, 135, 700, 25), $"Transport: {transportInfo}");

        string matchStatus = (connectionAddress == hostSteamId && hostSteamId != "None") ? "✅ MATCH OK" : "❌ MISMATCH!";
        GUI.Label(new Rect(10, 160, 700, 25), $"Check: HostAddress == HostSteamID ? {matchStatus}");

        if (NetworkClient.active && networkManager != null && networkManager.transport != null)
        {
            GUI.Label(new Rect(10, 185, 700, 25), $"Transport Address: {networkManager.networkAddress}");
        }
    }
}