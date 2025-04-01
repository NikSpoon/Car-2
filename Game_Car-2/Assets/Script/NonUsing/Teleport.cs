using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;

public class Teleport : MonoBehaviour
{

    private GameObject _car;
    private GameObject _carBody;

    [SerializeField] private Transform _targetTo;
    private Rigidbody _carRigidbody;
    private void Start()
    {
        _car = GameObject.FindGameObjectWithTag("Player");
        _carBody = GameObject.FindGameObjectWithTag("Body");

        if (_car != null)
             _carRigidbody = _car.GetComponent<Rigidbody>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
       
          if (other.gameObject == _carBody)
         {
            
            _carRigidbody.linearVelocity = Vector3.zero;
            _carRigidbody.angularVelocity = Vector3.zero;
            _carRigidbody.Sleep(); 

            _car.transform.position = _targetTo.transform.position; Debug.Log("Автомобіль торкнувся триггера, телепортуємо!");

            _carRigidbody.WakeUp();
         }
        
    }
   
}
