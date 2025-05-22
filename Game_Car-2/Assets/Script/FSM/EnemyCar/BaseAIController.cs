using Assets.Script.FSM.EnemyCar.Condition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.FSM.EnemyCar
{

    public abstract class BaseAIController : MonoBehaviour
    {

        public NavMeshAgent agent;
        public Transform target;
        public StateMashine<State, object> _stateMashine;

        public List<Transform> Checkpoints =>
           RaceManager.Instance != null && RaceManager.Instance.Checkpoints != null
               ? RaceManager.Instance.Checkpoints
               : new List<Transform>();
        public TargetFinder targetFinder;

        public Transform AgroTarget { get; set; }
        public float VerticalInput { get;  set; }
        public float HorizontalInput { get;  set; }
        public bool Brake { get;  set; }
       


        public abstract StateMashine<State, object> GetBehavior();

        public void Start()
        {
            StartCoroutine(InitAI());
        }

        private IEnumerator InitAI()
        {
          
            yield return new WaitUntil(() => RaceManager.Instance != null);

            _stateMashine = GetBehavior();
        }

        public void Update()
        {
              if (_stateMashine == null)
        return;
            if (targetFinder != null)
            {
                AgroTarget = targetFinder.CurrentTarget;
            }

            _stateMashine.CurrentState.Execute();
            var nextState = _stateMashine.CurrentState.TryGetNexState();
            if (nextState != null)
            {
                _stateMashine.CurrentState = nextState;
            }

        }
      

    }
}

