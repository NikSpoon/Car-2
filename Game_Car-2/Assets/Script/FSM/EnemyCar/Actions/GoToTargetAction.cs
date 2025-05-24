using UnityEngine;

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

            float vertical = CalculateThrottle();
            float horizontal = SteerTowardsTarget();
            bool brake = ShouldBrake();

            Debug.Log($"Vertical: {vertical}, Horizontal: {horizontal}, Brake: {brake}");
            return (vertical, horizontal, brake);

        }

        /// <summary>
        /// Пока просто возвращает "1", чтобы ехать вперёд
        /// </summary>
        private float CalculateThrottle()
        {
            return 1f;
        }

        /// <summary>
        /// Пока пустой, вернёт 0 (прямо)
        /// В будущем можно сделать поворот в сторону цели
        /// </summary>
        private float SteerTowardsTarget()
        {
            return 0f;
        }

        /// <summary>
        /// Пока не тормозит, можно реализовать замедление при приближении к цели
        /// </summary>
        private bool ShouldBrake()
        {
            return false;
        }
    }
}