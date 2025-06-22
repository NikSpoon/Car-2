using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CarSpawner : NetworkBehaviour
{
    [SerializeField] private Transform _start;
    [SerializeField] private CarDatabase carDatabase;
    [SerializeField] private CarDatabase enemyCarDatabase;
    [SerializeField] private int enemyValue = 5;
    [SerializeField] private int startTime = 5;

    public event Action<int, bool> OnWaitForStart;

    private List<CarControler> allSpawnedCars = new();
    public bool start = false;

    private bool isMultiplayer = false;

    private void Awake()
    {
        isMultiplayer = NetworkServer.active || NetworkClient.active;
    }

    private void OnEnable()
    {
        if (NetworkServer.active) // Хост (сервер)
        {
            SpawnAllPlayers();
            SpawnEnemiesIfHost();
            StartCoroutine(ServerStartRaceRoutine());
        }
        else if (!isMultiplayer) // Сингл
        {
            SpawnSinglePlayer();
            SpawnSinglePlayerEnemies(enemyValue);
            StartCoroutine(SinglePlayerStartRoutine());
        }
    }

    #region SPAWN LOGIC

    private void SpawnSinglePlayer()
    {
        var profile = PlayerDataManager.Instance.PlayerProfile;
        var carPrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex];

        var car = Instantiate(carPrefab, _start.position, _start.rotation);
        SetupCar(car, profile.playerName, false);
    }

    private void SpawnSinglePlayerEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int carIndex = UnityEngine.Random.Range(0, enemyCarDatabase.carUpgrades.Count);
            var upgrade = enemyCarDatabase.carUpgrades[carIndex].upgrades[0];
            var botCar = Instantiate(upgrade, _start.position, _start.rotation);

            string botName = $"Bot_{i}";
            SetupCar(botCar, botName, true);
        }
    }

    private void SpawnAllPlayers()
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            var profile = conn.identity.GetComponent<NetworkPlayerProfile>();
            var carPrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex];

            var car = Instantiate(carPrefab, _start.position, _start.rotation);
            NetworkServer.Spawn(car, conn);

            SetupCar(car, profile.playerName, false);
        }
    }

    private void SpawnEnemiesIfHost()
    {
        for (int i = 0; i < enemyValue; i++)
        {
            int carIndex = UnityEngine.Random.Range(0, enemyCarDatabase.carUpgrades.Count);
            var upgrade = enemyCarDatabase.carUpgrades[carIndex].upgrades[0];
            var botCar = Instantiate(upgrade, _start.position, _start.rotation);

            NetworkServer.Spawn(botCar); // Спавн без владельца
            string botName = $"Bot_{i}";
            SetupCar(botCar, botName, true);
        }
    }

    private void SetupCar(GameObject carObj, string playerName, bool isBot)
    {
        var rb = carObj.GetComponent<Rigidbody>();
        var noCol = carObj.GetComponent<NoCollision>();
        var controller = carObj.GetComponent<CarControler>();

        rb.isKinematic = true;
        noCol?.EnablePassiveGhost(999f);

        if (controller != null)
        {
            controller.IsPlayerControl = !isBot;
            controller.IsEnamyControl = isBot;
        }

        carObj.tag = isBot ? "Enemy" : "Player";

        RaceManager.Instance.RegisterRaceCar(playerName, carObj);
        allSpawnedCars.Add(controller);
    }

    #endregion

    #region START RACE

    private IEnumerator ServerStartRaceRoutine()
    {
        // Ждём подключение всех + буфер
        yield return new WaitForSeconds(5f); // Можно сделать динамическим

        yield return CountdownBeforeStart();

        RpcStartRace(); // Запускаем гонку на всех
    }

    private IEnumerator SinglePlayerStartRoutine()
    {
        yield return new WaitForSeconds(3f);
        yield return CountdownBeforeStart();
        EnableAllCars();
        start = true;
    }

    private IEnumerator CountdownBeforeStart()
    {
        bool ghost = true;
        for (int i = startTime; i > 0; i--)
        {
            OnWaitForStart?.Invoke(i, ghost);
            yield return new WaitForSeconds(1f);
        }
        OnWaitForStart?.Invoke(0, false);
    }

    [ClientRpc]
    private void RpcStartRace()
    {
        EnableAllCars();
        start = true;
    }

    private void EnableAllCars()
    {
        foreach (var car in allSpawnedCars)
        {
            if (car == null) continue;

            var rb = car.GetComponent<Rigidbody>();
            var noCol = car.GetComponent<NoCollision>();

            noCol?.Respawn();
            rb.isKinematic = false;
        }
    }

    #endregion
}
