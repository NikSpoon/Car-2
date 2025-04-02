using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventBox : MonoBehaviour
{
    private GameObject _car;
    private Rigidbody _carRigidbody;
    private bool random;

    private CarMovement _carControler;
    private Wheel[] _wheels;

    [SerializeField] private Transform _targetTo;
    [SerializeField] private int _bosst = 25000;
    [SerializeField] private int _slow = 15000;
    [SerializeField] private int _jump = 5000;
    [SerializeField] private float _oil = 0.3f;
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
        _carRigidbody = _car.GetComponent<Rigidbody>();
        _carControler = _car.GetComponent<CarMovement>();

        _wheels = _carControler.GetComponentsInChildren<Wheel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var ifRandom = false;
        if (other.gameObject == _car)
        {
            if (_effectType == EffectType.Random)
            {
                ifRandom = true;
                _effectType = (EffectType)Random.Range(0, System.Enum.GetValues(typeof(EffectType)).Length - 1);
                random = true;
                Debug.Log(_effectType);
            }
            switch (_effectType)
            {

                case EffectType.Teleport:

                    if (ifRandom) break;

                    _carRigidbody.linearVelocity = Vector3.zero;
                    _carRigidbody.angularVelocity = Vector3.zero;

                    _carRigidbody.Sleep();

                    _car.transform.position = _targetTo.transform.position; Debug.Log("Автомобіль торкнувся триггера, телепортуємо!");
                    _carRigidbody.WakeUp();
                    break;

                case EffectType.Boost:

                    _carRigidbody.AddForce(_car.transform.forward * _bosst, ForceMode.Impulse);

                    Debug.Log("Сила применена к автомобилю!");
                    break;

                case EffectType.Slow:

                    _carRigidbody.AddForce(-_car.transform.forward * _slow, ForceMode.Impulse);

                    Debug.Log("Сила применена к автомобилю!");
                    break;

                case EffectType.Jump:

                    _carRigidbody.AddForce(_car.transform.up * _jump, ForceMode.Impulse);
                    Debug.Log("Автомобиль подпрыгнул!");
                    break;
                case EffectType.Oil:

                    StartCoroutine(OilTime());
                    Debug.Log("Автомобиль скользит по маслу!");
                    break;


            }

            if (random)
            {
                ifRandom = false;
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
                stForvord.stiffness = _oil;
                wheel._wheelCollider.forwardFriction = stForvord;
            }
            else
            {
                WheelFrictionCurve st = wheel._wheelCollider.forwardFriction;
                startValueRoad = st;
                st.stiffness = _oil;
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
