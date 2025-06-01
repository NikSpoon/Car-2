using UnityEngine;
using Assets.Script.FSM.EnemyCar.Conditions;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class ExitAggroCondition : BaseCondition
    {
        private BaiseCar _car;
        private AgrtoTimerConditions _timerCondition;

        public ExitAggroCondition(BaseAIController controller, AgrtoTimerConditions timerCondition) : base(controller)
        {
            _car = controller as BaiseCar;
            _timerCondition = timerCondition;
        }

        public override bool Evoluete()
        {
            // выйти из агро, если оно завершилось или была коллизия
            return _timerCondition != null && _timerCondition.ShouldExitAggro();
        }
    }
}