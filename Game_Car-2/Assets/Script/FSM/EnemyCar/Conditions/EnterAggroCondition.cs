using UnityEngine;
using Assets.Script.FSM.EnemyCar.Conditions; // важно для доступа к AgrtoTimerConditions

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class EnerAggroCondition : BaseCondition
    {
        private BaiseCar _car;
        private AgrtoTimerConditions _timerCondition;

        public EnerAggroCondition(BaseAIController controller, AgrtoTimerConditions timerCondition) : base(controller)
        {
            _car = controller as BaiseCar;
            _timerCondition = timerCondition;
        }

        public override bool Evoluete()
        {
            if (_car.AgroTarget == null) return false; // нет цели — нет агро

            if (_timerCondition == null)
            {
                Debug.LogWarning("AggroTimerCondition не найден!");
                return false;
            }

            if (_timerCondition.IsCooldown) return false; // кулдаун активен

            // если все условия выполнены — запускаем агро и возвращаем true
            _timerCondition.StartAggro();
            return true;
        }
    }
}
