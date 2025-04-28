using UnityEngine;

public class FlyCarPhyisic : MonoBehaviour
{

 
    [SerializeField] private float rotationSmoothness = 2f;
    [SerializeField] private float groundCheckDistance = 1.0f;

    private Rigidbody _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Проверка: машина в воздухе?
        if (!IsGrounded())
        {
            AutoLevel();
        }
    }

    private void AutoLevel()
    {
        // Сохраняем текущий угол по Y (направление движения)
        float yRotation = transform.eulerAngles.y;

        // Целевая ориентация — по оси Y, без наклона по X и Z
        Quaternion targetRotation = Quaternion.Euler(0, yRotation, 0);

        // Плавно вращаем
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSmoothness);
    }

    private bool IsGrounded()
    {
        // Простой рейкаст вниз — если не касается земли, значит в воздухе
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }
}
