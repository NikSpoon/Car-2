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
       

        // Фрикционные силы
        WheelFrictionCurve forwardFriction = wheelCollider.forwardFriction;
        forwardFriction.stiffness = 1.5f;
        wheelCollider.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wheelCollider.sidewaysFriction;
        sidewaysFriction.stiffness = 1.2f;
        wheelCollider.sidewaysFriction = sidewaysFriction;

        // Подвеска
        JointSpring suspension = wheelCollider.suspensionSpring;
        suspension.spring = 50000f;  // Жесткая подвеска
        suspension.damper = 3000f;   // Сопротивление амортизаторов
        suspension.targetPosition = 0.5f; // Нейтральное положение подвески
        wheelCollider.suspensionSpring = suspension;


        // Кривые сцепления
        WheelFrictionCurve forwardFrictionCurve = wheelCollider.forwardFriction;
        forwardFrictionCurve.extremumSlip = 0.5f;
        forwardFrictionCurve.extremumValue = 1.5f;
        wheelCollider.forwardFriction = forwardFrictionCurve;

        WheelFrictionCurve sidewaysFrictionCurve = wheelCollider.sidewaysFriction;
        sidewaysFrictionCurve.extremumSlip = 0.5f;
        sidewaysFrictionCurve.extremumValue = 1.2f;
        wheelCollider.sidewaysFriction = sidewaysFrictionCurve;
    }
}
