
using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    class StartCondition : BaseCondition
    {
        private Rigidbody _rb;

        public StartCondition(BaseAIController controller) : base(controller)
        {

            _rb = _controller.GetComponent<Rigidbody>();
        }

        public override bool Evoluete()
        {
            return !_rb.isKinematic; 
        }
    }
}
