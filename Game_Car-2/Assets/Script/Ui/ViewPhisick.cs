using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ViewPhisick : MonoBehaviour
{
    [SerializeField] private CarPhysic _carPhysic;
    [SerializeField] private TextMeshProUGUI _carSpeadText;
    [SerializeField] private TextMeshProUGUI _engineRPMText;
    [SerializeField] private TextMeshProUGUI _whellTorqueText;

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
