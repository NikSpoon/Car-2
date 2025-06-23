using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Runtime.ConstrainedExecution;

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
        StartCoroutine(InitRotine());
    }

    private void Start()
    {
        if (NetworkServer.active) // Хост (сервер)
        {
            StartCoroutine(InitMirorRotine());
            SpawnMiror();
            ServerStartRaceRoutine();
        }
        else if (!isMultiplayer) // Сингл
        {
            SpawnSingle();
            SinglePlayerStartRoutine();
        }
    }
    #region SPAWN LOGIC

    private void SpawnSingle()
    {
        var profile = PlayerDataManager.Instance.PlayerProfile;
        SpawnSinglePlayer(profile);
        SpawnSinglePlayerEnemies(enemyValue, profile);
    }
    private void SpawnSinglePlayer(PlayerProfile profile)
    {
        var carPrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex];

        var car = Instantiate(carPrefab, _start.position, _start.rotation);
        SetupCar(car, profile.playerName, false);
    }

    private void SpawnSinglePlayerEnemies(int count, PlayerProfile profile)
    {
        BotCreator botCreator = new BotCreator();
        for (int i = 0; i < count; i++)
        {
            int carIndex = UnityEngine.Random.Range(0, enemyCarDatabase.carUpgrades.Count);
            var upgrade = enemyCarDatabase.carUpgrades[carIndex].upgrades[profile.selectedBodyUpgradeIndex];
            var botCar = Instantiate(upgrade, _start.position, _start.rotation);

            AIProfile aiProfile = botCreator.CreateUniqueBot();
            string botName = aiProfile.botName;

            SetupCar(botCar, botName, true);
        }
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

    #endregion

    #region START RACE

    private IEnumerator ServerStartRaceRoutine()
    {
        // Ждём подключение всех + буфер
        yield return new WaitForSeconds(5f); // Можно сделать динамическим

        yield return CountdownBeforeStart();

        RpcStartRace(); // Запускаем гонку на всех
    }
    private IEnumerator InitRotine()
    {
        yield return new WaitForSeconds(3f);
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

    #endregion
}
