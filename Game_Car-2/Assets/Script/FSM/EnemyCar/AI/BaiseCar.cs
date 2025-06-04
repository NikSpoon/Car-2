using Assets.Script.FSM.EnemyCar.Actions;
using Assets.Script.FSM.EnemyCar.Condition;
using Assets.Script.FSM.EnemyCar.Conditions;
using Assets.Script.FSM.EnemyCar;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;


public class BaiseCar : BaseAIController
{
    private AgrtoTimerConditions agrtoTimerConditions;
    private List<Transform> ChekpointEnemy;
    private int currentCheckpointIndex = 0;
    private Transform _respChek;
  
    [SerializeField] private Respawn respawnComponent;
    private Vector3 lastPosition;
    private float standStillTimer = 0f;
    [SerializeField] private float standStillThreshold = 2f; // сколько секунд считать "стоянием"
    [SerializeField] private float minMoveDistance = 0.1f;  // минимальное расстояние, чтобы считать, что бот двинулся

    [SerializeField] private int _agroTime = 5;      
    [SerializeField] private int _agroCooldown = 30;
    public bool IsExecutingAgroAction { get; set; } = false;
    public int AgroTime => _agroTime;
    public int AgroCooldownTime => _agroCooldown;

    public event Action<int, int> OnCooldownAgro;    



    public bool AllCheckpointsPassed()
    {
        return currentCheckpointIndex >= ChekpointEnemy.Count;
    }
    public override void Update()
    {
        base.Update();
        if (carSpawner == null || !carSpawner.start)
        {
            return;
        }
        agrtoTimerConditions?.Evoluete();
        CheckIfStuckAndRespawn();

    }
    private void CheckIfStuckAndRespawn()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);

        if (distanceMoved < minMoveDistance)
        {
            standStillTimer += Time.deltaTime;
            if (standStillTimer >= standStillThreshold)
            {
                if (respawnComponent != null)
                {
                    respawnComponent.Resp();
                }
                standStillTimer = 0f;
            }
        }
        else
        {
            standStillTimer = 0f;
        }

        lastPosition = transform.position;
    }
    public override void Start()
    {
        base.Start();

       
        ChekpointEnemy = new List<Transform>(Checkpoints);

        // 2. Запускаем FSM (после того как чекпоинты скопированы)
        StartCoroutine(InitAI());
        StartCoroutine(InitFSM());
        _respChek = ChekpointEnemy[0];
       
    }
    private IEnumerator InitFSM()
    {
        yield return new WaitUntil(() => RaceManager.Instance != null);


        yield return new WaitUntil(() => RaceManager.Instance.Checkpoints != null && RaceManager.Instance.Checkpoints.Count > 0);

       // Debug.Log($"[InitAI] Total checkpoints: {Checkpoints.Count}");
        for (int i = 0; i < Checkpoints.Count; i++)
        {
         //   Debug.Log($"[InitAI] Checkpoint {i}: {Checkpoints[i].name}");
        }


        if (_stateMashine != null && _stateMashine.CurrentState != null)
        {
           // Debug.Log($"[FSM INIT] Current State: {_stateMashine.CurrentState.GetType().Name}");
        }
        else
        {
           // Debug.LogError("[FSM INIT] StateMachine or CurrentState is null!");
        }



    }

    public override StateMashine<State, object> GetBehavior()
    {
        if (ChekpointEnemy.Count > 0)
        {
            Target = ChekpointEnemy[currentCheckpointIndex];
            
        }
        else
        {
           // Debug.LogError("Checkpoints list is empty!");
            
        }

        

        agrtoTimerConditions = new AgrtoTimerConditions(this); 


        var startState = new State("StartState");
        var raceState = new State("RaceState");
        var aggroState = new State("AggroState");
        var finishState = new State("FinishState");

        var goToCheckpoint = new GoToTargetAction(this);
        var agroToEnemy = new AgroTargetAction(this);
        var burnout = new StartActions(this);

        // Состояния
        startState.actions.Add(burnout);
        
        raceState.actions.Add(goToCheckpoint);
        aggroState.actions.Add(agroToEnemy);
        finishState.actions.Add(burnout);


        
        // Переходы

        startState.transitions.Add(new Transition<State, BaseCondition>(raceState, new StartCondition(this)));

        raceState.transitions.Add(new Transition<State, BaseCondition>(aggroState, new EnerAggroCondition(this, agrtoTimerConditions)));
        raceState.transitions.Add(new Transition<State, BaseCondition>(finishState, new ReachedFinishCondition(this)));

        aggroState.transitions.Add( new Transition<State, BaseCondition>(raceState, new ExitAggroCondition(this, agrtoTimerConditions)));
        aggroState.transitions.Add(new Transition<State, BaseCondition>(finishState, new ReachedFinishCondition(this)));

        return new StateMashine<State, object>(startState);
    }

    private void OnTriggerEnter(Collider other)
    {  
        Transform checkpointTransform = other.transform;

        int index = ChekpointEnemy.IndexOf(checkpointTransform);
        if (index == -1)
        {
            
            return;
        }

        if (index == currentCheckpointIndex)
        {
            OnCheckpointReached(checkpointTransform);
        }
        else
        {
          //  Debug.Log($"Пройден чекпоинт {checkpointTransform.name}, но мы ждём {ChekpointEnemy[currentCheckpointIndex].name}");
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
       
        if (collision.transform.root == transform.root)
            return;

        if (AgroTarget != null)
        {
            // Получаем все коллайдеры у AgroTarget
            var targetColliders = AgroTarget.GetComponentsInChildren<Collider>();

            foreach (var col in targetColliders)
            {
                if (collision.collider == col)
                {
                    agrtoTimerConditions._collision = true;
                 
                    return;
                }
            }
        }
    }
    public void OnCheckpointReached(Transform checkpoint)
    {
        // Debug.Log($"Checkpoint reached: {checkpoint.name}, next target: {(currentCheckpointIndex + 1 < ChekpointEnemy.Count ? ChekpointEnemy[currentCheckpointIndex + 1].name : "Finish")}");

        currentCheckpointIndex++;
        if (currentCheckpointIndex >= ChekpointEnemy.Count)
        {
            Debug.Log("Все чекпоинты пройдены!");
           
        }
        else
        {
            Target = ChekpointEnemy[currentCheckpointIndex];
            _respChek = ChekpointEnemy[currentCheckpointIndex - 1 ];
            agent.SetDestination(Target.position);
          
        }
        
    }
   
    public void InvokeAgroCooldownEvent(int agroTime, int cooldownLeft)
    {
        OnCooldownAgro?.Invoke(agroTime, cooldownLeft);
    }
}
