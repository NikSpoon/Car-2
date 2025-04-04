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

        if (_target != null && _target.Length != 0)
        {
        CurrentTarget = _target[_currentTargetIndex].transform;
        transform.position = CurrentTarget.position ;
        }
       



    }
    void Start()
    {
       
    }
  
    private void OnTriggerEnter(Collider collision)
   {
        if (_target == null || _target.Length == 0)
        {
            Debug.LogError("Target array is not initialized or is empty!");
            return;
        }


        if (_currentTargetIndex < 0 || _currentTargetIndex >= _target.Length)
        {
            Debug.LogError($"Index {_currentTargetIndex} is out of bounds! Target length: {_target.Length}");
            return;
        }

        Debug.Log("Collision Detected with: " + collision.gameObject.name); 
        if (collision.gameObject == _target[_currentTargetIndex])
        {

            
           

            if (_currentTargetIndex == _target.Length - 1)
            {
                _currentTargetIndex = 0;
                return;
            }
            else
            {
                
                _currentTargetIndex++;
            }

            
            CurrentTarget = _target[_currentTargetIndex].transform;
            
            VerticalInput = SetVerticalInput();
           
        }
        else
        {
            return;
        }
        
    }
    private float SetVerticalInput()
    {
        Vector3 direction = CurrentTarget.position - transform.position;
        var verticalInput = Vector3.Dot(transform.forward, direction.normalized);
        return verticalInput;
    }
   
    public void InitializeTargets(GameObject[] targets)
    {
        _target = targets;

        if (_target != null && _target.Length > 0)
        {
            _target[0].SetActive(true);
            CurrentTarget = _target[0].transform;
        }
    }
}
