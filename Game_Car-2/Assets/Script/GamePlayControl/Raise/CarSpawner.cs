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
    //  [SerializeField] private int enemyValue = 5;
    [SerializeField] private int startTime = 10;

    public event Action<int, bool> OnWaitForStart;

    private List<CarControler> allSpawnedCars = new();
    public bool start = false;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitRotine());
        start = false;
    }


    [Server]
    private void SpawnMiror()
    {

        // Берём всех игроков из сессии — и игроков с подключением, и ботов
        var session = FindFirstObjectByType<NetworkGameSession>();
        if (session == null)
        {
            Debug.LogError("NetworkGameSession не найден!");
            return;
        }
        foreach (var profile in session.syncedPlayers)
        {
            if (profile == null) continue;

            var bot = profile.isBot;

            var conn = profile.connectionToClient;

            if (conn != null)
            {

                //  Debug.Log($"✅ Привязали машину к игроку {profile.playerName}");
            }

            if (!bot)
            {
                var car = SpawnPlayer(profile);


                if (conn != null && conn.isReady)
                {
                    NetworkServer.Spawn(car, conn as NetworkConnectionToClient);

                    var carIdentity = car.GetComponent<NetworkIdentity>();
                    profile.carIdentity = carIdentity;

                    SetupCar(car, profile.playerName, false);


                    var followCar = profile.gameObject.GetComponentInChildren<PlayerFollowCar>();
                    if (followCar != null)
                    {
                        Transform bodyTransform = SetRootForMirrir(car);
                        followCar.FindRoot(bodyTransform);
                    }
                }
                else
                {
                    Debug.LogWarning($"Нет подключения для игрока {profile.playerName}, спавним без владельца");
                    NetworkServer.Spawn(car);
                }

                SetupCar(car, profile.playerName, false);

            }
            else
            {
                var botCar = SpawnEnemiesIfHost(profile);

                NetworkServer.Spawn(botCar); // Спавн без владельца

                SetupCar(botCar, profile.playerName, true);
            }
            Debug.Log($"[SpawnMiror] Игрок: {profile.playerName}, Conn: {(conn != null ? conn.connectionId.ToString() : "null")}, IsBot: {bot}");

        }
    }
    private IEnumerator WaitForAllPlayersReady()
    {
        var session = FindFirstObjectByType<NetworkGameSession>();
        if (session == null)
        {
            Debug.LogError("NetworkGameSession не найден!");
            yield break;
        }

        while (true)
        {
            bool allReady = true;

            foreach (var profile in session.syncedPlayers)
            {
                if (profile == null) continue;

                var conn = profile.connectionToClient;


                if (!profile.isBot)
                {
                    if (conn != null)
                    {
                        if (!conn.isReady)
                        {
                            allReady = false;
                            break;
                        }
                    }
                    else
                    {
                        if (!(NetworkServer.active && NetworkClient.activeHost))
                        {

                            allReady = false;
                            break;
                        }

                    }
                }
                else
                {
                    // Для ботов можно считать их всегда готовыми

                }
            }

            if (allReady)
                break;

            yield return new WaitForSeconds(1f);
        }

        //  Debug.Log("Все игроки готовы! Запускаем игру.");

    }


    private GameObject SpawnPlayer(NetworkPlayerProfile profile)
    {
        var carPrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex].upgradePrefab;

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
        var carPrefab = enemyCarDatabase.carUpgrades[carIndex].upgrades[carUpgrades].upgradePrefab;
        var botCar = Instantiate(carPrefab, _start.position, _start.rotation);

        return botCar;
    }

    private void SetupCar(GameObject carObj, string playerName, bool isBot)
    {
        var controller = carObj.GetComponent<CarControler>();
        RaceManager.Instance.RegisterRaceCar(playerName, carObj);
        allSpawnedCars.Add(controller);

        controller.RpcSetupCar(isBot);
    }
    private IEnumerator ServerStartRaceRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (isServer)
        {
            StartRace();
        }

    }
    private IEnumerator InitRotine()
    {
        while (!NetworkServer.active)
            yield return null;

        yield return WaitForAllPlayersReady();

        SpawnMiror();

        yield return WaitForAllPlayersInCars(); 

        yield return CountdownBeforeStart();

        yield return ServerStartRaceRoutine();


    }
    private IEnumerator WaitForAllPlayersInCars()
    {
        var session = FindFirstObjectByType<NetworkGameSession>();
        if (session == null)
        {
            yield break;
        }

        while (true)
        {
            bool allInCars = true;

            foreach (var profile in session.syncedPlayers)
            {
                if (profile == null) continue;

                // Игнорируем ботов — считаем, что они уже "в машине"
                if (profile.isBot)
                    continue;

                // Проверка: есть ли у игрока машина?
                if (profile.carIdentity == null || profile.carIdentity.gameObject == null)
                {
                    allInCars = false;
                    break;
                }

                // Опционально: Проверка, стоит ли машина на старте
                var carPos = profile.carIdentity.transform.position;
                var distanceToStart = Vector3.Distance(carPos, _start.position);
                if (distanceToStart > 2f) // допуск ±2 метра
                {
                    allInCars = false;
                    break;
                }
            }

            if (allInCars)
                break;

            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("✅ Все игроки находятся в своих машинах на старте");
    }
    private IEnumerator CountdownBeforeStart()
    {
        bool ghost = true;
        var session = FindFirstObjectByType<NetworkGameSession>();

        for (int i = startTime; i > 0; i--)
        {
            foreach (var profile in session.syncedPlayers)
            {
                if (!profile.isBot && profile.connectionToClient != null)
                {
                    TargetUpdateCountdown(profile.connectionToClient, i, ghost);
                }
            }

            yield return new WaitForSeconds(1f);
        }

       
        foreach (var profile in session.syncedPlayers)
        {
            if (!profile.isBot && profile.connectionToClient != null)
            {
                TargetUpdateCountdown(profile.connectionToClient, 0, false);
            }
        }
    }
    [Server]
    private void StartRace()
    {

        foreach (var car in allSpawnedCars)
        {
            if (car != null)
            {
                car.RpcStartCar();
            }
        }
        start = true;

    }

    private Transform SetRootForMirrir(GameObject carObj)
    {
        foreach (Transform child in carObj.transform)
        {
            if (child.CompareTag("Body"))
            {
                // Нашли объект с тегом "Body"
                // Debug.Log("Найден объект с тегом Body: " + child.name);
                // Можно вернуть child или что-то сделать
                return child;
            }

        }
        return null;
    }
    
    [TargetRpc]
    private void TargetUpdateCountdown(NetworkConnection conn, int time, bool ghost)
    {
        OnWaitForStart?.Invoke(time, ghost);
    }
    private void OnDisable()
    {

        Debug.Log("🚫 CarSpawner disabled!");
    }
    
}
