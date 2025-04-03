using Unity.VisualScripting;
using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    [SerializeField] private GameObject[] _target;
    public Transform CurrentTarget { get; private set; }

    private int _currentTargetIndex = 0;
    public float VerticalInput { get; private set; }
    public float HorizontalInput { get; private set; }
    public bool Brake { get; private set; }
    private void Awake()
    {

        if (_target.Length == 0)
        {
            Debug.LogError("EnemyCar: No waypoints assigned!");
            enabled = false;
            return;
        }

        CurrentTarget = _target[_currentTargetIndex].transform;
        transform.position = CurrentTarget.position + new Vector3(0, 10, 0);

    }
    void Start()
    {

        _target[_currentTargetIndex].SetActive(true);
    }
    private void Update()
    {
        Debug.Log(_currentTargetIndex);
    }
    private void OnTriggerEnter(Collider collision)
   {
   
        Debug.Log("Collision Detected with: " + collision.gameObject.name); 
        if (collision.gameObject == _target[_currentTargetIndex])
        {
            Debug.Log("Correct target hit!");
            _target[_currentTargetIndex].SetActive(false);

            if (_currentTargetIndex == _target.Length - 1)
            {
                _currentTargetIndex = 0;
            }
            else
            {
                Debug.Log("++");
                _currentTargetIndex++;
            }

            _target[_currentTargetIndex].SetActive(true);
            CurrentTarget = _target[_currentTargetIndex].transform;
            
            VerticalInput = SetVerticalInput();
            HorizontalInput = SetHorizontalInput();
        }
        
    }
    private float SetVerticalInput()
    {
        Vector3 direction = CurrentTarget.position - transform.position;
        var verticalInput = Vector3.Dot(transform.forward, direction.normalized);
        return verticalInput;
    }
    private float SetHorizontalInput()
    {
        Vector3 direction = CurrentTarget.position - transform.position;
        float angelToTarget = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        return angelToTarget;
    }

}
