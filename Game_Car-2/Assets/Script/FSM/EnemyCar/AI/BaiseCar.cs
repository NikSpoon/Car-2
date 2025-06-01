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
    private static int botCounter = 0;


    [SerializeField] private int _agroTime = 5;      
    [SerializeField] private int _agroCooldown = 30;
  
    [HideInInspector] public bool HasCollidedWithEnemy = false;
    public int AgroTime => _agroTime;
    public int AgroCooldownTime => _agroCooldown;

    public event Action<int, int> OnCooldownAgro;    

    private static Color[] botColors = {
    Color.cyan, Color.magenta, Color.green, Color.blue, Color.yellow, Color.white,
    new Color(1f, 0.5f, 0f), // orange
    new Color(0.5f, 0f, 1f), // purple
    new Color(0f, 1f, 0.5f), // teal
    new Color(1f, 0f, 0.5f), // pink
};

    private Color uniqueColor;

    public bool AllCheckpointsPassed()
    {
        return currentCheckpointIndex >= ChekpointEnemy.Count;
    }
    public override void Start()
    {
        base.Start();

        uniqueColor = botColors[botCounter % botColors.Length];
        botCounter++;

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

        if (collision.gameObject.layer == LayerMask.NameToLayer("Car"))
        {
            HasCollidedWithEnemy = true;
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
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // 1. Путь агента
        if (agent.path != null && agent.path.corners.Length > 1)
        {
            Gizmos.color = Color.black;
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
                Gizmos.DrawSphere(corners[i], 0.15f);
            }
            Gizmos.DrawSphere(corners[corners.Length - 1], 0.15f);
        }

        // 2. Отрисовка текущей Target (например, чекпоинт)
        if (Target != null)
        {
            Gizmos.color = uniqueColor;
            Gizmos.DrawLine(transform.position, Target.position);
            Gizmos.DrawSphere(Target.position, 0.4f);
        }

        // 3. Отрисовка текущей агро-цели
        if (AgroTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, AgroTarget.position);
            Gizmos.DrawWireSphere(AgroTarget.position, 0.4f);
        }

        // 4. Steering target
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(agent.steeringTarget, 0.2f);
    }
    public void InvokeAgroCooldownEvent(int agroTime, int cooldownLeft)
    {
        OnCooldownAgro?.Invoke(agroTime, cooldownLeft);
    }
}
