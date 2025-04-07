using UnityEngine;

public class CustomWheel : MonoBehaviour
{

    public Transform wheelVisual;  // Модель колеса (визуал)
    public float suspensionDistance = 0.3f;  // Длина подвески
    public float springStrength = 20000f;    // Жесткость пружины
    public float springDamp = 2000f;         // Демпфирование
    public float wheelRadius = 0.35f;

    private Rigidbody carRigidbody;
    private float prevSpringLength;

    void Start()
    {
        carRigidbody = GetComponentInParent<Rigidbody>();
        prevSpringLength = suspensionDistance;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 down = -transform.up;

        float rayLength = suspensionDistance + wheelRadius;

        // Пускаем луч вниз
        if (Physics.Raycast(origin, down, out hit, rayLength))
        {
            // Текущая длина пружины
            float currentSpringLength = hit.distance - wheelRadius;
            currentSpringLength = Mathf.Clamp(currentSpringLength, 0, suspensionDistance);

            // Расчёт силы сжатия пружины
            float springVelocity = (prevSpringLength - currentSpringLength) / Time.fixedDeltaTime;
            float springForce = (suspensionDistance - currentSpringLength) * springStrength;
            float damperForce = springVelocity * springDamp;

            float totalForce = springForce + damperForce;

            // Применяем силу вверх в точку касания
            carRigidbody.AddForceAtPosition(-down * totalForce, hit.point);

            prevSpringLength = currentSpringLength;

            // Обновляем визуал колеса
            if (wheelVisual != null)
            {
                wheelVisual.position = origin + down * currentSpringLength;
            }
        }
        else
        {
            // Если колесо не касается земли — опускаем визуал в максимально вытянутую позицию
            if (wheelVisual != null)
            {
                wheelVisual.position = origin + down * suspensionDistance;
            }
        }
    }
}


