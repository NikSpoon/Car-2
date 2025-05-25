using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.FSM.EnemyCar.Actions
{
    public class GoToTargetAction : BaseAction
    {
        private readonly float slowDownRadius = 10f;
        private readonly LayerMask obstacleLayer = 1 << 8;

        public GoToTargetAction(BaseAIController controller) : base(controller) { }

        public override (float vertical, float horizontal, bool brake) Execute()
        {
            if (_controller.Target == null)
                return (0f, 0f, false);
          
            _controller.agent.SetDestination(_controller.Target.position);

            float vertical = CalculateThrottle();
            float horizontal = SteerTowardsTarget();
            bool brake = ShouldBrake();


            return (vertical, horizontal, brake);

        }

       
        private float CalculateThrottle()
        {
            return 0.2f;
        }

        private float SteerTowardsTarget()
        {
            if (_controller.Target == null)
                return 0f;

            Vector3 targetPosition = _controller.Target.position;

            //Debug.Log("Target position: " + targetPosition);

            Vector3 directionToTarget = (targetPosition - _controller.transform.position).normalized;

            directionToTarget.y = 0;
            Vector3 forward = _controller.transform.forward;
            forward.y = 0;

            float angle = Vector3.SignedAngle(forward, directionToTarget, Vector3.up);

            //Debug.Log("Angle to target: " + angle+ " name: " +  _controller.Target.name);

            float steer = Mathf.Clamp(angle / 45f, -1f, 1f);
            //Debug.Log("Steer value: " + steer);

            return steer;

        }

        private bool ShouldBrake()
        {
            return false;
        }
    }
}