using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class HealthView : MonoBehaviour

{
    [SerializeField] private Health _health;

    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        _health.OnHealthChanged += OnDamaged;
        if (_text != null)
            _text.text = "Health = " + _health.CurrentHeath;
    }
    private void OnDestroy()
    {
        _health.OnHealthChanged -= OnDamaged;

    }
    private void SetFill(float fillAmount)
    {
        _image.fillAmount = fillAmount;
    }
    private void OnDamaged(int current, int max)
    {
        SetFill((float)current / (float)max);
        if (_text != null)
            _text.text = "Health = " + _health.CurrentHeath;
    }

}