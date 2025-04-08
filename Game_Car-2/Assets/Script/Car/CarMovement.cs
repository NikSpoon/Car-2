using System;
using Unity.VisualScripting;
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

    [Header("Привід")]
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

    private Vector3 CurrentTarget;
    
   
  
    

    
}
