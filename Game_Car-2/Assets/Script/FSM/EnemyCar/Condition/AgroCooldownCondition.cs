using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class AgroCooldownCondition : BaseCondition
    {
        private float _cooldownTime = 5f; 
        private float _lastExitTime = -Mathf.Infinity;

        public AgroCooldownCondition(BaseAIController controller) : base(controller)
        {
        }


        public void SetExitTime()
        {
            _lastExitTime = Time.time;
        }

        public override bool Evoluete()
        {
           
            return Time.time - _lastExitTime > _cooldownTime;
        }
    }
}