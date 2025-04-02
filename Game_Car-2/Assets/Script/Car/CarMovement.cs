using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private Wheel[] _wheels;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _centreOfMass;
    [SerializeField] private AnimationCurve _steeringCuve;
    [SerializeField] private Transform _Car;

    private float _VerticalInput;
    private float _HorizontalInput;
    private float _BrakeForce;


    [SerializeField] private float _speed;

    [SerializeField] private int _brakeForse;
    [SerializeField] private int _motorForse;
    [SerializeField] private int _wheelsAngleMax = 50;
    [SerializeField] private int _brakeTorque = 100000;

    [SerializeField] private bool _fourWheelDrive;
    [SerializeField] private bool _frontWheelDrive;
    [SerializeField] private bool _rearWheelDrive;

    [SerializeField] private float _accelerationRate = 5f; 
    [SerializeField] private float _decelerationRate = 7f; 
    private float _currentMotorForce = 0f; 
    float movingDorection;

    [SerializeField] private bool Drifft;
    [SerializeField] private bool Real;
    [SerializeField] private bool Rally;
    private void Start()
    {
        _rb.centerOfMass = _centreOfMass.position;
        TupeOfCar();

    }

  
    public void Move(float VerticalInput )
    {
        _speed = _rb.linearVelocity.magnitude;
        //Debug.Log(_speed);

        _VerticalInput = VerticalInput;

        if (_VerticalInput != 0)
        {
            _currentMotorForce = Mathf.MoveTowards(_currentMotorForce, this._VerticalInput * _motorForse, _accelerationRate * Time.deltaTime);
        }
        else
        {
            _currentMotorForce = Mathf.MoveTowards(_currentMotorForce, 0, _decelerationRate * Time.deltaTime);
        }

        if (_fourWheelDrive == true)
        {

            foreach (Wheel wheel in _wheels)
            {
                wheel._wheelCollider.motorTorque = _currentMotorForce;
            }
        }
        if (_rearWheelDrive)
        {
            foreach (Wheel wheel in _wheels)
            {
                if (wheel.IsForward == false)
                    wheel._wheelCollider.motorTorque = _currentMotorForce * 2;
                else
                    wheel._wheelCollider.motorTorque = 0;
            }
        }
        if (_frontWheelDrive)
        {
            foreach (Wheel wheel in _wheels)
            {
                if (wheel.IsForward)
                    wheel._wheelCollider.motorTorque = _currentMotorForce * 2;
                else
                    wheel._wheelCollider.motorTorque = 0;
            }

        }
        

    }


    public void Steering(float HorizontalInput)
    
    {
        _HorizontalInput = HorizontalInput;
        float steeringAngle = _HorizontalInput * _steeringCuve.Evaluate(_speed);
        float slipAngle = Vector3.Angle(_Car.forward, _rb.linearVelocity.normalized - _Car.forward);

        if (slipAngle < 120 && _rb.linearVelocity.magnitude > 1f)
            steeringAngle += Vector3.SignedAngle(_Car.forward, _rb.linearVelocity.normalized, Vector3.up);

        steeringAngle = Mathf.Clamp(steeringAngle, -_wheelsAngleMax, _wheelsAngleMax);


        foreach (Wheel wheel in _wheels)
        {
            if (wheel.IsForward)
            {
                wheel._wheelCollider.steerAngle = steeringAngle;
            }

        }
    }

    public void Brake( bool Brake)
    {

        foreach (Wheel wheel in _wheels)
        {
            _BrakeForce = (wheel.IsForward == true ? wheel.BrakeForceForwardWheel : wheel.BrakeForceRearWheel);

            if (Brake)
            {

                wheel._wheelCollider.brakeTorque = _brakeTorque;
                wheel._wheelCollider.motorTorque = 0;
            }
            else
            {

                wheel._wheelCollider.brakeTorque = 0;


                float motorForce = _VerticalInput * _motorForse;
                if (_rearWheelDrive && !wheel.IsForward)
                    wheel._wheelCollider.motorTorque = motorForce;
                else if (_frontWheelDrive && wheel.IsForward)
                    wheel._wheelCollider.motorTorque = motorForce;
                else if (_fourWheelDrive)
                    wheel._wheelCollider.motorTorque = motorForce;
            }

        }
        movingDorection = Vector3.Dot(_Car.forward, _rb.linearVelocity);
        _BrakeForce = ((movingDorection < -0.5f && _VerticalInput > 0) || (movingDorection > 0.5f && _VerticalInput < 0)) ? Mathf.Abs(_VerticalInput) : 0;
    }

    
    
    private void TupeOfCar()
    {
        if (Drifft)
        {
            foreach (Wheel wheel in _wheels)
            {
                wheel.SetupDriftCar();
            }

            _rb.mass = 1500f; // Стандартная масса для дрифт-кара (легче для маневров)
            _rb.linearDamping = 0.2f; // Низкое сопротивление для быстрого отклика
            _rb.angularDamping = 2f; // Чуть более высокий коэффициент для контроля вращения
            _rb.centerOfMass = new Vector3(0, -0.5f, 0); // Центр массы смещён ниже, чтобы улучшить стабильность в поворотах

            // Сила торможения (меньше, чтобы не блокировать колёса при дрифте)
            _brakeForse = 3000;

            // Сила мотора (повышенная для быстрого разгона)
            _motorForse = 6000;

            // Максимальный угол поворота колёс (больший для улучшенной маневренности при дрифте)
            _wheelsAngleMax = 45;

            // Тормозной момент (небольшой, чтобы предотвратить блокировку)
            _brakeTorque = 15000;

            // Для дрифта — привод на задние колёса (передний привод может мешать дрифту)
            _rearWheelDrive = true;
            _fourWheelDrive = false;
            _frontWheelDrive = false;

            // Подвеска для дрифта
            _accelerationRate = 6f;  // Увеличиваем ускорение
            _decelerationRate = 4f;  // Меньше замедление


        }
        if (Rally)
        {
            foreach (Wheel wheel in _wheels)
            {
                wheel.SetupRallyCar();
            }

            _rb.mass = 1600f; // Стандартная масса для раллийного авто
            _rb.linearDamping = 0.3f; // Небольшое сопротивление для быстрого отклика
            _rb.angularDamping = 3f; // Умеренная инерция вращения
            _rb.centerOfMass = new Vector3(0, -0.3f, 0); // Центр массы немного выше для лучшей устойчивости

            // Сила торможения (умеренная для стабильности)
            _brakeForse = 5000;

            // Сила мотора (умеренная для равномерного разгона)
            _motorForse = 4500;

            // Максимальный угол поворота колёс (более ограниченный угол для лучшей устойчивости на пересечённой местности)
            _wheelsAngleMax = 35;

            // Тормозной момент (нормальный)
            _brakeTorque = 30000;

            // Для ралли — полный привод
            _rearWheelDrive = false;
            _frontWheelDrive = false;
            _fourWheelDrive = true;

            // Подвеска для ралли
            _accelerationRate = 4f;  // Умеренное ускорение
            _decelerationRate = 5f;  // Умеренное замедление

        }
        if (Real)
        {
            foreach (Wheel wheel in _wheels)
            {
                wheel.SetupStandardCar();
            }

            _rb.mass = 1700f; // Масса стандартного автомобиля
            _rb.linearDamping = 0.4f; // Более высокое сопротивление для устойчивости
            _rb.angularDamping = 4f; // Низкая инерция для контролируемости
            _rb.centerOfMass = new Vector3(0, -0.25f, 0); // Центр массы близко к центру автомобиля


            // Сила торможения (стандартная)
            _brakeForse = 4000;

            // Сила мотора (стандартная для обычной езды)
            _motorForse = 5000;

            // Максимальный угол поворота колёс (умеренный для стабильности)
            _wheelsAngleMax = 30;

            // Тормозной момент (стандартный)
            _brakeTorque = 25000;

            // Стандартный полный привод
            _rearWheelDrive = true;
            _fourWheelDrive = false;
            _frontWheelDrive = false;

            // Подвеска для стандартного авто
            _accelerationRate = 4f;  // Умеренное ускорение
            _decelerationRate = 5f;  // Умеренное замедление

        }
    }
}
