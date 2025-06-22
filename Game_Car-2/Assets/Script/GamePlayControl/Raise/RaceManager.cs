

using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance { get; private set; }

    private Dictionary<string, RaceCarData> _raceCars = new();

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


    public void RegisterRaceCar(string playerName, GameObject carPrefab)
    {
        if (!_raceCars.ContainsKey(playerName))
        {
            _raceCars.Add(playerName, new RaceCarData(carPrefab));
        }
        else
        {
            Debug.LogWarning($"Машина с именем {playerName} уже зарегистрирована.");
        }
    }

    public RaceCarData GetRaceCarData(string playerName)
    {
        _raceCars.TryGetValue(playerName, out var data);
        return data;
    }

    public Dictionary<string, RaceCarData> GetAllRaceCars()
    {
        return _raceCars;
    }

    public void ClearRaceCars()
    {
        _raceCars.Clear();
    }
    public void SetPlayerStats()
    {

    }
}
