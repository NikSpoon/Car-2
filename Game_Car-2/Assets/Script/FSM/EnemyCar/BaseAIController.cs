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

        public List<Transform> Checkpoints => RaceManager.Instance.Checkpoints;
       
        public float VerticalInput { get;  set; }
        public float HorizontalInput { get;  set; }
        public bool Brake { get;  set; }
       
        public void Awake()
        {
            
            _stateMashine = GetBehavior();

        }

        public abstract StateMashine<State, object> GetBehavior();


        public void Update()
        {

            _stateMashine.CurrentState.Execute();
            var nextState = _stateMashine.CurrentState.TryGetNexState();
            if (nextState != null)
            {
                _stateMashine.CurrentState = nextState;
            }

        }
      

    }
}

