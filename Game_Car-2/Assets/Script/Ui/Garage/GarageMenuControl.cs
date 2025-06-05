using FSM.App;
using System.Collections;
using UnityEngine;

public class GarageMenuControl : MonoBehaviour
{
    [SerializeField] private InputServis _input;
    [SerializeField] private RaiceStarter _raiceStarter;


    [SerializeField] private GameObject _creator;
    [SerializeField] private GameObject _bust;
    [SerializeField] private GameObject _options;



    [SerializeField] private GameObject _creatorNull;
    [SerializeField] private GameObject _buy;
    [SerializeField] private GameObject _cenNotBuy;

    private float _escapeHoldTime = 0f;
    private const float ESCAPE_HOLD_DURATION = 1.5f; // секунда(ы) удержания для полного закрытия



    private void Start()
    {
        _creator.SetActive(true);
        _creator.SetActive(false);
    }
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
    public void OnClickStart()
    {

        if (_raiceStarter != null && _raiceStarter.IsRaiceStarterFul)
            PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToGameplay);
        else
            _creatorNull.SetActive(true);
    }
    public void OnClickCeator()
    {
        if (ActivePanel() == null)
            _creator.SetActive(true);
        else
            _creator.SetActive(false);

    }
    public void OnClickBust()
    {
        if (ActivePanel() == null)
            _bust.SetActive(true);
        else
            _bust.SetActive(false);
    }
    public void OnClickOptions()
    {
        if (ActivePanel() == null)
            _options.SetActive(true);
        else
            _options.SetActive(false);

    }
    public void OnClickExitToMeinMenu()
    {


        PlayerDataManager.Instance.AppSystem.Trigger(FSM.App.AppTriger.ToMainMenu);

    }
    public void OnClickBuy()
    {

        if (ActivePanel() == null)
            _buy.SetActive(true);
        else
        {
            _cenNotBuy.SetActive(false);
            _buy.SetActive(false);

        }
    }
    public void OnClickCenNotBuy()
    {
        if (ActivePanel() == _buy) // и не хватило денег)
            _cenNotBuy.SetActive(true);

        if (_cenNotBuy.activeSelf)
            _cenNotBuy.SetActive(false);
    }

    private void ExitPanel(GameObject panel)
    {

        if (_input == null) return;
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }

    }
    private GameObject ActivePanel()
    {

        if (_creatorNull.activeSelf) return _creatorNull;
        if (_creator.activeSelf) return _creator;
        if (_bust.activeSelf) return _bust;
        if (_options.activeSelf) return _options;
        if (_buy.activeSelf)
            return _cenNotBuy.activeSelf ? _cenNotBuy : _buy;

        return null;
    }
    private void CloseAllPanels()
    {
        _creatorNull.SetActive(false);
        _creator.SetActive(false);
        _bust.SetActive(false);
        _options.SetActive(false);
        _buy.SetActive(false);
        _cenNotBuy.SetActive(false);
    }
}

