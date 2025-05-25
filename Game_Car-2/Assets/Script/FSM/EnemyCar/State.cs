using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.FSM.EnemyCar
{

    public  class State
    {

        public string Name { get; private set; }

        public List<BaseAction> actions = new List<BaseAction>();
        public List<Transition<State, BaseCondition>> transitions = new List<Transition<State, BaseCondition>>();


        public float vertical = 0f;
        public float horizontal = 0f;
        public bool brake = false;
        public State(string name)
        {
            Name = name;
        }


        public (float vertical, float horizontal, bool brake) Execute()
        {
            float vertical = 0f;
            float horizontal = 0f;
            bool brake = false;

            foreach (var act in actions)
            {
                var (v, h, b) = act.Execute();
                vertical = v;
                horizontal = h;
                brake = b;
            }
        
            return (vertical, horizontal, brake);
        }
        

        public State TryGetNexState()
        {
            foreach (var transition in transitions)
            {
                if (transition.Trigger.Evoluete())
                {
                    return transition.NextState;
                }
            }
            return default;
        }

    }
}
