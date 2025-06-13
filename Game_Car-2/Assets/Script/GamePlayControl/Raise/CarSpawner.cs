
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    private GameObject car;
    private NoCollision carNoCollision;
    public CarDatabase carDatabase;
    public CarDatabase enemyCarDatabase;
    private Rigidbody carRb;

    public bool start = false;

    private CarControler carControler;

    [SerializeField] private int enemyValue = 5;
    [SerializeField] private Transform _start;
    [SerializeField] private int startTime = 10;

    private bool IsGhostStartActive = false;
    public event Action<int, bool> OnWaitForStart;
    private void Awake()
    {

        var profile = PlayerDataManager.Instance.PlayerProfile;

        GameObject carPrefab = carDatabase.carPrefabs[profile.selectedCarIndex];
        GameObject upgradePrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex];

        car = Instantiate(upgradePrefab, _start.position, _start.rotation);
        carRb = car.GetComponent<Rigidbody>();
        carRb.isKinematic = true;
        carNoCollision = car.GetComponent<NoCollision>();

        RaceManager.Instance.RegisterRaceCar(profile.playerName, car);
    }
    private void Start()
    {
        carNoCollision.EnablePassiveGhost(999f);
        SpawnEnemy(enemyValue);
        StartCoroutine(HandleStartSequence());
    }
    private IEnumerator HandleStartSequence()
    {
        yield return StartCoroutine(WaitForOther());
        yield return StartCoroutine(StartRaise());


        carNoCollision.Respawn();
        carRb.isKinematic = false;

        start = true;


    }
    private IEnumerator StartRaise()
    {
        IsGhostStartActive = true;

        for (int i = startTime; i > 0; i--)
        {
            OnWaitForStart?.Invoke(i, IsGhostStartActive);
            yield return new WaitForSeconds(1f);
        }

        OnWaitForStart?.Invoke(0, false);
        IsGhostStartActive = false;

    }
    private IEnumerator WaitForOther()
    {
        yield return new WaitForSeconds(10f);

    }
    private List<CarControler> enemyControllers = new List<CarControler>();

    private void SpawnEnemy(int value)
    {
        var profile = PlayerDataManager.Instance.PlayerProfile;
        var enemyUpdateIndex = profile.selectedBodyUpgradeIndex;
        var enemyCarIndex = UnityEngine.Random.Range(0, enemyCarDatabase.carPrefabs.Count);

        GameObject enemyUpgradePrefab = enemyCarDatabase.carUpgrades[enemyCarIndex].upgrades[enemyUpdateIndex];

        for (int i = 0; i < value; i++)
        {
            var enemyCar = Instantiate(enemyUpgradePrefab, _start.position, _start.rotation);
            var rb = enemyCar.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            enemyCar.tag = "Enemy";


            var noCollision = enemyCar.GetComponent<NoCollision>();
            var carController = enemyCar.GetComponent<CarControler>();

            var uniqueEnemyName = $"Enemy_{i}_{Guid.NewGuid()}";
            RaceManager.Instance.RegisterRaceCar(uniqueEnemyName, enemyCar);

            if (carController.IsPlayerControl)
            {
                carController.IsPlayerControl = false;
                carController.IsEnamyControl = true;
            }
            enemyControllers.Add(carController);


            if (noCollision != null && rb != null)
            {
                noCollision.EnablePassiveGhost(999f);
                StartCoroutine(WaitForStartThenEnablePhysics(rb, noCollision));

            }

        }
    }
    private IEnumerator WaitForStartThenEnablePhysics(Rigidbody rb, NoCollision noCollision)
    {
        // Ждем пока start не станет true
        yield return new WaitUntil(() => start == true);

        // Отключаем ghost, включаем физику
        noCollision.Respawn();
        rb.isKinematic = false;  // включаем физику
    }

}