
using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Actions
{
    class AgroTatgetTo : BaseAction
    { 
        public AgroTatgetTo(BaseAIController controller) : base(controller)
        {

            
        }

        public override (float vertical, float horizontal, bool brake) Execute()
        {
            Transform target = _controller.AgroTarget;
            if (target == null)
                return (0f, 0f, false);

            _controller.agent.SetDestination(target.position);

            Vector3 localVelocity = _controller.transform.InverseTransformDirection(
                _controller.agent.desiredVelocity.normalized
            );

            float vertical = localVelocity.z;
            float horizontal = localVelocity.x;
            bool brake = false;

            return (vertical, horizontal, brake);
        }

    }
}



