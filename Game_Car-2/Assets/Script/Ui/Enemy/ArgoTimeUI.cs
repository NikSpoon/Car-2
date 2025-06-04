using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArgoTimeUI : MonoBehaviour
{
    [SerializeField] private BaiseCar _car;

    [SerializeField] private Image _imageAgroTime;
    [SerializeField] private TextMeshProUGUI _textAgroTime;

    [SerializeField] private Image _imageAgroCooldown;
    [SerializeField] private TextMeshProUGUI _textAgroCooldown;

    private int _totalAgroTime;
    private int _totalCooldownTime;

    private void Start()
    {
        if (_car == null)
        {
            Debug.LogError("BaiseCar reference is not assigned in ArgoTimeUI!");
            enabled = false;
            return;
        }

        _totalAgroTime = _car.AgroTime;
        _totalCooldownTime = _car.AgroCooldownTime;

        _car.OnCooldownAgro += UpdateUI;
    }

    private void OnDestroy()
    {
        if (_car != null)
        {
            _car.OnCooldownAgro -= UpdateUI;
        }
    }

    private void UpdateUI(int agroTimeLeft, int cooldownLeft)
    {
        

        float agroFill = Mathf.Clamp01((_totalAgroTime - agroTimeLeft) / (float)_totalAgroTime);
        float cooldownFill = Mathf.Clamp01((_totalCooldownTime - cooldownLeft) / (float)_totalCooldownTime);

        _imageAgroTime.fillAmount = agroFill;
       

        _imageAgroCooldown.fillAmount = cooldownFill;
        
    }
}
