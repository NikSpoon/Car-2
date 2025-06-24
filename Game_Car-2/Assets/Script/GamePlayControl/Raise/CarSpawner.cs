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


    private void Awake()
    {
        StartCoroutine(InitRotine());
    }

    private void Start()
    {
        StartCoroutine(InitMirorRotine());
        SpawnMiror();
        StartCoroutine(ServerStartRaceRoutine());
    }

    private void SpawnMiror()
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null) continue;

            var profile = conn.identity.GetComponent<NetworkPlayerProfile>();
            if (profile == null) continue;

            var bot = profile.isBot;

            if (!bot)
            {
                var car = SpawnPlayer(profile);

                NetworkServer.Spawn(car, conn);

                SetupCar(car, profile.playerName, false);
            }
            else
            {
                var botCar = SpawnEnemiesIfHost(profile);

                NetworkServer.Spawn(botCar); // Спавн без владельца

                SetupCar(botCar, profile.playerName, true);
            }
        }
    }

    private GameObject SpawnPlayer(NetworkPlayerProfile profile)
    {
        var carPrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex];

        var car = Instantiate(carPrefab, _start.position, _start.rotation);
        return car;
    }

    private GameObject SpawnEnemiesIfHost(NetworkPlayerProfile profile)
    {
        int carIndex = 0, carUpgrades = 0;
        if (profile.isBotRandom)
        {
            if (NetworkServer.active)
            {
                var hostProfile = PlayerDataManager.Instance.PlayerProfile;
                carIndex = hostProfile.selectedCarIndex;
                carUpgrades = UnityEngine.Random.Range(0, enemyCarDatabase.carUpgrades.Count);
            }
        }
        else
        {
            carUpgrades = profile.selectedBodyUpgradeIndex;
            carIndex = profile.selectedCarIndex;
        }
        var carPrefab = carDatabase.carUpgrades[carIndex].upgrades[carUpgrades];
        var botCar = Instantiate(carPrefab, _start.position, _start.rotation);

        return botCar;
    }

    private void SetupCar(GameObject carObj, string playerName, bool isBot)
    {
        var rb = carObj.GetComponent<Rigidbody>();
        var noCol = carObj.GetComponent<NoCollision>();
        var controller = carObj.GetComponent<CarControler>();

        if (rb == null) Debug.LogError("SetupCar: Rigidbody is missing!");
        if (noCol == null) Debug.LogWarning("SetupCar: NoCollision is missing!");
        if (controller == null) Debug.LogError("SetupCar: CarControler is missing!");

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


    private IEnumerator ServerStartRaceRoutine()
    {
        // Ждём подключение всех + буфер
        yield return new WaitForSeconds(5f); // Можно сделать динамическим

        yield return CountdownBeforeStart();

        RpcStartRace(); // Запускаем гонку на всех
    }
    private IEnumerator InitRotine()
    {
        yield return new WaitUntil(() => NetworkServer.active && NetworkClient.ready);
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
    private IEnumerator InitMirorRotine()
    {
        while (NetworkClient.ready)
        {

            yield return new WaitForSeconds(1f);
        }
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
    private void OnDisable()
    {

        Debug.Log("Dosable");
    }

}
