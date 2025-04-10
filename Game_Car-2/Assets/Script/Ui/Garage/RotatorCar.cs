using UnityEngine;
using UnityEngine.UIElements;

public class RotatorCar : MonoBehaviour
{
    [SerializeField] private InputServis _input;
    private Vector3 _rotate;
    private float rotationSpeed = 50;
    void Update()
    {
        if (_input.HorizontalInput != 0)
        {
            _rotate = (Vector3.up * (_input.HorizontalInput* 20) * rotationSpeed * Time.deltaTime);
        }
       else if (_input.Brake)
        {
            _rotate = Vector3.zero;
        }
        transform.Rotate(_rotate);
    }
}
