using Assets.Script.FSM.EnemyCar.Actions;
using Assets.Script.FSM.EnemyCar.Condition;

namespace Assets.Script.FSM.EnemyCar
{
    public class BaiseCar : BaseAIController
    {
        public override StateMashine<State, object> GetBehavior()
        {
            var startState = new State();
            var raiseState = new State();
            var agresivState = new State();
            var finishState = new State();

            startState.transitions.Add(new Transition<State, BaseCondition>(raiseState, new StartCondition(this)));


            raiseState.transitions.Add(new Transition<State, BaseCondition>(agresivState, new StartCondition(this)));
            raiseState.transitions.Add(new Transition<State, BaseCondition>(finishState, new StartCondition(this)));


            agresivState.transitions.Add(new Transition<State, BaseCondition>(agresivState, new StartCondition(this)));
            agresivState.transitions.Add(new Transition<State, BaseCondition>(finishState, new StartCondition(this)));


            startState.actions.Add(new GoToTargetAction(this));
            raiseState.actions.Add(new GoToTargetAction(this));
            agresivState.actions.Add(new GoToTargetAction(this));
            finishState.actions.Add(new GoToTargetAction(this));
            

            return null;
        }
     
    }

}
