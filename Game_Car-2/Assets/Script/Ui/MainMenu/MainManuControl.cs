using FSM.App;
using System.Collections;
using UnityEngine;

public class MainMenuControl : MonoBehaviour
{
    [SerializeField] private InputServis _input;
    
    
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _save;
    [SerializeField] private GameObject _multiplayer;
  


    private float _escapeHoldTime = 0f;
    private const float ESCAPE_HOLD_DURATION = 1.5f;

   

    private void Update()
    {
        if (_input == null) return;

        var panel = ActivePanel();

        if (_input.Exit)
        {
            _escapeHoldTime += Time.deltaTime;

            if (_escapeHoldTime >= ESCAPE_HOLD_DURATION)
            {
                CloseAllPanels();
                _escapeHoldTime = 0f;
                return;
            }
            else if (panel != null)
            {
                ExitPanel(panel);
            }
        }
        else
        {
            _escapeHoldTime = 0f;
        }
    }

    public void OnClickPlay()
    {
        PlayerDataManager.Instance.AppSystem.Trigger(AppTriger.ToGerage);
    }

    public void OnClickOptions()
    {
        if (ActivePanel() == null)
        {
            _options.SetActive(!_options.activeSelf);
        }
    }

    public void OnClickSave()
    {
        if (ActivePanel() == null)
        {
            _save.SetActive(!_save.activeSelf);
        }
    }

    public void OnClickMultiplayer()
    {
        if (ActivePanel() == null)
        {
            _multiplayer.SetActive(!_multiplayer.activeSelf);
        }
    }

    public void OnClickExit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ExitPanel(GameObject panel)
    {
        if (_input == null || panel == null) return;

        if (panel.activeSelf)
            panel.SetActive(false);
    }

    private GameObject ActivePanel()
    {
        if (_options.activeSelf) return _options;
        if (_save.activeSelf) return _save;
        if (_multiplayer.activeSelf) return _multiplayer;

        return null;
    }

    private void CloseAllPanels()
    {
        _options.SetActive(false);
        _save.SetActive(false);
        _multiplayer.SetActive(false);
    }
}
