using System.Collections.Generic;

namespace Assets.Script.FSM.EnemyCar
{

    public  class State
    {
        public List<BaseAction> actions;
        public List<Transition<State, BaseCondition>> transitions;

        public void Execute()
        {
            foreach (var act in actions)
            {
                act.Execute();
            }
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
