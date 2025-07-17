using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;

public class ViewPhysics : MonoBehaviour
{
    private CarSpawner _start;
    
    [SerializeField] private CarPhysic _carPhysic;
    [SerializeField] private NoCollision _resp;
    [SerializeField] private NetworkIdentity _networkIdentity;
    
    [SerializeField] private TextMeshProUGUI _carSpeadText;
    [SerializeField] private TextMeshProUGUI _engineRPMText;
    [SerializeField] private TextMeshProUGUI _whellTorqueText;

    [SerializeField] private TextMeshProUGUI _timeNoCollision;
    [SerializeField] private TextMeshProUGUI _startTimer;

    private void Start()
    {
            if (!_networkIdentity.isOwned) return;
        
        
        StartCoroutine(MyAwake());
    }
    private IEnumerator MyAwake()
    {

        while (_resp == null || _start == null || _carPhysic == null)
        {
            GameObject StartObj = GameObject.FindGameObjectWithTag("Start");

            if (StartObj != null)
            {
                _start = StartObj.GetComponent<CarSpawner>();
            }
            yield return _start;
        }

        // ✅ Подписывайся на события здесь, когда всё точно найдено!

        _carPhysic.OnSpeadChanged += OnSpead;
        _resp.OnNoCollision += OnNoCollision;
        _start.OnWaitForStart += OnWaitForStart;


        _carSpeadText.text = "Speed = 0";
        _engineRPMText.text = "EngineRPM = 0";
        _whellTorqueText.text = "WheelTorque = 0";

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
       
        if (_start != null) 
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
