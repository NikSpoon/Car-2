

using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    [SerializeField] private CarSpawner _carSpawner;
    private List<Transform> _checkPoints = new();
    private RaiseChekpoint _raiseCheckpoint;

    public List<Transform> Checkpoints => _checkPoints;

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

}