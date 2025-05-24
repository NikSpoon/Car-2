
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.FSM.EnemyCar
{

    public abstract class BaseAIController : MonoBehaviour
    {

        public NavMeshAgent agent;
        public StateMashine<State, object> _stateMashine;
        public CarSpawner carSpawner;
        public List<Transform> Checkpoints =>
           RaceManager.Instance != null && RaceManager.Instance.Checkpoints != null
               ? RaceManager.Instance.Checkpoints
               : new List<Transform>();
        public TargetFinder targetFinder;

        public Transform AgroTarget { get; set; }
        public Transform Target { get; set; }
        public float VerticalInput { get; set; }
        public float HorizontalInput { get; set; }
        public bool Brake { get; set; }



        public abstract StateMashine<State, object> GetBehavior();
       
        public void Start()
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            carSpawner = Object.FindFirstObjectByType<CarSpawner>();
            StartCoroutine(InitAI());
        }

        private IEnumerator InitAI()
        {

            yield return new WaitUntil(() => RaceManager.Instance != null);

            _stateMashine = GetBehavior();

            if (_stateMashine != null && _stateMashine.CurrentState != null)
            {
                Debug.Log($"[FSM INIT] Current State: {_stateMashine.CurrentState.GetType().Name}");
            }
            else
            {
                Debug.LogError("[FSM INIT] StateMachine or CurrentState is null!");
            }
        }

        public void Update()
        {
            if (_stateMashine == null || _stateMashine.CurrentState == null)
            {
                Debug.Log("[FSM] StateMachine или CurrentState не инициализированы");
                return;
            }

            string stateName = _stateMashine.CurrentState?.Name ?? "null";
            string targetName = Target != null ? Target.name : "null";
            Debug.Log($"[FSM] Current State: {stateName}, Target: {targetName}");

            if (_stateMashine == null)
                return;

            if (targetFinder != null)
            {
                AgroTarget = targetFinder.CurrentTarget;
            }

            var (vertical, horizontal, brake) = _stateMashine.CurrentState.Execute();

            VerticalInput = vertical;
            HorizontalInput = horizontal;
            Brake = brake;

            var nextState = _stateMashine.CurrentState.TryGetNexState();
            if (nextState != null)
            {
                _stateMashine.CurrentState = nextState;
            }

        }
    

    }
}

