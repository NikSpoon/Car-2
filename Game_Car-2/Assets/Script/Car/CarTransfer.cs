using NUnit.Framework;
using System;
using UnityEngine;

public class CarTransfer : MonoBehaviour
{
    [SerializeField] private Wheel[] _wheels;
    [SerializeField] private CarControler _carControler;
    [SerializeField] private int _numberOfGears = 5;

    private Array Transfer;


    private void Start()
    {
        Transfer = new Array[_numberOfGears];
    }

    private void FirstGearsValue()
    {
        foreach (var wheel in _wheels)
        {
            if (wheel.IsForward)
            {

            }
            else
            {

            }

        }

    }
    private void SecondGearsValue()
    {

    }
    private void ThedGearsValue()
    {

    }
    private void FoursGearsValue()
    {

    }
    private void FiveGearsValue()
    {

    }

}