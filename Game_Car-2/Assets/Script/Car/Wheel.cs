using Unity.VisualScripting;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private Transform _wheelMesh;
    [SerializeField] public WheelCollider _wheelCollider;
    [SerializeField] public bool IsForward;

    public void Start()
    {

        SetupWheelCollider(_wheelCollider);
    }
    public void Update()
    {
        UpdateWheelPositionAndRotation();

    }

    private void UpdateWheelPositionAndRotation()
    {
        Vector3 position;
        Quaternion rotation;

        _wheelCollider.GetWorldPose(out position, out rotation);

        _wheelMesh.position = position;
        _wheelMesh.rotation = rotation;

    }
    public void ApplyMotorTorque(float torque)
    {
           
        if (Mathf.Approximately(torque, 0f))
        {
            _wheelCollider.motorTorque = 0f;
        }
        else
            {
                _wheelCollider.motorTorque = torque;
            }
    }
    public void ApplySteerAngle(float angle)
    {
        _wheelCollider.steerAngle = angle;
    }
    public void ApplyBrakeTorque(float brake)
    {
        _wheelCollider.brakeTorque = brake;  
      
    }
    private void SetupWheelCollider(WheelCollider wheelCollider)
    {

        wheelCollider.mass = 25f;
        
        wheelCollider.suspensionDistance = 0.25f;
        wheelCollider.wheelDampingRate = 1.0f;

        // Подвеска
        JointSpring suspension = new JointSpring();
        suspension.spring = IsForward ? 45000f : 40000f;
        suspension.damper = 3500f;
        suspension.targetPosition = 0.45f;
        wheelCollider.suspensionSpring = suspension;

        // 📈 Фрикция вперёд-назад
        WheelFrictionCurve forwardFriction = wheelCollider.forwardFriction;
        forwardFriction.extremumSlip = 0.4f;
        forwardFriction.extremumValue = 1.6f;
        forwardFriction.asymptoteSlip = 0.8f;
        forwardFriction.asymptoteValue = 0.9f;
        forwardFriction.stiffness = IsForward ? 1.15f : 1.85f; // Задним больше тяги
        wheelCollider.forwardFriction = forwardFriction;

        // 🔁 Боковое сцепление
        WheelFrictionCurve sidewaysFriction = wheelCollider.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.3f;
        sidewaysFriction.extremumValue = 1.5f;
        sidewaysFriction.asymptoteSlip = 0.55f;
        sidewaysFriction.asymptoteValue = 0.85f;
        sidewaysFriction.stiffness = IsForward ? 1.25f : 1.1f; // Задние легче срываются
        wheelCollider.sidewaysFriction = sidewaysFriction;
    }
    }


