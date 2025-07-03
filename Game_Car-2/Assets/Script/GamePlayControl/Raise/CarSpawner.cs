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
  //  [SerializeField] private int enemyValue = 5;
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
        StartCoroutine(ServerStartRaceRoutine());
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
            else
            {
                Debug.LogWarning($"❌ Нет соединения у {profile.playerName}");
               
            }

            if (!bot)
            {
                var car = SpawnPlayer(profile);

                if (conn != null)
                {
                    NetworkServer.Spawn(car, conn as NetworkConnectionToClient);

                    var carIdentity = car.GetComponent<NetworkIdentity>();
                    profile.carIdentity = carIdentity;

                    SetupCar(car, profile.playerName, false);

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
        var carPrefab = enemyCarDatabase.carUpgrades[carIndex].upgrades[carUpgrades];
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

        yield return CountdownBeforeStart();

        if (isServer)
        {
            RpcStartRace();
        }
        else
        {
            Debug.LogError("Попытка вызвать RpcStartRace с клиента!");
        }
    }
    private IEnumerator InitRotine()
    {
        while (!NetworkServer.active)
            yield return null;



        Debug.Log("✅ Server is active — starting Spawn");
        SpawnMiror();
        
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

       
    }
    private void Update()
    {
        
    }
}
