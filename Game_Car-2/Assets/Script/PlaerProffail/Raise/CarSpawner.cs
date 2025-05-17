using NUnit.Framework;
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


    private CarControler carControler;

    [SerializeField] private int enemyValue = 5;
    [SerializeField] private Transform _start;
    [SerializeField] private int startTime = 10;

    private bool IsGhostStartActive = false;
    public event Action<int, bool> OnWaitForStart;
    private void Awake()
    {
            
        var profile = PlayerDataManager.Instance.playerProfile;

        GameObject carPrefab = carDatabase.carPrefabs[profile.selectedCarIndex];
        GameObject upgradePrefab = carDatabase.carUpgrades[profile.selectedCarIndex].upgrades[profile.selectedBodyUpgradeIndex];

        car = Instantiate(upgradePrefab, _start.position, _start.rotation);
        carRb = car.GetComponent<Rigidbody>();
        carRb.isKinematic = true; 
        carNoCollision = car.GetComponent<NoCollision>();
           
    }
    private void Start()
    {
        carNoCollision.EnablePassiveGhost(999f);
        StartCoroutine(HandleStartSequence());
        SpawnEnemy(enemyValue);
    }
    private IEnumerator HandleStartSequence()
    {
        yield return StartCoroutine(WaitForOther()); 
        yield return StartCoroutine(StartRaise());

        carRb.isKinematic = false;
       
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
        var profile = PlayerDataManager.Instance.playerProfile;
        var enemyUpdateIndex = profile.selectedBodyUpgradeIndex;
        var enemyCarIndex = UnityEngine.Random.Range(0, enemyCarDatabase.carPrefabs.Count);

        GameObject enemyUpgradePrefab = enemyCarDatabase.carUpgrades[enemyCarIndex].upgrades[enemyUpdateIndex];

        for (int i = 0; i < value; i++)
        {
            Vector3 spawnPosition = _start.position + new Vector3(i * 2f, 0, 0); // чтобы не налазили
            var enemyCar = Instantiate(enemyUpgradePrefab, spawnPosition, _start.rotation);
            enemyCar.tag = "Enemy";

            var carController = enemyCar.GetComponent<CarControler>();
            if (carController.IsPlayerControl)
            {
                carController.IsPlayerControl = false;
                carController.IsEnamyControl = true;
            }
            enemyControllers.Add(carController);

            var noCollision = enemyCar.GetComponent<NoCollision>();
            var rb = enemyCar.GetComponent<Rigidbody>();

            if (noCollision != null && rb != null)
            {
                noCollision.EnablePassiveGhost(999f);
                rb.isKinematic = true;
                // Для врагов можно не запускать StartRaise — оно для игрока
                // rb.isKinematic = false; можно включить в другом месте, например при старте гонки
            }
        }
    }

}