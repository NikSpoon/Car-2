

using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [SerializeField] private CarSpawner _carSpawner;
    private List<Transform> _checkPoints = new();
    private RaiseChekpoint _raiseCheckpoint;

    public List<Transform> Checkpoints => _checkPoints;

    private List<CarStatus> _carStatuses = new();
    public IReadOnlyList<CarStatus> CarStatuses => _carStatuses;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _raiseCheckpoint = FindAnyObjectByType<RaiseChekpoint>();
        if (_raiseCheckpoint != null)
        {
            _checkPoints = _raiseCheckpoint.GetСhekPoint();
        }
        else
        {
            Debug.LogError("RaiseChekpoint не найден!");
        }
    }

    public void RegisterCar(GameObject car, float initialHP, bool isEnemy)
    {
        _carStatuses.Add(new CarStatus(car, initialHP, isEnemy));
    }

    public void UpdateCarHP(GameObject car, float newHP)
    {
        var status = _carStatuses.Find(c => c.CarObject == car);
        if (status != null)
        {
            status.CurrentHP = newHP;
        }
    }

    public CarStatus GetNearestEnemy(Vector3 position, float maxDistance)
    {
        CarStatus closest = null;
        float minDist = float.MaxValue;

        foreach (var car in _carStatuses)
        {
            if (!car.IsEnemy) continue;

            float dist = Vector3.Distance(position, car.Position);
            if (dist < maxDistance && dist < minDist)
            {
                minDist = dist;
                closest = car;
            }
        }

        return closest;
    }
}
public class CarStatus
{
    public GameObject CarObject { get; }
    public float CurrentHP { get; set; }
    public Vector3 Position => CarObject.transform.position;
    public bool IsEnemy { get; }

    public CarStatus(GameObject carObject, float initialHP, bool isEnemy)
    {
        CarObject = carObject;
        CurrentHP = initialHP;
        IsEnemy = isEnemy;
    }
}
