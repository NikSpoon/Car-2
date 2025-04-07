using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarPhysic : MonoBehaviour
{
    [Header("Main value")]
    [SerializeField] private Transform _car;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Wheel[] _wheels;
    [SerializeField] private List<Gear> _gears;


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
    [SerializeField] private int _brakeTorque = 100000;
    [SerializeField] private float _engineMaxPowerRPM = 150000;

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

    [Header("Rmp Values")]
    [SerializeField] private float _minRPM;
    [SerializeField] private float _maxRPM;
    [SerializeField] private float _rpmUpSpeed = 2000f;
    [SerializeField] private float _rpmDownSpeed = 3000f;
    public event Action<float,float,float> OnSpeadChanged;
    private void Awake()
    {
        if (!_AWD && !_RWD && !_FWD)
            _RWD = true;

        if (!_smolCar && !_normalCar && !_bigCar)
            _normalCar = true;

    }
    private void Start()
    {
        _currentEngineRPM = 0;
    }
    public void Move(float vertical)
    {
        Engine(vertical);
        OnSpeadChanged?.Invoke(_speed, _currentEngineRPM, _currentWhellTorque);
    }
    private void Engine(float input)
    {
        UpdateGear();

        input = CalculateEngineRPM(input);


        _currentMotorForce = input * _motorForce * Time.deltaTime;

        Gear currentGear = _gears[_currentGearIndex];
        float torque = Mathf.Clamp(_currentMotorForce * currentGear.gearRatio, -_motorForce, _motorForce); // Ограничиваем крутящий момент
       
        _currentWhellTorque = torque;

        // Применяем мощность на колеса в зависимости от типа привода
        foreach (var wheel in _wheels)
        {
            if (_AWD || (_FWD && wheel.IsForward) || (_RWD && !wheel.IsForward))
            {
                wheel.ApplyMotorTorque(torque);
            }
        }

       

        _speed = _rigidbody.linearVelocity.magnitude * 3.6f; // Convert m/s to km/h
        Debug.Log($"Speed: {_speed}, RPM: {_currentEngineRPM}, Torque: {_currentWhellTorque}, Motor Force: {_currentMotorForce}");
    }

    // Debug.Log($"RPM: {_currentEngineRPM}, Torque: {_currentWhellTorque}, Speed: {_speed}");


private float CalculateEngineRPM(float input)
    {
        bool isGasPresed = Mathf.Abs(input) > 0.1f;

        if (isGasPresed)
        {
            _currentEngineRPM += _rpmUpSpeed * Time.deltaTime;
        }
        else
        {
            _currentEngineRPM -= _rpmDownSpeed * Time.deltaTime;
            if (_speed < 0.1f)
            {
                _currentEngineRPM = 0;
            }
        }
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
    
}
