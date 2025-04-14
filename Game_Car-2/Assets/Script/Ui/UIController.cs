using Gameplay.App;
using UnityEngine;

public class UIController : MonoBehaviour
{

    [SerializeField] private Transform _root;
    [SerializeField] private GameObject _mainMenuScren;
    [SerializeField] private GameObject _garageScren;
    [SerializeField] private GameObject _gameplayScren;
    [SerializeField] private GameObject _finishScren;

    private GameObject _currentScren;
    private void Start()
    {
        var appSystem = PlayerDataManager.Instance.AppSystem;
        appSystem.OnStateChange += OnStateChange;
        DontDestroyOnLoad(gameObject);
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
                _currentScren = Instantiate(_mainMenuScren, _root);
                break;
            case AppState.Garage:
                _currentScren = Instantiate(_garageScren, _root);
                break;
            case AppState.Gameplay:
                _currentScren = Instantiate(_gameplayScren, _root);
                break;
            case AppState.Finish:
                _currentScren = Instantiate(_finishScren, _root);
                break;

        }
    }
}
