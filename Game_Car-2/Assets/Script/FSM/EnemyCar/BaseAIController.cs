
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
           RaceManager.Instance != null && RaceManager.Instance.Checkpoints != null ? RaceManager.Instance.Checkpoints :
            new List<Transform>();

        // Зоны обнаружения препятствий
        public float slowDownRadius = 6f;
        public float brakeRadius = 2.5f;
        public LayerMask obstacleLayer;

        public TargetFinder targetFinder;

        public Transform AgroTarget { get; set; }
        public Transform Target { get; set; }
        public float VerticalInput { get; set; }
        public float HorizontalInput { get; set; }
        public bool Brake { get; set; }

        public Vector3 Dierction { get; set; }

        public abstract StateMashine<State, object> GetBehavior();

     
        public virtual void Start()
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
  
        }
        private void ChangeState(State newState)
        {
            if (_stateMashine.CurrentState != null)
                _stateMashine.CurrentState.OnExit();

            _stateMashine.CurrentState = newState;

            if (_stateMashine.CurrentState != null)
                _stateMashine.CurrentState.OnEnter();
        }
        public IEnumerator InitAI()
        {
            yield return new WaitUntil(() => RaceManager.Instance != null);


            yield return new WaitUntil(() => RaceManager.Instance.Checkpoints != null && RaceManager.Instance.Checkpoints.Count > 0);

            carSpawner = Object.FindFirstObjectByType<CarSpawner>();
            _stateMashine = GetBehavior();
           
           // Debug.Log($"[InitAI] Total checkpoints: {Checkpoints.Count}");
            for (int i = 0; i < Checkpoints.Count; i++)
            {
               // Debug.Log($"[InitAI] Checkpoint {i}: {Checkpoints[i].name}");
            }


            if (_stateMashine != null && _stateMashine.CurrentState != null)
            {
                //Debug.Log($"[FSM INIT] Current State: {_stateMashine.CurrentState.GetType().Name}");
            }
            else
            {
               // Debug.LogError("[FSM INIT] StateMachine or CurrentState is null!");
            }



        }

        public virtual void Update()
        {
            if (_stateMashine == null || _stateMashine.CurrentState == null)
            {
               // Debug.Log("[FSM] StateMachine или CurrentState не инициализированы");
                return;
            }

            string stateName = _stateMashine.CurrentState?.Name ?? "null";
            string targetName = Target != null ? Target.name : "null";
         // Debug.Log($"[FSM] Current State: {stateName}");

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
                ChangeState(nextState);
            }

            Dierction = agent.destination;


        }
        public void FixedUpdate()
        {
            agent.nextPosition = transform.position;
        }
     

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, slowDownRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, brakeRadius);
        }

        private void OnDrawGizmos()
        {

            // Цвет направления
            Gizmos.color = Color.green;

            // Вектор от позиции к steeringTarget
            Vector3 direction = agent.steeringTarget - transform.position;
            Gizmos.DrawLine(transform.position, agent.steeringTarget);

            // Отметим точку назначения
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(agent.steeringTarget, 0.3f);
           
            Gizmos.color = Color.grey;
            if(Target !=null)
            Gizmos.DrawSphere(Target.position, 0.6f);
            // Показываем путь, если есть
            if (agent.path != null && agent.path.corners.Length > 1)
            {
                Gizmos.color = Color.black;

                Vector3[] corners = agent.path.corners;
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[i + 1]);
                    Gizmos.DrawSphere(corners[i], 0.15f);
                }

                // Последняя точка
                Gizmos.DrawSphere(corners[corners.Length - 1], 0.15f);

            }
        }
    }


}

