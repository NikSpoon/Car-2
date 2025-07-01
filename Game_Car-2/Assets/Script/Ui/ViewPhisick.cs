using System.Collections;
using TMPro;
using UnityEngine;

public class ViewPhisick : MonoBehaviour
{
    private CarPhysic _carPhysic;
    private NoCollision _resp;
    private CarSpawner _start;
    [SerializeField] private TextMeshProUGUI _carSpeadText;
    [SerializeField] private TextMeshProUGUI _engineRPMText;
    [SerializeField] private TextMeshProUGUI _whellTorqueText;

    [SerializeField] private TextMeshProUGUI _timeNoCollision;
    [SerializeField] private TextMeshProUGUI _startTimer;
    private void Start()
    {
        StartCoroutine(MyAwake());
    }
    private IEnumerator MyAwake()
    {
        while (_resp == null || _start == null || _carPhysic == null)
        {
           // Debug.LogError("Не найден объект NoCollision - " + _resp + ". CarSpawner -  " + _start);

            GameObject car = GameObject.FindGameObjectWithTag("Player");
            GameObject StartObj = GameObject.FindGameObjectWithTag("Start");

            if (car != null)
            {
                _carPhysic = car.GetComponent<CarPhysic>();
                _resp = car.GetComponent<NoCollision>();
            }

            if (StartObj != null)
            {
                _start = StartObj.GetComponent<CarSpawner>();
            }

            yield return new WaitForSeconds(1);
        }

        // ✅ Подписывайся на события здесь, когда всё точно найдено!
        _carPhysic.OnSpeadChanged += OnSpead;
        _resp.OnNoCollision += OnNoCollision;
        _start.OnWaitForStart += OnWaitForStart;

        if (_carSpeadText != null) _carSpeadText.text = "Spead = 0";
        if (_engineRPMText != null) _engineRPMText.text = "EngineRPM = 0";
        if (_whellTorqueText != null) _whellTorqueText.text = "WhellTorque = 0";

        if (_timeNoCollision != null)
        {
            _timeNoCollision.text = "NoCollision = 0";
            _timeNoCollision.gameObject.SetActive(false);
        }
    }

   
    private void OnDestroy()
    {
        _carPhysic.OnSpeadChanged -= OnSpead;
        _resp.OnNoCollision -= OnNoCollision;
        _start.OnWaitForStart -= OnWaitForStart;
    }
    private void OnWaitForStart(int time, bool IsTimerActive)
    {
        if (IsTimerActive)
        {
            _startTimer.gameObject.SetActive(true);
            _startTimer.text = "Start after: " + time;

        }
        else
        {
            _startTimer.gameObject.SetActive(false);
        }
    }
    private void OnSpead(float spead, float currentEngineRPM, float WhellTorque)
    {
        _carSpeadText.text = "Spead =  " + spead;
        _engineRPMText.text = "EngineRPM =  " + currentEngineRPM;
        _whellTorqueText.text = "WhellTorque =  " + WhellTorque;
    }
    private void OnNoCollision(float time, bool IsGhostActive)
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
