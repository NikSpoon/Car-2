using UnityEngine;
using System.Collections;

public class Bosst : MonoBehaviour
{
    private GameObject _car;
    private GameObject _carBody;
    private Rigidbody _carRigidbody;

    [SerializeField] private int _bosst = 25000;

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

            _carRigidbody.AddForce(_carBody.transform.forward * _bosst, ForceMode.Impulse);

            Debug.Log("Сила применена к автомобилю!");
        }

    }
}
