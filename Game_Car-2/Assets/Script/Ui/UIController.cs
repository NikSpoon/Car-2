using FSM.App;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour
{
    [SerializeField] private AppTriger _appTriger;
    [SerializeField] private GameSessionData _data;

    [SerializeField] private Transform _root;
    [SerializeField] private GameObject _mainMenuScren;
    [SerializeField] private GameObject _garageScren;
    [SerializeField] private GameObject _gameplayScren;
    [SerializeField] private GameObject _finishScren;

    [SerializeField] private GameObject _loadingScreen;

    private GameObject _currentScren;
    private void Awake()
    {
        var appSystem = PlayerDataManager.Instance.AppSystem;
        PlayerDataManager.Instance.AppSystem.OnStateChange += OnStateChange;
        DontDestroyOnLoad(gameObject);
      
        if (_data == null) 
        {
            Debug.LogError("GameSessionData не назначен в UIController!");
        }
    }
   

    [ContextMenu("SetTrigger")]
    public void SetTrigger()
    {
        PlayerDataManager.Instance.AppSystem.Trigger(_appTriger);
    }
    private void OnStateChange(StateChangeData<AppState, AppTriger> data)
    {
       
        if (_currentScren != null)
        {
            Destroy(_currentScren);
        }
     
        switch (data.NewState)
        {
            case AppState.MainMenu:
              
                SceneManager.LoadScene(1);
                // включаю катинку 
                // запускаю корутину
                // віключаю корутину и картинку как закончил закгужать 
                _currentScren = Instantiate(_mainMenuScren, _root);
                break;

            case AppState.Garage:
                SceneManager.LoadScene(2);
                // включаю катинку 
                // запускаю корутину
                // віключаю корутину и картинку как закончил закгужать 
                _currentScren = Instantiate(_garageScren, _root);
                break;

            case AppState.Gameplay:
                SceneManager.LoadScene(_data.raceSceneName);
                // включаю катинку 
                // запускаю корутину
                // віключаю корутину и картинку как закончил закгужать 
                _currentScren = Instantiate(_gameplayScren, _root);
                break;

            case AppState.Finish:
                SceneManager.LoadScene(4);
                // включаю катинку 
                // запускаю корутину
                // віключаю корутину и картинку как закончил закгужать 
                _currentScren = Instantiate(_finishScren, _root);
                break;

        }
    }
  
    private IEnumerator WaitForPlayerDataManager()
    {
        while (PlayerDataManager.Instance == null)
        {
            yield return null;
        }

        var appSystem = PlayerDataManager.Instance.AppSystem;
        appSystem.OnStateChange += OnStateChange;
        DontDestroyOnLoad(gameObject);
    }

}