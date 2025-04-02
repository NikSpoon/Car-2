using UnityEngine;

public class Movement : MonoBehaviour
{
   
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] float speed = 10;

    public Vector3 direction { get;  set; }

    void FixedUpdate()
    {
        var nextPosition = _rigidbody.position + direction * speed * Time.fixedDeltaTime;
        _rigidbody.MovePosition(nextPosition);
    }
}
