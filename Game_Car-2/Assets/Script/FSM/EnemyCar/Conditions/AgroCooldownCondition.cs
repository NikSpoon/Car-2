using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class AgroCooldownCondition : BaseCondition
    {
        private float cooldownTime = 5f;
        private float lastAgroTime = -999f;

        public AgroCooldownCondition(BaseAIController controller) : base(controller)
        {
        }

        public void ResetCooldown()
        {
            lastAgroTime = Time.time;
        }

        public override bool Evoluete()
        {
            return Time.time - lastAgroTime > cooldownTime;
        }
    }
}