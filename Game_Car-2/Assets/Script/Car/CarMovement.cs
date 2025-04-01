using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] private Wheel[] _wheels;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _centreOfMass;
    [SerializeField] private AnimationCurve _steeringCuve;
    [SerializeField] private Transform _Car;

    private float _vertocalInput;
    private float _horizontalInput;
    private float _brakeInput;


    [SerializeField] private float _speed;

    [SerializeField] private int _brakeForse;
    [SerializeField] private int _motorForse;
    [SerializeField] private int _whelsAngleMax = 50;
    [SerializeField] private int _brakeTorque = 100000;

    [SerializeField] private bool _fourWheelDrive;
    [SerializeField] private bool _frontWheelDrive;
    [SerializeField] private bool _rearWheelDrive;

    [SerializeField] private float _accelerationRate = 5f; 
    [SerializeField] private float _decelerationRate = 7f; 
    private float _currentMotorForce = 0f; 
    float movingDorection;
    private void Start()
    {
        _rb.centerOfMass = _centreOfMass.position;

    }
   
    public void Move(float VerticalInput )
    {
        _speed = _rb.linearVelocity.magnitude;
        Debug.Log(_speed);

        _vertocalInput = VerticalInput;

        if (_vertocalInput != 0)
        {
            _currentMotorForce = Mathf.MoveTowards(_currentMotorForce, _vertocalInput * _motorForse, _accelerationRate * Time.deltaTime);
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
                if (wheel._isForvord == false)
                    wheel._wheelCollider.motorTorque = _currentMotorForce * 2;
                else
                    wheel._wheelCollider.motorTorque = 0;
            }
        }
        if (_frontWheelDrive)
        {
            foreach (Wheel wheel in _wheels)
            {
                if (wheel._isForvord)
                    wheel._wheelCollider.motorTorque = _currentMotorForce * 2;
                else
                    wheel._wheelCollider.motorTorque = 0;
            }

        }
        

    }


    public void Steeting(float HorizontalInput)
    {
        _horizontalInput = HorizontalInput;
        float steeringAngle = _horizontalInput * _steeringCuve.Evaluate(_speed);
        float slipAngle = Vector3.Angle(_Car.forward, _rb.linearVelocity.normalized - _Car.forward);

        if (slipAngle < 120 && _rb.linearVelocity.magnitude > 1f)
            steeringAngle += Vector3.SignedAngle(_Car.forward, _rb.linearVelocity.normalized, Vector3.up);

        steeringAngle = Mathf.Clamp(steeringAngle, -_whelsAngleMax, _whelsAngleMax);


        foreach (Wheel wheel in _wheels)
        {
            if (wheel._isForvord)
            {
                wheel._wheelCollider.steerAngle = steeringAngle;
            }

        }
    }

    public void Brake( bool Brake)
    {

        foreach (Wheel wheel in _wheels)
        {
            _brakeInput = (wheel._isForvord == true ? wheel._brakeForseForvordWheel : wheel._brakeForseRearWheel);

            if (Brake)
            {

                wheel._wheelCollider.brakeTorque = _brakeTorque;
                wheel._wheelCollider.motorTorque = 0;
            }
            else
            {

                wheel._wheelCollider.brakeTorque = 0;


                float motorForce = _vertocalInput * _motorForse;
                if (_rearWheelDrive && !wheel._isForvord)
                    wheel._wheelCollider.motorTorque = motorForce;
                else if (_frontWheelDrive && wheel._isForvord)
                    wheel._wheelCollider.motorTorque = motorForce;
                else if (_fourWheelDrive)
                    wheel._wheelCollider.motorTorque = motorForce;
            }

        }
        movingDorection = Vector3.Dot(_Car.forward, _rb.linearVelocity);
        _brakeInput = ((movingDorection < -0.5f && _vertocalInput > 0) || (movingDorection > 0.5f && _vertocalInput < 0)) ? Mathf.Abs(_vertocalInput) : 0;
    }

    

}
