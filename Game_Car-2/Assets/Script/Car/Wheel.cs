using Unity.VisualScripting;
using UnityEngine;

public class Wheel : MonoBehaviour
{   
    [SerializeField] private Transform _wheelMash;
    [SerializeField] public WheelCollider _wheelCollider;
    [SerializeField] public bool _isForvord;
    [SerializeField] public int _brakeForseForvordWheel = 2000;
    [SerializeField] public int _brakeForseRearWheel = 800;

    public void Start()
    {
       
       if (_isForvord)
        {
            _wheelCollider.mass = 25f;
            _wheelCollider.radius = 0.35f;
            _wheelCollider.suspensionDistance = 0.25f;

            // Подвеска  
            JointSpring suspension = _wheelCollider.suspensionSpring;
            suspension.spring = 48000f;
            suspension.damper = 6000f;
            suspension.targetPosition = 0.4f;
            _wheelCollider.suspensionSpring = suspension;

            // Продольное сцепление (ускорение/торможение)  
            WheelFrictionCurve forwardFriction = _wheelCollider.forwardFriction;
            forwardFriction.extremumSlip = 0.25f;
            forwardFriction.extremumValue = 1.3f;
            forwardFriction.asymptoteSlip = 0.75f;
            forwardFriction.asymptoteValue = 0.9f;
            forwardFriction.stiffness = 1.2f;
            _wheelCollider.forwardFriction = forwardFriction;

            // Боковое сцепление (повороты)  
            WheelFrictionCurve sidewaysFriction = _wheelCollider.sidewaysFriction;
            sidewaysFriction.extremumSlip = 0.4f;
            sidewaysFriction.extremumValue = 1.1f;
            sidewaysFriction.asymptoteSlip = 1.0f;
            sidewaysFriction.asymptoteValue = 0.85f;
            sidewaysFriction.stiffness = 0.85f;
            _wheelCollider.sidewaysFriction = sidewaysFriction;

        }
       else
        {
            _wheelCollider.mass = 27f;
            _wheelCollider.radius = 0.35f;
            _wheelCollider.suspensionDistance = 0.27f;

            // Подвеска  
            JointSpring suspension = _wheelCollider.suspensionSpring;
            suspension.spring = 45000f;
            suspension.damper = 5500f;
            suspension.targetPosition = 0.45f;
            _wheelCollider.suspensionSpring = suspension;

            // Продольное сцепление  
            WheelFrictionCurve forwardFriction = _wheelCollider.forwardFriction;
            forwardFriction.extremumSlip = 0.3f;
            forwardFriction.extremumValue = 1.2f;
            forwardFriction.asymptoteSlip = 0.9f;
            forwardFriction.asymptoteValue = 0.8f;
            forwardFriction.stiffness = 1.0f;
            _wheelCollider.forwardFriction = forwardFriction;

            // Боковое сцепление  
            WheelFrictionCurve sidewaysFriction = _wheelCollider.sidewaysFriction;
            sidewaysFriction.extremumSlip = 0.55f;  // Чуть выше, чтобы легче входить в занос  
            sidewaysFriction.extremumValue = 1.0f;
            sidewaysFriction.asymptoteSlip = 1.3f;
            sidewaysFriction.asymptoteValue = 0.7f;
            sidewaysFriction.stiffness = 0.65f;  // Позволяет контролируемый занос  
            _wheelCollider.sidewaysFriction = sidewaysFriction;

        }

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

        _wheelMash.position = position; 
        _wheelMash.rotation = rotation;

    }

  
   
}
