using UnityEngine;
using TMPro;
using Steamworks;
using Mirror;
using Edgegap;
using System.Collections;


public class UISessionPanel : MonoBehaviour
{
    [Header("UI ссылки")]
    [SerializeField] private TextMeshProUGUI sessionsCountText;
    [SerializeField] private TMP_InputField sessionIdInput;
    [SerializeField] private GameObject panelSessionRoot;         
    [SerializeField] private SteamLobbyUIManager lobbyUIManager;


    [Header("Логика")]
    [SerializeField] private SteamLobbyManager steamLobbyManager;


    private void Start()
    {

        steamLobbyManager.OnLobbyCreatedUI += OnLobbyCreated;
        steamLobbyManager.OnLobbyEmpty += OnLobbyEmpty;
    }
    private void OnDestroy()
    {
        steamLobbyManager.OnLobbyCreatedUI -= OnLobbyCreated;
        steamLobbyManager.OnLobbyEmpty -= OnLobbyEmpty;
    }

    public void OnClickCreate()
    {
        steamLobbyManager.CreateLobby();

        panelSessionRoot.SetActive(true);

    }

    public void OnClickJoinById(string id)
    {
        if (id == null)
        {
            id = sessionIdInput.text;
        }

        if (!string.IsNullOrEmpty(id))
        {
            steamLobbyManager.JoinLobbyById(id);

        }
        else
        {
            return;
        }
    }
    public void JoinById(CSteamID id)
    {
        StartCoroutine(ConnectAndJoin(id));
        panelSessionRoot.SetActive(true);
    }
    private IEnumerator ConnectAndJoin(CSteamID lobbyId)
    {
        if (!NetworkClient.active)
        {
            NetworkManager.singleton.StartClient();
        }

        // Ждём подключения
        while (!NetworkClient.isConnected)
        {
            yield return null;
        }
        steamLobbyManager.JoinLobbyById(lobbyId.ToString()); //????????????????????????????????????????????????????????????
        var player = NetworkClient.connection.identity.GetComponent<NetworkPlayerProfile>();
        player.CmdJoinLobbyById(lobbyId); // или любой аналогичный метод
    }
    public void OnClickExit()
    {
        panelSessionRoot.SetActive(false);
       

    }
 
    private void OnLobbyCreated(CSteamID cSteamID)
    {
        if (steamLobbyManager.lobby.TryGetValue(cSteamID, out NetworkGameSession session))
        {
            lobbyUIManager.AddLobbyToUI(session);
        }
        else
        {
            Debug.LogWarning($"UI: Не удалось найти сессию для лобби {cSteamID}");
        }
    }
    private void OnLobbyEmpty(CSteamID cSteamID)
    {
        if (steamLobbyManager.lobby.TryGetValue(cSteamID, out NetworkGameSession session))
        {
            lobbyUIManager.RemoveLobby(cSteamID);
            //NetworkServer.Shutdown();
        }
        else
        {
            Debug.LogWarning($"UI: Не удалось найти сессию для лобби {cSteamID}");
        }
    }
    
    private float updateInterval = 2f;
    private float updateTimer;

    public void Update()
    {
        
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            sessionsCountText.text = $"Libbies = {steamLobbyManager.lobby.Count}";
            updateTimer = 0f;

            foreach (var session in steamLobbyManager.lobby.Values)
            {
                lobbyUIManager.UpdateLobbyUI(session);
            }
        }
    }
}
