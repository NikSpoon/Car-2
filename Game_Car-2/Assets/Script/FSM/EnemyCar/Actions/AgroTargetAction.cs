using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.FSM.EnemyCar.Actions
{
    class AgroTargetAction : BaseAction
    {
        public AgroTargetAction(BaseAIController controller) : base(controller) { }

        public override (float vertical, float horizontal, bool brake) Execute()
        {
            var agent = _controller.agent;
            var target = _controller.AgroTarget;

            if (agent == null || target == null)
                return (0f, 0f, false);

            // Обновляем цель в агенте
            if (agent.destination != target.position)
            {
                agent.SetDestination(target.position);
            }

            // Синхронизируем агент с текущей позицией машины
            agent.nextPosition = _controller.transform.position;

            // Получаем направление движения
            Vector3 desiredVelocity = agent.desiredVelocity;
            if (desiredVelocity.magnitude < 0.1f)
            {
                return (0f, 0f, true);
            }

            Vector3 desiredDir = desiredVelocity.normalized;
            float angle = Vector3.SignedAngle(_controller.transform.forward, desiredDir, Vector3.up);

            float horizontal = Mathf.Clamp(angle / 45f, -1f, 1f);
            float speedFactor = Mathf.Clamp01(1f - Mathf.Abs(horizontal));
            float vertical = 1f * speedFactor;

            float distance = Vector3.Distance(_controller.transform.position, target.position);
            bool brake = distance < 3f;

            if (brake)
            {
                vertical = 0f;
            }

            return (vertical, horizontal, brake);
        }
    }
}