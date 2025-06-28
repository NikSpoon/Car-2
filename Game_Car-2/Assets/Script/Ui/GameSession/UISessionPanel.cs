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
            if (ulong.TryParse(id, out ulong parsed))
            {
                CSteamID lobbySteamID = new CSteamID(parsed);
                JoinById(lobbySteamID);
            }
            else
            {
                Debug.LogError("❌ Неверный формат Lobby ID!");
            }
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
        steamLobbyManager.JoinLobbyById(lobbyId.m_SteamID.ToString());
        yield return null;
    }
    public void OnClickExit()
    {
        panelSessionRoot.SetActive(false);
       

    }
 
    private void OnLobbyCreated(CSteamID cSteamID)
    {
        if (steamLobbyManager.Lobbies.TryGetValue(cSteamID, out NetworkGameSession session))
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
        if (steamLobbyManager.Lobbies.TryGetValue(cSteamID, out NetworkGameSession session))
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
            sessionsCountText.text = $"Libbies = {steamLobbyManager.Lobbies.Count}";
            updateTimer = 0f;

            foreach (var session in steamLobbyManager.Lobbies.Values)
            {
                lobbyUIManager.UpdateLobbyUI(session);
            }
        }
    }
}
