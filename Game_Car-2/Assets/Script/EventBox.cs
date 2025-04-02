using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventBox : MonoBehaviour
{
    private GameObject _car;
    private GameObject _carBody;
    private Rigidbody _carRigidbody;
    private bool random;

    private CarControler _carControler;
    private Wheel[] _wheels;

    [SerializeField] private Transform _targetTo;
    [SerializeField] private int _bosst = 25000;
    [SerializeField] private int _slow = 15000;
    [SerializeField] private int _jump = 5000;
    [SerializeField] private int _oil = 25000;
    [SerializeField] private int _oilTime = 5;



    [SerializeField] private EffectType _effectType;

    private enum EffectType
    {
        Teleport,
        Boost,
        Slow,
        Jump,
        Oil,
        Random
    }



    private void Start()
    {
        _car = GameObject.FindGameObjectWithTag("Player");
        _carBody = GameObject.FindGameObjectWithTag("Body");



        if (_car != null)
        {
            _carRigidbody = _car.GetComponent<Rigidbody>();
            _carControler = _car.GetComponent<CarControler>();
            _wheels = _carControler.GetComponent<Wheel[]>();
        }

        random = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _carBody)
        {
            if (_effectType == EffectType.Random)
            {

                _effectType = (EffectType)Random.Range(0, System.Enum.GetValues(typeof(EffectType)).Length - 1);
                random = true;
                Debug.Log(_effectType);
            }
            switch (_effectType)
            {

                case EffectType.Teleport:

                    if (random) break;
                    _carRigidbody.linearVelocity = Vector3.zero;
                    _carRigidbody.angularVelocity = Vector3.zero;

                    _carRigidbody.Sleep();

                    _car.transform.position = _targetTo.transform.position; Debug.Log("Автомобіль торкнувся триггера, телепортуємо!");
                    _carRigidbody.WakeUp();
                    break;

                case EffectType.Boost:

                    _carRigidbody.AddForce(_carBody.transform.forward * _bosst, ForceMode.Impulse);

                    Debug.Log("Сила применена к автомобилю!");
                    break;

                case EffectType.Slow:

                    _carRigidbody.AddForce(-_carBody.transform.forward * _slow, ForceMode.Impulse);

                    Debug.Log("Сила применена к автомобилю!");
                    break;

                case EffectType.Jump:

                    _carRigidbody.AddForce(_carBody.transform.up * _jump, ForceMode.Impulse);
                    Debug.Log("Автомобиль подпрыгнул!");
                    break;
                case EffectType.Oil:

                    StartCoroutine(OilTime());
                    Debug.Log("Автомобиль скользит по маслу!");
                    break;


            }

            if (random)
            {
                random = false;
            }
        }




    }
    private IEnumerator OilTime()
    {
        WheelFrictionCurve startValueForvord = new WheelFrictionCurve();
        WheelFrictionCurve startValueRoad = new WheelFrictionCurve();

        foreach (Wheel wheel in _wheels)
        {

            if (wheel.IsForward)
            {
                WheelFrictionCurve stForvord = wheel._wheelCollider.forwardFriction;
                startValueForvord = stForvord;
                stForvord.stiffness = 0.1f;
                wheel._wheelCollider.forwardFriction = stForvord;
            }
            else
            {
                WheelFrictionCurve st = wheel._wheelCollider.forwardFriction;
                startValueRoad = st;
                st.stiffness = 0.1f;
                wheel._wheelCollider.forwardFriction = st;
            }
        }
        yield return new WaitForSeconds(_oilTime);

        foreach (Wheel wheel in _wheels)
        {
            if (wheel.IsForward)
            {
                wheel._wheelCollider.forwardFriction = startValueForvord;

            }
            else
            {
                wheel._wheelCollider.forwardFriction = startValueRoad;
            }
        }
    }
}
