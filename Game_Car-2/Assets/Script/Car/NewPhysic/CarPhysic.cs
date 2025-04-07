using Unity.VisualScripting;
using UnityEngine;

public class CarPhysic : MonoBehaviour
{
    [Header("Main value")]
    [SerializeField] private Transform _car;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Wheel[] _wheels;

    [Header("Type of car")]
    [SerializeField] private bool _AWD;
    [SerializeField] private bool _RWD;
    [SerializeField] private bool _FWD;

    [Header("Type of raes")]
    [SerializeField] private bool _smolCar;
    [SerializeField] private bool _normalCar;
    [SerializeField] private bool _bigCar;

    [Header("Other Values")]
    [SerializeField] private int _motorForce;
    [SerializeField] private int _maxSteerAngle = 50;
    [SerializeField] private int _brakeTorque = 100000;

    private float verticalInput;
    private float horizontalInput;
    


    [SerializeField] private float _engineMaxPowerRPM;

    [Header("Event Values")]
    [SerializeField] private float _speed;
    [SerializeField] private float _currentEngineRPM; // Оборот двигателя — Engine RPM (RPM = Revolutions Per Minute)
    [SerializeField] private float _currentWhellTorque;

    private void Awake()
    {
        if (!_AWD && !_RWD && !_FWD)
            _RWD = true;
        
        if (!_smolCar && !_normalCar && !_bigCar)
            _normalCar = true;

    }
    private void Start()
    {
        
    }
    private void Engine()
    {

    }
}
