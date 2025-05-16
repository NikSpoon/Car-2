
using System;
using System.Collections.Generic;
using UnityEngine;

public class RaiseChekpoint : MonoBehaviour
{
    [SerializeField] private Transform _firstPoint;
    [SerializeField] private Transform _Finish;

    private List<Transform> _chekpoints = new List<Transform>();
    private Transform _car;
    public Transform CurentPoint { get;  private set; }
    public event Action<int,int> OnChekPointChenge;

    public void Awake()
    {
        _chekpoints.Clear();

        CurentPoint = _firstPoint.transform;
        var chArray = GameObject.FindGameObjectsWithTag("Chekpoint");

        Array.Sort(chArray, (a, b) => string.Compare(a.name, b.name));

        _chekpoints.Add(_firstPoint);
        foreach (var chek in chArray)
        {
            var transform = chek.GetComponent<Transform>();
            _chekpoints.Add(transform);
        }
        _chekpoints.Add(_Finish);


        var car = GameObject.FindGameObjectWithTag("Player");
        _car = car.transform;
        OnChekPointChenge?.Invoke(0, _chekpoints.Count - 1);


    }

    public void UpdateCheckpoint(Transform nextPoint)
    {
        var index = _chekpoints.IndexOf(nextPoint);

        if (index!= -1)
        {
            CurentPoint = nextPoint;
            var rem = _chekpoints.Count - 1 - index;
            Debug.Log($"Checkpoint updated to: {CurentPoint.name}");
            OnChekPointChenge?.Invoke(index, rem);
        }
    }
   public List<Transform> GetСhekPoint()
    {
        var list = _chekpoints;

        return list;
    }
}
