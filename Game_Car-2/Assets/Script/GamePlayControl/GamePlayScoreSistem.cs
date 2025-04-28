using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayScoreSistem : MonoBehaviour
{

    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private int _totalCoins = 0;
    private int _maxCoins = 50;

    private void Start()
    {

        Coin.OnCoinUp += CoinUI; ;
        
    }
    private void OnDestroy()
    {
        Coin.OnCoinUp -= CoinUI;
       
    }
    private void CoinUI(int coinValue)
    {
        _totalCoins += coinValue;

        if (_image != null)
            _image.fillAmount = Mathf.Clamp01((float)_totalCoins / _maxCoins);

        if (_text != null)
            _text.text = _totalCoins.ToString();
    }
}