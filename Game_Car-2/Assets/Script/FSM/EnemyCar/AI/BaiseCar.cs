using Assets.Script.FSM.EnemyCar.Actions;
using Assets.Script.FSM.EnemyCar.Condition;
using Assets.Script.FSM.EnemyCar.Conditions;
using Assets.Script.FSM.EnemyCar;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class BaiseCar : BaseAIController
{
    public AgroCooldownCondition AgroCooldown { get; private set; }
    private List<Transform> ChekpointEnemy;
    private int currentCheckpointIndex = 0;
    private Transform _respChek;

    public bool AllCheckpointsPassed()
    {
        return currentCheckpointIndex >= ChekpointEnemy.Count;
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

        Debug.Log($"[InitAI] Total checkpoints: {Checkpoints.Count}");
        for (int i = 0; i < Checkpoints.Count; i++)
        {
            Debug.Log($"[InitAI] Checkpoint {i}: {Checkpoints[i].name}");
        }


        if (_stateMashine != null && _stateMashine.CurrentState != null)
        {
            Debug.Log($"[FSM INIT] Current State: {_stateMashine.CurrentState.GetType().Name}");
        }
        else
        {
            Debug.LogError("[FSM INIT] StateMachine or CurrentState is null!");
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
            Debug.LogError("Checkpoints list is empty!");
            
        }

        if (Target != null)
        {
            agent.SetDestination(Target.position);
        }

        AgroCooldown = new AgroCooldownCondition(this);
       

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

        raceState.transitions.Add(new Transition<State, BaseCondition>(aggroState, new DetectClosestTargetCondition(this, AgroCooldown)));
        raceState.transitions.Add(new Transition<State, BaseCondition>(finishState, new ReachedFinishCondition(this)));

        aggroState.transitions.Add(new Transition<State, BaseCondition>(raceState, AgroCooldown));
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

    public void OnCheckpointReached(Transform checkpoint)
    {
        Debug.Log($"Checkpoint reached: {checkpoint.name}, next target: {(currentCheckpointIndex + 1 < ChekpointEnemy.Count ? ChekpointEnemy[currentCheckpointIndex + 1].name : "Finish")}");

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
}
