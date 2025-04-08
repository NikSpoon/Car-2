
using System;
using System.Collections.Generic;

using UnityEngine;

public class CarPhysic : MonoBehaviour
{
    [Header("Main value")]
    [SerializeField] private Transform _car;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Wheel[] _wheels;
    [SerializeField] private List<Gear> _gears;
    [SerializeField] private Transform _centreOfMass;

    [Header("Type of car")]
    [SerializeField] private bool _AWD;
    [SerializeField] private bool _RWD;
    [SerializeField] private bool _FWD;

    [Header("Type of raes")]
    [SerializeField] private bool _smolCar;
    [SerializeField] private bool _normalCar;
    [SerializeField] private bool _bigCar;

    [Header("Other Values")]
    [SerializeField] private int _motorForce = 500000;
    [SerializeField] private int _maxSteerAngle = 50;
    [SerializeField] private float _brakeTorque = 100000;
    [SerializeField] private float _engineMaxPowerRPM = 150000;
    [SerializeField] private float _maxSpead = 200;

    private int _currentGearIndex = 0;

    private float verticalInput;
    private float horizontalInput;
    private float _currentMotorForce = 0f;

    [Header("Acceleration/Deceleration Settings")]
    [SerializeField] private float _accelerationRate = 500f; // Скорость ускорения
    [SerializeField] private float _decelerationRate = 300f; // Скорость замедления


    [Header("Event Values")]
    [SerializeField] private float _speed;
    [SerializeField] private float _currentEngineRPM; // Оборот двигателя — Engine RPM (RPM = Revolutions Per Minute)
    [SerializeField] private float _currentWhellTorque;
    [SerializeField] private float _currentSteeringAngle = 50f;

    [Header("Rmp Values")]
    [SerializeField] private float _minRPM;
    [SerializeField] private float _maxRPM;
    [SerializeField] private float _rpmUpSpeed = 2000f;
    [SerializeField] private float _rpmDownSpeed = 3000f;
    public event Action<float, float, float> OnSpeadChanged;

