using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] Transform _transform;
    [SerializeField] Vector3 _ofset;
   
 
    void Update()
    {
        transform.position = _transform.position + _ofset;
    }
}
