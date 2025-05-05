using TMPro;
using UnityEngine;

public class ViewPhisick : MonoBehaviour
{
    private CarPhysic _carPhysic;
    private NoCollision _resp;
    [SerializeField] private TextMeshProUGUI _carSpeadText;
    [SerializeField] private TextMeshProUGUI _engineRPMText;
    [SerializeField] private TextMeshProUGUI _whellTorqueText;

    [SerializeField] private TextMeshProUGUI _timeNoCollision;
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
            return;
        }
        _resp = car.GetComponent<NoCollision>();
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

        _resp.OnNoCollision += OnNoCollision;
        if (_timeNoCollision != null)
        {
            _timeNoCollision.text = "NoCollision =  0 ";
            _timeNoCollision.gameObject.SetActive(false);
        }
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
    private void OnNoCollision(float time,bool IsGhostActive)
    {

        if (IsGhostActive)
        {
            _timeNoCollision.gameObject.SetActive(true);
           _timeNoCollision.text = "NoCollision =  " + time;
        }
        else
        {
            _timeNoCollision.gameObject.SetActive(false);
        }
    }
}
