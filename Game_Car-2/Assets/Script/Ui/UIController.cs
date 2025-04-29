using FSM.App;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour
{
    [SerializeField] private AppTriger _appTriger;

    [SerializeField] private Transform _root;
    [SerializeField] private GameObject _mainMenuScren;
    [SerializeField] private GameObject _garageScren;
    [SerializeField] private GameObject _gameplayScren;
    [SerializeField] private GameObject _finishScren;

    [SerializeField] private GameObject _loadingScreen;

    private GameObject _currentScren;
    private void Start()
    {
       
        StartCoroutine(WaitForPlayerDataManager());
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
                StartCoroutine(WaitForLoadScene("MeinMenu", _mainMenuScren));
                break;

            case AppState.Garage:
                StartCoroutine(WaitForLoadScene("Garage", _garageScren));
                break;

            case AppState.Gameplay:
                StartCoroutine(WaitForLoadScene("Test Car", _gameplayScren));
                break;

            case AppState.Finish:
                _currentScren = Instantiate(_finishScren, _root);
                _loadingScreen?.SetActive(false);
                break;

        }
    }
    private void ShowLoadingScreen()
    {
        if (_loadingScreen != null)
            _loadingScreen.SetActive(true);
    }

    private IEnumerator WaitForLoadScene(string sceneName, GameObject screenPrefab)
    {
        ShowLoadingScreen();

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);

        while (!loadOp.isDone)
            yield return null;

        _currentScren = Instantiate(screenPrefab, _root);
        _loadingScreen?.SetActive(false);
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

