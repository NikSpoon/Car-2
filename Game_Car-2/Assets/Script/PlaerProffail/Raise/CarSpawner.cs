using System;
using System.Collections;

using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    private GameObject car;
    private NoCollision carNoCollision;
    public CarDatabase carDatabase;
    private Rigidbody carRb;

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

    }
    private IEnumerator HandleStartSequence()
    {
        yield return StartCoroutine(WaitForOther()); 
        yield return StartCoroutine(StartRaise());

        carRb.isKinematic = false;
        carNoCollision.Respawn();
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

}