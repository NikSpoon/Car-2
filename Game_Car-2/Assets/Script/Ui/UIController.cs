using FSM.App;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIController : MonoBehaviour
{


    [SerializeField] private Transform _root;
    [SerializeField] private GameObject _mainMenuScren;
    [SerializeField] private GameObject _garageScren;
    [SerializeField] private GameObject _gameplayScren;
    [SerializeField] private GameObject _finishScren;

    [SerializeField] private GameObject _loadingScreen;

    private GameObject _currentScren;
  
    private void Awake()
    {
        PlayerDataManager.Instance.AppSystem.OnStateChange += OnStateChange;
    }
    private void Start()
    {
        
        DontDestroyOnLoad(gameObject);
   
    }

   
    private void OnStateChange(StateChangeData<AppState, AppTriger> data)
    {
       
        if (_currentScren != null)
        {
            Destroy(_currentScren);
        }
     
                Debug.Log(data.NewState + "data.NewState");
        switch (data.NewState)
        {
            case AppState.MainMenu:
              
                SceneManager.LoadScene("MainMenu"); // Проверь, что название сцены правильное
                Debug.Log("MainMenu scene loading...");
                // запускаю корутину
                // віключаю корутину и картинку как закончил закгужать 
                _currentScren = Instantiate(_mainMenuScren, _root);
                break;

            case AppState.Garage:
                SceneManager.LoadScene("Garage");

                // запускаю корутину
                // віключаю корутину и картинку как закончил закгужать 
                _currentScren = Instantiate(_garageScren, _root);
                break;

            case AppState.Gameplay:
                SceneManager.LoadScene("Test Car");
                // включаю катинку 
                // запускаю корутину
                // віключаю корутину и картинку как закончил закгужать 
                _currentScren = Instantiate(_gameplayScren, _root);
                break;

            case AppState.Finish:
                // включаю катинку 
                // запускаю корутину
                // віключаю корутину и картинку как закончил закгужать 
                _currentScren = Instantiate(_finishScren, _root);
                break;

        }
    }
  

}