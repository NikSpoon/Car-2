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

    private void Start()
    {
        _car.OnCooldownAgro += UpdateUI;
    }

    private void OnDestroy()
    {
        _car.OnCooldownAgro -= UpdateUI;
    }

    private void UpdateUI(int agroTime, int cooldownLeft)
    {
        // Заполненность прогресс-бара (от 0 до 1)
        float agroFill = Mathf.Clamp01(1f - (cooldownLeft / (float)agroTime));
        float cooldownFill = Mathf.Clamp01(cooldownLeft / (float)agroTime);

        _imageAgroTime.fillAmount = agroFill;
       // _textAgroTime.text = $"{agroTime - cooldownLeft}s";

        _imageAgroCooldown.fillAmount = cooldownFill;
       // _textAgroCooldown.text = $"{cooldownLeft}s";
    }

}
