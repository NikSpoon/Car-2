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
    [SerializeField] private int startTime = 5;

    public event Action<int, bool> OnWaitForStart;

    private List<CarControler> allSpawnedCars = new();
    public bool start = false;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitRotine());
      
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
                
                Debug.Log($"✅ Привязали машину к игроку {profile.playerName}");
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

                    Debug.Log($"[Server] Назначено управление машиной:{car}" +
          $"Игрок: {profile.playerName}\n" +
          $"NetID машины: {carIdentity.netId}\n" +
          $"AssetID машины: {carIdentity.assetId}\n" +
          $"Connection ID: {conn.connectionId}\n" +
          $"isOwned: {carIdentity.isOwned}\n" +
          $"isServer: {carIdentity.isServer}\n" +
          $"isClient: {carIdentity.isClient}");


                    // Получаем PlayerFollowCar из профиля игрока (например, компонент камеры)
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

        Debug.Log("Все игроки готовы! Запускаем игру.");

        StartRace();
    }

    private void StartRace()
    {
        // Основной код старта
        Debug.Log("Стартуем гонку!");
       
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
        // Серверная часть (регистрация, добавление в список)
        var controller = carObj.GetComponent<CarControler>();
        RaceManager.Instance.RegisterRaceCar(playerName, carObj);
        allSpawnedCars.Add(controller);

        // Вызов RPC, чтобы настроить машину и на клиенте
        var carIdentity = carObj.GetComponent<NetworkIdentity>();
        RpcSetupCar(carIdentity, isBot);
    }
    [ClientRpc]
    private void RpcSetupCar(NetworkIdentity carIdentity, bool isBot)
    {
        var carObj = carIdentity.gameObject;

        var rb = carObj.GetComponent<Rigidbody>();
        var noCol = carObj.GetComponent<NoCollision>();
        var controller = carObj.GetComponent<CarControler>();

        if (rb != null) rb.isKinematic = true;
        noCol?.EnablePassiveGhost(999f);

        if (controller != null)
        {
            controller.IsPlayerControl = !isBot;
            controller.IsEnamyControl = isBot;
        }

        carObj.tag = isBot ? "Enemy" : "Player";
    }

    private IEnumerator ServerStartRaceRoutine()
    {

        yield return CountdownBeforeStart();

        if (isServer)
        {
            RpcStartRace();
        }
       
    }
    private IEnumerator InitRotine()
    {
        while (!NetworkServer.active)
            yield return null;

        Debug.Log("✅ Server is active — waiting for players ready");

        yield return WaitForAllPlayersReady();

        Debug.Log("✅ Все игроки готовы — спавним машины");

        SpawnMiror();
        
        yield return ServerStartRaceRoutine();


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
    private void OnDisable()
    {

        Debug.Log("🚫 CarSpawner disabled!");
    }
    private void Update()
    {
        
    }
}
