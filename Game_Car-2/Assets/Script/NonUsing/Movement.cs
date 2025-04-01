using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] InputServis _inputServis;
    [SerializeField] Rigidbody _rigidbody;

    [SerializeField] float speed = 1;

    void FixedUpdate()
    {
        var nextPosition = _rigidbody.position + _inputServis.Direction * speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(nextPosition);
    }
}
