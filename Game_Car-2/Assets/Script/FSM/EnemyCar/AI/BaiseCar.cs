using Assets.Script.FSM.EnemyCar.Actions;
using Assets.Script.FSM.EnemyCar.Condition;

namespace Assets.Script.FSM.EnemyCar
{
    public class BaiseCar : BaseAIController
    {
         public AgroCooldownCondition AgroCooldown { get; private set; }

        public override StateMashine<State, object> GetBehavior()
        {
            AgroCooldown = new AgroCooldownCondition(this);
           
            var startState = new State();
            var raiseState = new State();
            var aggroState = new State();
            var finishState = new State();

            var goAction = new GoToTargetAction(this);
            var agroAction = new AgroTatgetTo(this);
            var agroCooldown = new AgroCooldownCondition(this);

            // Переход из старт в гонку
            startState.transitions.Add(new Transition<State, BaseCondition>(raiseState, new StartCondition(this)));

            // Переходы из гонки:
            raiseState.transitions.Add(new Transition<State, BaseCondition>(aggroState, new DetectClosestTargetCondition(this)));
            raiseState.transitions.Add(new Transition<State, BaseCondition>(finishState, new ReachedFinishCondition(this, goAction)));

            // Переходы из агро:
            aggroState.transitions.Add(new Transition<State, BaseCondition>(raiseState, new AgroCooldownCondition(this))); 
            aggroState.transitions.Add(new Transition<State, BaseCondition>(finishState, new ReachedFinishCondition(this, goAction)));

            // Добавляем действия в состояния
            startState.actions.Add(new StartActions(this));
            raiseState.actions.Add(goAction);
            aggroState.actions.Add(agroAction);
            finishState.actions.Add(new StartActions(this)); // или какое-то финальное действие


            return new StateMashine<State, object>(startState);
        }
     
    }

}
