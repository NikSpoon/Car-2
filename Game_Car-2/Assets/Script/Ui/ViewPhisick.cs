using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewPhisick : MonoBehaviour
{
    [SerializeField] private CarMovement _carMovement;
    [SerializeField] private TextMeshProUGUI _carMovementText;


    private void Start()
    {
        _carMovement.OnSpeadChanged += OnSpead;
        if (_carMovementText != null)
            _carMovementText.text = "Spead =  0 " ;
    }
    private void OnDestroy()
    {
        _carMovement.OnSpeadChanged -= OnSpead;

    }

    private void OnSpead(float spead)
    {
        _carMovementText.text = "Spead =  " + spead;
    }

}
