using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class UISessionPanel : MonoBehaviour
{
    [Header("UI ссылки")]
    [SerializeField] private TextMeshProUGUI sessionsCountText;
    [SerializeField] private TMP_InputField sessionIdInput;
    [SerializeField] private Transform context;                // Контейнер для сессий
    [SerializeField] private GameObject oneSessionPrefab;      // Префаб для отображения одной сессии
    [SerializeField] private GameObject panelSessionRoot;             // Само окно (для скрытия/показа)

    [Header("Логика")]
    [SerializeField] private SteamLobbyManager steamLobbyManager;

    private List<GameObject> spawnedSessionItems = new List<GameObject>();

    private void Start()
    {
        
        steamLobbyManager.OnLobbyCreatedUI += OnLobbyCreated;
    }

  
    public void OnClickCreate()
    {
        steamLobbyManager.CreateLobby();

       panelSessionRoot.SetActive(true);
       
    }

    public void OnClickJoin()
    {
        if (!string.IsNullOrEmpty(sessionIdInput.text))
        {
            steamLobbyManager.JoinLobbyById(sessionIdInput.text);
        }
    }

    public void OnClickExit()
    {
        panelSessionRoot.SetActive(false);
    }

    
    private void OnLobbyCreated(string lobbyId)
    {
        // Debug.Log("Лобби успешно создано: " + lobbyId);
        
        // можно обновить UI, если нужно
    }

   
    public void ShowSessions(List<NetworkGameSession> sessions)
    {
        // Очищаем старые UI-элементы
        foreach (var item in spawnedSessionItems)
        {
            Destroy(item);
        }
        spawnedSessionItems.Clear();
      
        // Обновляем текст количества сессий
        sessionsCountText.text = $"Сессий найдено: {sessions.Count}";

        // Создаём элементы UI для каждой сессии
        foreach (var session in sessions)
        {
            var sessionUIObj = Instantiate(oneSessionPrefab, context);
            ClosePanel.Instance.openPanels.Add(sessionUIObj);
            var ui = sessionUIObj.GetComponent<UIGameSession>();
            ui.SetSession(session);
            spawnedSessionItems.Add(sessionUIObj);
        }
    }
    public void AttachToNetworkSession(NetworkGameSession session)
    {
        if (session == null)
        {
            Debug.LogWarning("AttachToNetworkSession получил null-сессию.");
            return;
        }

        Debug.Log("📥 Привязываем сессию к UI: " + session.sessionId);

        // Показываем только одну сессию — текущую
        ShowSessions(new List<NetworkGameSession> { session });
    }
}
