using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.FSM.EnemyCar.Actions
{
    class AgroTargetAction : BaseAction
    {
        public AgroTargetAction(BaseAIController controller) : base(controller) { }

        public override (float vertical, float horizontal, bool brake) Execute()
        {
            if (_controller.AgroTarget == null)
                return (0f, 0f, false);


            float vertical = CalculateThrottle();
            float horizontal = SteerTowardsTarget();
            bool brake = ShouldBrake();


            return (vertical, horizontal, brake);

        }
        private float CalculateThrottle()
        {

            return 1f;
        }

        private float SteerTowardsTarget()
        {

            if (_controller.AgroTarget == null)
                return 0f;

            Vector3 worldDirection = (_controller.agent.steeringTarget - _controller.transform.position).normalized;

            // Преобразуем в локальные координаты (относительно машины)
            Vector3 localDirection = _controller.transform.InverseTransformDirection(worldDirection);

            // Угол поворота на основе направления по X
            float steerInput = Mathf.Clamp(localDirection.x, -1f, 1f);

            return steerInput;

        }

        private bool ShouldBrake()
        {
            return false;
        }

    }
}