
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
    [SerializeField] private Wind _wind;

    [Header("Type of car")]
    [SerializeField] private bool _AWD;
    [SerializeField] private bool _RWD;
    [SerializeField] private bool _FWD;


    [Header("Other Values")]
    [SerializeField] private int _motorForce = 2500;
    [SerializeField] private int _maxSteerAngle = 50;
    [SerializeField] private float _brakeTorque = 0;
    [SerializeField] private float _maxSpead = 220;
    [SerializeField] private float _maxReverseSpeed = 40f;

    private float _engineMaxPowerRPM = 9000;
    private int _currentGearIndex = 0;


    private float verticalInput;
    private float horizontalInput;


    [Header("Event Values")]
    [SerializeField] private float _speed;
    [SerializeField] private float _currentEngineRPM; // Оборот двигателя — Engine RPM (RPM = Revolutions Per Minute)
    [SerializeField] private float _currentWhellTorque;
    [SerializeField] private float _currentSteeringAngle;

    [Header("Rmp Values")]
    [SerializeField] private float _rpmUpSpeed = 400f;
    [SerializeField] private float _rpmDownSpeed = 400f;
    public event Action<float, float, float> OnSpeadChanged;

    [Header("Graph Curves")]
    [SerializeField] private AnimationCurve _steeringCurve;

    bool isGasPresed;

    private bool _justShifted = false;
    private float _rpmDropTimer = 0f;
    [SerializeField] private float _rpmDropDuration = 0.5f;
    

    
    private void Awake()
    {
        if (!_AWD && !_RWD && !_FWD)
            _AWD = true;

        _gears = new List<Gear>
{
    new Gear { minRPM = 1000f, maxRPM = 3000f, gearRatio = 3.8f, minRecommendedSpeed = 0f, maxRecommendedSpeed = 30f },
    new Gear { minRPM = 1500f, maxRPM = 3500f, gearRatio = 2.5f, minRecommendedSpeed = 25f, maxRecommendedSpeed = 50f },
    new Gear { minRPM = 2000f, maxRPM = 4000f, gearRatio = 1.8f, minRecommendedSpeed = 45f, maxRecommendedSpeed = 70f },
    new Gear { minRPM = 2500f, maxRPM = 4500f, gearRatio = 1.3f, minRecommendedSpeed = 65f, maxRecommendedSpeed = 90f },
    new Gear { minRPM = 3000f, maxRPM = 5000f, gearRatio = 1.0f, minRecommendedSpeed = 85f, maxRecommendedSpeed = 120f },
    new Gear { minRPM = 3500f, maxRPM = 5500f, gearRatio = 0.85f, minRecommendedSpeed = 110f, maxRecommendedSpeed = 220f }
};
    }
    private void Start()
    {
        _rigidbody.centerOfMass = _car.InverseTransformPoint(_centreOfMass.position);

        _currentEngineRPM = 0;
        _currentGearIndex = 1;

    }
    public void Move(float vertical, float horisontal, bool brake)
    {
        isGasPresed = Mathf.Abs(verticalInput) > 0.1f;

        SteerWheels(horisontal);
        Engine(vertical);
        ApplyHandbrake(brake);
        
        OnSpeadChanged?.Invoke(_speed, _currentEngineRPM, _currentWhellTorque);
    }
    private void Engine(float input)
    {
        verticalInput = input;


        Gear currentGear = _gears[_currentGearIndex];

        _currentEngineRPM = CalculateEngineRPM(isGasPresed, verticalInput);

        float directionMultiplier = Mathf.Sign(verticalInput);
        float torque = CalculateWheelTorque(_currentEngineRPM, currentGear.gearRatio) * directionMultiplier;

        // Ограничим заднюю скорость
        if (_speed > _maxReverseSpeed && verticalInput < 0)
        {
            torque = 0f;
        }

        _currentWhellTorque = torque;
     
        foreach (var wheel in _wheels)
        {
            if (_AWD || (_FWD && wheel.IsForward) || (_RWD && !wheel.IsForward))
            {
                wheel.ApplyMotorTorque(_currentWhellTorque);

            }


        }

        _speed = _rigidbody.linearVelocity.magnitude * 3.6f;

        SimulateResistance();

        if (_speed < 1 && !isGasPresed)
        {
            _rigidbody.linearVelocity = Vector3.zero;

        }

        UpdateGear();

        OnSpeadChanged?.Invoke(_speed, _currentEngineRPM, _currentWhellTorque);
    }

    private float CalculateWheelTorque(float engineRPM, float gearRatio)
    {
        if (!isGasPresed)
            return 0f;

        return (engineRPM / _engineMaxPowerRPM) * (_motorForce * gearRatio);

    }
    private float CalculateEngineRPM(bool gas, float input)
    {
        Gear currentGear = _gears[_currentGearIndex];

        float targetRPM = gas ? _engineMaxPowerRPM * Mathf.Abs(input) : 0f;

        if (_justShifted && _rpmDropTimer > 0f)
        {
            // Если только что переключили передачу, устанавливаем обороты в 0
            _rpmDropTimer -= Time.fixedDeltaTime;
            _currentEngineRPM = Mathf.Lerp(_currentEngineRPM, 0f, 1f - (_rpmDropTimer / _rpmDropDuration)); // Линейно снижаем до 0

            if (_rpmDropTimer <= 0f)
            {
                _justShifted = false;
            }
        }
        else
        {
            // Наращиваем или уменьшаем обороты по целевому значению
            if (targetRPM > _currentEngineRPM)
                _currentEngineRPM = Mathf.MoveTowards(_currentEngineRPM, targetRPM, _rpmUpSpeed * Time.fixedDeltaTime);
            else
                _currentEngineRPM = Mathf.MoveTowards(_currentEngineRPM, targetRPM, _rpmDownSpeed * Time.fixedDeltaTime);
        }

        // Убираем ограничения на минимальные обороты, так как теперь они будут уменьшаться до 0
        _currentEngineRPM = Mathf.Clamp(_currentEngineRPM, 0f, _engineMaxPowerRPM);
        return _currentEngineRPM;
    }


    private void UpdateGear()
    {
        if (_gears == null || _gears.Count <= 1) return;
        if (verticalInput < 0.1f) return;

        Gear currentGear = _gears[_currentGearIndex];
        

        if (_currentEngineRPM >= currentGear.maxRPM && _currentGearIndex < _gears.Count - 1)
        {
            Gear nextGear = _gears[_currentGearIndex + 1];
        
            int minRecommendedSpeedInt = (int)nextGear.minRecommendedSpeed;

            // Переключаем только если скорость находится хотя бы рядом с рекомендуемым диапазоном
            if (_speed >= nextGear.minRecommendedSpeed )
            {
                _currentGearIndex++;
                _justShifted = true;
                _rpmDropTimer = _rpmDropDuration;
                Debug.Log("⏫ Shift up: " + _currentGearIndex + "  = Spead: " + _speed + " MotorForce :" + _motorForce + " RpmUpSpeed: " + _rpmUpSpeed 
                    + " nextGear.minRecommendedSpeed " + nextGear.minRecommendedSpeed + ("RPM: " + _currentEngineRPM + " | maxRPM: " + currentGear.maxRPM));
            }
        }
        else if (_currentEngineRPM <= currentGear.minRPM && _currentGearIndex > 0)
        {
            Gear prevGear = _gears[_currentGearIndex - 1];

            // Переключаем только если скорость не превышает рекомендованный максимум предыдущей
            if (_speed <= prevGear.maxRecommendedSpeed) // небольшой запас
            {
                _currentGearIndex--;
                _justShifted = true;
                _rpmDropTimer = _rpmDropDuration;
                Debug.Log("⏬ Shift down: " + _currentGearIndex + "  = Spead: " + _speed + " MotorForce :" + _motorForce + " RpmUpSpeed: " + _rpmUpSpeed);
            }
        }
        
    }


    [Header(" воздух")]
    [SerializeField] private float _dragCoefficient = 0.4257f;
    [SerializeField] private float _rollingResistance = 12.8f;

    private void SimulateResistance()
    {
        if (_rigidbody == null) return;

        float speedMS = _speed / 3.6f; // из км/ч в м/с

        // Воздушное сопротивление с учетом ветра
        Vector3 windResistance = Vector3.zero;
        if (_wind != null)
        {
            windResistance = _wind.CalculateWindResistance(_rigidbody.position, _rigidbody.linearVelocity);  // используем linearVelocity
        }

        float airDrag = _dragCoefficient * speedMS * speedMS;
        Vector3 totalResistance = -_rigidbody.linearVelocity.normalized * (airDrag + windResistance.magnitude);

        // Сопротивление качению
        float rollingDrag = _rollingResistance * speedMS;
        totalResistance += -_rigidbody.linearVelocity.normalized * rollingDrag;

        // Добавим сопротивление при отсутствии газа
        if (Mathf.Abs(verticalInput) < 0.1f)
        {
            totalResistance *= 1.5f; // Увеличиваем сопротивление на холостых
        }

        _rigidbody.AddForce(totalResistance); // Применяем сопротивление
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



        ApplySteering(steeringAngle);


    }


    [Header(" Braek ")]
    [SerializeField] private float _handbrakeTorque = 500000f;
    [SerializeField] private float _handbrakeRampSpeed = 2f;

    private float _currentHandbrakeTorque = 0f;
    private void ApplyHandbrake(bool brake)
    {

        float targetTorque = brake ? _handbrakeTorque : 0f;
        _currentHandbrakeTorque = Mathf.MoveTowards(_currentHandbrakeTorque, targetTorque, _handbrakeRampSpeed * Time.fixedDeltaTime * _handbrakeTorque);

        foreach (var wheel in _wheels)
        {
            if (!wheel.IsForward)
            {
                wheel.ApplyBrakeTorque(_currentHandbrakeTorque);

            }
        }
    }
    private void ApplyBrake()
    {
        /*  if (_speed > 1f)  // Если скорость больше порога
          {
              // 1. Тормоз на холостом ходу, который зависит от скорости
              float idleBrakeForce = Mathf.Lerp(0f, _brakeTorque, Mathf.Clamp01(_speed / _maxSpead));

              // 2. Дополнительное торможение при движении назад
              float reverseBrakeForce = 0f;
              if (verticalInput < 0f && Mathf.Abs(_speed) > 0f) // Если движемся назад и есть скорость
              {
                  // Применяем тормоз в зависимости от скорости и усиления торможения на обратной передаче
                  reverseBrakeForce = Mathf.Lerp(0f, _brakeTorque, Mathf.Clamp01(Mathf.Abs(_speed) / _maxReverseSpeed));
              }

              float totalBrakeForce = (idleBrakeForce + reverseBrakeForce )*3;

              // Применяем общий тормозной момент
              foreach (var wheel in _wheels)
              {
                  if (wheel)
                  {
                      wheel.ApplyBrakeTorque(totalBrakeForce);  // Применяем тормозной момент
                  }
              }
          }
          else
          {
              // Если скорость меньше порога, применяем минимальное сопротивление или вообще ничего
              foreach (var wheel in _wheels)
              {
                  if (wheel)
                  {
                      wheel.ApplyBrakeTorque(0f);  // Не применяем тормоза, если скорость слишком низкая
                  }
              }

            }
         */
        foreach (var wheel in _wheels)
        {
            var _brakeTorqueS = Mathf.Lerp(0f, _brakeTorque, Mathf.Clamp01(_speed / _maxSpead));
            if (_speed > 1f)
            {
                wheel.ApplyBrakeTorque(_brakeTorque);  // Применяем тормозной момент
            }

            else
            {
                // Если скорость меньше порога, применяем минимальное сопротивление или вообще ничего



                wheel.ApplyBrakeTorque(0f);  // Не применяем тормоза, если скорость слишком низкая


            }

        }
    }
}