using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class ReachedFinishCondition : BaseCondition
    {
        public ReachedFinishCondition(BaseAIController controller) : base(controller)
        {
        }

        public override bool Evoluete()
        {
            var target = _controller.Target;
            if (target == null)
                return false;

            float distance = Vector3.Distance(_controller.transform.position, target.position);
            return distance < 3f;
        }
    }
}