    [Header("Graph Curves")]
    [SerializeField] private AnimationCurve _brakeCurve;
    [SerializeField] private AnimationCurve _airResistanceCurve;
    [SerializeField] private AnimationCurve _inertiaCurve;
    [SerializeField] private AnimationCurve _steeringCurve;
    bool isGasPresed;
    private void Awake()
    {
        if (!_AWD && !_RWD && !_FWD)
            _AWD = true;

        if (!_smolCar && !_normalCar && !_bigCar)
            _normalCar = true;




        if (_brakeCurve == null)
            _brakeCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));

        if (_airResistanceCurve == null)
            _airResistanceCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        if (_inertiaCurve == null)
            _inertiaCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));

      
      // _rigidbody.centerOfMass = _centreOfMass.position;
    }
    private void Start()
    {
        _rigidbody.centerOfMass = _car.InverseTransformPoint(_centreOfMass.position);

        _currentEngineRPM = 0;
    }
    public void Move(float vertical, float horisontal, bool brake)
    {
        SteerWheels(horisontal);
        Engine(vertical);
        ApplyHandbrake(brake);

        OnSpeadChanged?.Invoke(_speed, _currentEngineRPM, _currentWhellTorque);
    }
    private void Engine(float input)
    {
        UpdateGear();

        verticalInput = input;


        _currentMotorForce = verticalInput * _motorForce;

        isGasPresed = Mathf.Abs(input) > 0.1f;
        _currentEngineRPM = CalculateEngineRPM(isGasPresed);

        Gear currentGear = _gears[_currentGearIndex];


        float torque = input * currentGear.gearRatio * _motorForce;
        _currentWhellTorque = torque;

        float brakeForce = ApplyIdleBraking();
       


        foreach (var wheel in _wheels)
        {
            if (_AWD || (_FWD && wheel.IsForward) || (_RWD && !wheel.IsForward))
            {
                wheel.ApplyMotorTorque(torque);
            }

            wheel.ApplyBrakeTorque(brakeForce);
        }

        SimulateResistance();

        _speed = _rigidbody.linearVelocity.magnitude * 3.6f; // Convert m/s to km/h


        if (_speed <= 1 && isGasPresed == false)
        {
            _rigidbody.linearVelocity = Vector3.zero;
        }




        Debug.Log($"currentGear {currentGear}");
        // Debug.Log($"Speed: {_speed}, RPM: {_currentEngineRPM}, Torque: {_currentWhellTorque}, Motor Force: {_currentMotorForce}");
    }
    private float CalculateEngineRPM(bool gas)
    {
        Gear currentGear = _gears[_currentGearIndex];
        float wheelRPM = (_speed / 3.6f) * currentGear.gearRatio * 60f / (2 * Mathf.PI * 0.3f);


        float smoothFactor = 5f; // настраивай от 2 до 10
        _currentEngineRPM = Mathf.Lerp(_currentEngineRPM, wheelRPM, Time.fixedDeltaTime * smoothFactor);

        if (gas)
        {
            _currentEngineRPM += _rpmUpSpeed * Time.fixedDeltaTime;
        }
        else
        {
            _currentEngineRPM -= _rpmDownSpeed * Time.fixedDeltaTime;

        }
        _currentEngineRPM = Mathf.Clamp(_currentEngineRPM, 0f, _engineMaxPowerRPM);
        return _currentEngineRPM;
    }
    private void UpdateGear()
    {
        if (_gears == null || _gears.Count == 0)
            return;

        _currentGearIndex = Mathf.Clamp(_currentGearIndex, 0, _gears.Count - 1);


        Gear curentGear = _gears[_currentGearIndex];
        if (_currentEngineRPM > curentGear.maxRPM && _currentGearIndex < _gears.Count - 1)
        {
            _currentGearIndex++;

        }
        else if (_currentEngineRPM < curentGear.minRPM && _currentGearIndex > 0)
        {
            _currentGearIndex--;

        }

    }
    private float ApplyIdleBraking()
    {

        float speedNormalized = Mathf.Clamp01(_speed / _maxSpead);
        return _brakeCurve.Evaluate(speedNormalized) * _brakeTorque;
    }

    [Header(" воздух")]
    [SerializeField] private float _dragCoefficient = 0.4257f;
    [SerializeField] private float _rollingResistance = 12.8f;

    private void SimulateResistance()
    {
        if (_rigidbody == null) return;

        float speedMS = _speed / 3.6f;


        float airDragFactor = _airResistanceCurve.Evaluate(Mathf.Clamp01(speedMS / _maxSpead));
        float airDrag = _dragCoefficient * speedMS * speedMS * airDragFactor;


        float rollingDragFactor = _inertiaCurve.Evaluate(Mathf.Clamp01(speedMS / _maxSpead));
        float rollingDrag = _rollingResistance * speedMS * rollingDragFactor;


        Vector3 resistance = -_rigidbody.linearVelocity.normalized * (airDrag + rollingDrag);
        _rigidbody.AddForce(resistance);
    }
    private void ApplySteering(float angle)
    {

        foreach (var wheel in _wheels)
        {
            if (wheel.IsForward)
            {
                wheel.ApplySteerAngle(angle);
            }
        }
    }

    private void SteerWheels(float input)
    {
        
        horizontalInput = input;


        float steeringAngle = horizontalInput * _steeringCurve.Evaluate(_speed);
        float slipAngle = Vector3.Angle(_car.forward, _rigidbody.linearVelocity.normalized - _car.forward);

        if (slipAngle < 120 && _rigidbody.linearVelocity.magnitude > 1f)
            steeringAngle += Vector3.SignedAngle(_car.forward, _rigidbody.linearVelocity.normalized, Vector3.up);

        steeringAngle = Mathf.Clamp(steeringAngle, -_maxSteerAngle, _maxSteerAngle);


        foreach (Wheel wheel in _wheels)
        {
            ApplySteering(steeringAngle);
           
        }
    }


    [Header(" Braek ")]
    [SerializeField] private float _handbrakeTorque = 500000f;
    [SerializeField] private float _handbrakeRampSpeed = 2f;

    private float _currentHandbrakeTorque = 0f;
    private void ApplyHandbrake(bool brake)
    {
        
        float targetTorque = brake ? _handbrakeTorque : 0f;
        _currentHandbrakeTorque = Mathf.MoveTowards( _currentHandbrakeTorque,targetTorque,_handbrakeRampSpeed * Time.fixedDeltaTime * _handbrakeTorque );

        foreach (var wheel in _wheels)
        {
            if (!wheel.IsForward) // Применяем только к задним колёсам
            {
                wheel.ApplyBrakeTorque(_currentHandbrakeTorque);
                Debug.Log(brake);
            }
        }
    }
}