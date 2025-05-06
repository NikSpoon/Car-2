using FSM.App;
using UnityEngine;


public class MainManuControl : MonoBehaviour
{
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _save;
    [SerializeField] private GameObject _multiplaer;
    [SerializeField] private GameObject _exit;

    public void OnClickPlay()
    {

        if (PlayerDataManager.Instance.AppSystem.CurrentState == AppState.MainMenu)
        {
            PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToGerage);
        }
        else if (PlayerDataManager.Instance.AppSystem.CurrentState == AppState.Garage)
        {
            PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToGameplay);
        }
    }



    public void OnClickOptions()
    {
        if (_save.activeSelf != true && _multiplaer.activeSelf != true)
        {
            if (_options.activeSelf)
                _options.SetActive(false);
            else
                _options.SetActive(true);
        }
    }

    public void OnClickSave()
    {
        if (_options.activeSelf != true && _multiplaer.activeSelf != true)
        {

            if (_save.activeSelf)
                _save.SetActive(false);
            else
                _save.SetActive(true);
        }
    }

    public void OnClickMultiplaer()
    {
        if (_save.activeSelf != true && _options.activeSelf != true)
        {
            if (_multiplaer.activeSelf)
                _multiplaer.SetActive(false);
            else
                _multiplaer.SetActive(true);
        }
    }

    public void OnClickExit()
    {
        if (PlayerDataManager.Instance.AppSystem.CurrentState == AppState.MainMenu)
        {
            Application.Quit(); // Работает в билде
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Только для редактора
#endif
            return;

        }
        else if (PlayerDataManager.Instance.AppSystem.CurrentState == AppState.Garage)
        {
            PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToMainMenu);
        }
    }

}

