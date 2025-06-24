using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using System.Collections;

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
        if (sessionsCountText == null)
        {
            Debug.LogError("sessionsCountText не назначен в инспекторе UISessionPanel!");
            return;
        }

        if (oneSessionPrefab == null)
        {
            Debug.LogError("oneSessionPrefab не назначен в инспекторе UISessionPanel!");
            return;
        }

        if (context == null)
        {
            Debug.LogError("context (Transform-контейнер) не назначен в инспекторе UISessionPanel!");
            return;
        }

        sessionsCountText.text = $"Сессий найдено: {sessions.Count}";

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

        // Debug.Log("📥 Привязываем сессию к UI: " + session.sessionId);

        // Показываем только одну сессию — текущую
        StartCoroutine(DelayedShowSessions(new List<NetworkGameSession> { session }));
    }
    private IEnumerator DelayedShowSessions(List<NetworkGameSession> sessions)
    {
        // Ждём, пока ClosePanel.Instance и openPanels инициализируются
        while (ClosePanel.Instance == null || ClosePanel.Instance.openPanels == null)
        {
            yield return null; // ждём следующий кадр
        }

        ShowSessions(sessions);
    }
}
