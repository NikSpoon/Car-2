using TMPro;
using UnityEngine;

public class ViewPhisick : MonoBehaviour
{
    [SerializeField] private CarPhysic _carPhysic;
    [SerializeField] private TextMeshProUGUI _carSpeadText;
    [SerializeField] private TextMeshProUGUI _engineRPMText;
    [SerializeField] private TextMeshProUGUI _whellTorqueText;
    private void Awake()
    {
        GameObject car = GameObject.FindGameObjectWithTag("Player");

        if (car == null)
        {
            Debug.LogError("Не найден объект с тегом Player!");
            return;
        }

        _carPhysic = car.GetComponent<CarPhysic>();

        if (_carPhysic == null)
        {
            Debug.LogError("На объекте Player отсутствует компонент CarPhysic!");
        }
    }

    private void Start()
    {
        _carPhysic.OnSpeadChanged += OnSpead;
        if (_carSpeadText != null)
            _carSpeadText.text = "Spead =  0 ";
        if (_engineRPMText != null)
            _engineRPMText.text = "EngineRPM =  0 ";
        if (_whellTorqueText != null)
            _whellTorqueText.text = "WhellTorque =  0 ";
    }
    private void OnDestroy()
    {
        _carPhysic.OnSpeadChanged -= OnSpead;

    }

    private void OnSpead(float spead, float currentEngineRPM, float WhellTorque)
    {
        _carSpeadText.text = "Spead =  " + spead;
        _engineRPMText.text = "EngineRPM =  " + currentEngineRPM;
        _whellTorqueText.text = "WhellTorque =  " + WhellTorque;
    }

}
