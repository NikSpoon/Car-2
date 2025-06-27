
using Mirror;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class RaiseChekpoint : MonoBehaviour
{
    [SerializeField] private Transform _checkpointsParent;

    private List<Transform> _chekpoints = new List<Transform>();
    private Transform _car;
    public Transform CurentPoint { get; private set; }
    public event Action<int, int> OnChekPointChenge;

    public void Start()
    {
        gameObject.SetActive(true);
        _chekpoints.Clear();


        if (_checkpointsParent == null)
        {
            Debug.LogError("Checkpoints parent is not assigned!");
            return;
        }

        foreach (Transform checkpoint in _checkpointsParent)
        {
            _chekpoints.Add(checkpoint);
        }



        // Получаем локального игрока (GameObject)
        var localPlayerGO = NetworkClient.localPlayer?.gameObject;

        if (localPlayerGO == null)
        {
            Debug.LogError("Локальный игрок ещё не инициализирован!");
            return;
        }

        foreach (var car in GameObject.FindGameObjectsWithTag("Player"))
        {
            NetworkIdentity identity = car.GetComponent<NetworkIdentity>();
            if (identity != null && identity.isLocalPlayer)
            {
                _car = car.transform;
                break;
            }
        }

        OnChekPointChenge?.Invoke(0, _chekpoints.Count - 1);

    }

    public void UpdateCheckpoint(Transform nextPoint)
    {
        var index = _chekpoints.IndexOf(nextPoint);

        if (index != -1)
        {
            CurentPoint = nextPoint;
            var rem = _chekpoints.Count - 1 - index;
            // Debug.Log($"Checkpoint updated to: {CurentPoint.name}");    
            OnChekPointChenge?.Invoke(index, rem);
        }
    }
    public List<Transform> GetСhekPoint()
    {
        var list = _chekpoints;

        return list;
    }
}
