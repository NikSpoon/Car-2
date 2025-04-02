using Unity.VisualScripting;
using UnityEngine;

public class Wheel : MonoBehaviour
{   
    [SerializeField] private Transform _wheelMesh;
    [SerializeField] public WheelCollider _wheelCollider;
    [SerializeField] public bool IsForward;
    [SerializeField] public int BrakeForceForwardWheel = 2000;
    [SerializeField] public int BrakeForceRearWheel = 800;

    public void Start()
    {

        
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
    public void SetupDriftCar()
    {
        if (IsForward)
            SetupDriftCarFront(_wheelCollider);
        else
            SetupDriftCarRear(_wheelCollider);

    }
    public void SetupRallyCar()
    {
        if (IsForward)
            SetupRallyCarFront(_wheelCollider);
        else
            SetupRallyCarRear(_wheelCollider);
    }
    public void SetupStandardCar()
    {
        if (IsForward)
            SetupStandardCarFront(_wheelCollider);
        else
            SetupStandardCarRear(_wheelCollider);
    }
    void SetupDriftCarRear(WheelCollider wc)
    {
        wc.suspensionSpring = new JointSpring { spring = 25000f, damper = 2500f, targetPosition = 0.35f };
        wc.suspensionDistance = 0.2f;

        WheelFrictionCurve forwardFriction = wc.forwardFriction;
        forwardFriction.extremumSlip = 0.45f;  // Возможность пробуксовки
        forwardFriction.extremumValue = 1.2f;
        forwardFriction.asymptoteSlip = 0.7f;
        forwardFriction.asymptoteValue = 0.8f;
        forwardFriction.stiffness = 1.0f;
        wc.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wc.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.55f;  // Увеличенная пробуксовка
        sidewaysFriction.extremumValue = 1.4f;
        sidewaysFriction.asymptoteSlip = 0.8f;
        sidewaysFriction.asymptoteValue = 0.7f;
        sidewaysFriction.stiffness = 0.6f;  // Меньше сцепление для задних колёс
        wc.sidewaysFriction = sidewaysFriction;
    }


    void SetupDriftCarFront(WheelCollider wc)
    {
        wc.suspensionSpring = new JointSpring { spring = 30000f, damper = 3000f, targetPosition = 0.35f };
        wc.suspensionDistance = 0.2f;

        WheelFrictionCurve forwardFriction = wc.forwardFriction;
        forwardFriction.extremumSlip = 0.3f;
        forwardFriction.extremumValue = 1.5f;
        forwardFriction.asymptoteSlip = 0.6f;
        forwardFriction.asymptoteValue = 0.8f;
        forwardFriction.stiffness = 1.5f;  // Больше сцепление для передних колёс
        wc.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wc.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.45f;
        sidewaysFriction.extremumValue = 1.3f;
        sidewaysFriction.asymptoteSlip = 0.7f;
        sidewaysFriction.asymptoteValue = 0.8f;
        sidewaysFriction.stiffness = 1.0f;
        wc.sidewaysFriction = sidewaysFriction;
    }

    void SetupRallyCarRear(WheelCollider wc)
    {
        wc.suspensionSpring = new JointSpring { spring = 45000f, damper = 4000f, targetPosition = 0.5f };
        wc.suspensionDistance = 0.3f;

        WheelFrictionCurve forwardFriction = wc.forwardFriction;
        forwardFriction.extremumSlip = 0.35f;
        forwardFriction.extremumValue = 1.5f;
        forwardFriction.asymptoteSlip = 0.6f;
        forwardFriction.asymptoteValue = 0.8f;
        forwardFriction.stiffness = 1.6f;
        wc.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wc.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.25f;
        sidewaysFriction.extremumValue = 1.6f;
        sidewaysFriction.asymptoteSlip = 0.6f;
        sidewaysFriction.asymptoteValue = 0.9f;
        sidewaysFriction.stiffness = 1.6f;
        wc.sidewaysFriction = sidewaysFriction;
    }


    void SetupRallyCarFront(WheelCollider wc)
    {
        wc.suspensionSpring = new JointSpring { spring = 45000f, damper = 4000f, targetPosition = 0.5f };
        wc.suspensionDistance = 0.3f;

        WheelFrictionCurve forwardFriction = wc.forwardFriction;
        forwardFriction.extremumSlip = 0.35f;
        forwardFriction.extremumValue = 1.5f;
        forwardFriction.asymptoteSlip = 0.6f;
        forwardFriction.asymptoteValue = 0.8f;
        forwardFriction.stiffness = 1.7f;
        wc.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wc.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.25f;
        sidewaysFriction.extremumValue = 1.6f;
        sidewaysFriction.asymptoteSlip = 0.6f;
        sidewaysFriction.asymptoteValue = 0.9f;
        sidewaysFriction.stiffness = 1.7f;
        wc.sidewaysFriction = sidewaysFriction;
    }

    void SetupStandardCarRear(WheelCollider wc)
    {
        wc.suspensionSpring = new JointSpring { spring = 30_000f, damper = 3_500f, targetPosition = 0.45f };
        wc.suspensionDistance = 0.25f;

        WheelFrictionCurve forwardFriction = wc.forwardFriction;
        forwardFriction.extremumSlip = 0.2f;
        forwardFriction.extremumValue = 1.7f;
        forwardFriction.asymptoteSlip = 0.5f;
        forwardFriction.asymptoteValue = 0.9f;
        forwardFriction.stiffness = 2.0f;
        wc.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wc.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.15f;
        sidewaysFriction.extremumValue = 1.8f;
        sidewaysFriction.asymptoteSlip = 0.4f;
        sidewaysFriction.asymptoteValue = 1.0f;
        sidewaysFriction.stiffness = 2.0f;
        wc.sidewaysFriction = sidewaysFriction;
    }

    // Передние колеса
    void SetupStandardCarFront(WheelCollider wc)
    {
        wc.suspensionSpring = new JointSpring { spring = 30_000f, damper = 3_500f, targetPosition = 0.45f };
        wc.suspensionDistance = 0.25f;

        WheelFrictionCurve forwardFriction = wc.forwardFriction;
        forwardFriction.extremumSlip = 0.2f;
        forwardFriction.extremumValue = 1.7f;
        forwardFriction.asymptoteSlip = 0.5f;
        forwardFriction.asymptoteValue = 0.9f;
        forwardFriction.stiffness = 2.0f;
        wc.forwardFriction = forwardFriction;

        WheelFrictionCurve sidewaysFriction = wc.sidewaysFriction;
        sidewaysFriction.extremumSlip = 0.2f;
        sidewaysFriction.extremumValue = 1.8f;
        sidewaysFriction.asymptoteSlip = 0.4f;
        sidewaysFriction.asymptoteValue = 1.0f;
        sidewaysFriction.stiffness = 2.0f;
        wc.sidewaysFriction = sidewaysFriction;
    }
}
