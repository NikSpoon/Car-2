using UnityEngine;
using TMPro;
using System.Collections;

public class BuyPopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _messageText;
    

    [Header("Links")]
    [SerializeField] private GarageShopManager _shopManager;
    [SerializeField] private GarageUISelectCar _uISelectCar;

    public bool IsYes = false;

    private void Start()
    {
        _messageText.text = _uISelectCar.SendText();
    }

    public void OnClickYes()
    {
        bool result;
         IsYes = true;
        if (_uISelectCar.CarOrBody)
        {
            _messageText.text = _shopManager.SendText();
            result = _shopManager.TryBuyCar();
            
        }
        else
        {
            _messageText.text = _shopManager.SendText();
            result = _shopManager.TryBuyUpgrade();
           
        }

        _messageText.text = _shopManager.SendText();

        if (result)
        {
            StartCoroutine(BuyCar());
        }
    }

    public void OnClickNo()
    {
        IsYes = false;
        Close();
    }

    private void Close()
    {
        IsYes = false;
        gameObject.SetActive(false);
    }

    private IEnumerator BuyCar()
    {
        yield return new WaitForSeconds(2);
        _uISelectCar.RefreshSelected();
        Close();

    }
}