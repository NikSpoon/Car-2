using FSM.App;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Mirror;

public class UIController : MonoBehaviour
{

    [SerializeField] private GameSessionData _data;

    [SerializeField] private Transform _root;
    [SerializeField] private GameObject _firstLoadingScren;
    [SerializeField] private GameObject _mainMenuScren;
    [SerializeField] private GameObject _garageScren;
    [SerializeField] private GameObject _gameplayScren;
    [SerializeField] private GameObject _finishScren;

    [SerializeField] private GameObject _loadingScreen;

    private GameObject _currentScren;


    private void Awake()
    {
        
        if (FindObjectsByType<UIController>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        var appSystem = PlayerDataManager.Instance.AppSystem;
        PlayerDataManager.Instance.AppSystem.OnStateChange += OnStateChange;

       
        if (_data == null)
        {

            Debug.LogError("GameSessionData не назначен в UIController!");
        }
        
        _currentScren = Instantiate(_firstLoadingScren, _root);
    }

    private void OnStateChange(StateChangeData<AppState, AppTriger> data)
    {

        DontDestroyOnLoad(gameObject);
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