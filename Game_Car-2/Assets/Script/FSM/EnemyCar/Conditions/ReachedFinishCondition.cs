using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class ReachedFinishCondition : BaseCondition
    {

        private BaiseCar _baiseCar;

        public ReachedFinishCondition(BaseAIController controller) : base(controller)
        {
            _baiseCar = controller as BaiseCar;
        }

        public override bool Evoluete()
        {
            if (_baiseCar == null)
            {
                Debug.LogError("ReachedFinishCondition: _baiseCar is null!");
                return false;
            }

            bool finished = _baiseCar.AllCheckpointsPassed();
            if (finished)
                Debug.Log("ReachedFinishCondition: Все чекпоинты пройдены, переход к FinishState");

            return finished;
        }
    }
}