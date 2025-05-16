using System.Collections.Generic;

namespace Assets.Script.FSM.EnemyCar
{

    public  class State
    {
        public List<BaseAction> actions;
        public List<Transition<State, BaseCondition>> transitions;

       
        public float vertical = 0f;
        public float horizontal = 0f;
        public bool brake = false;
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
