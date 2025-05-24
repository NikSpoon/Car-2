using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class DetectClosestTargetCondition : BaseCondition
    {
        private float detectionRadius = 20f;
        private AgroCooldownCondition _agroCooldown;

        public DetectClosestTargetCondition(BaseAIController controller, AgroCooldownCondition agroCooldown) : base(controller)
        {
            _agroCooldown = agroCooldown;
        }

        public override bool Evoluete()
        {
            // Сначала проверяем, прошел ли кулдаун агрессии
            if (!_agroCooldown.Evoluete())
                return false;

            var target = _controller.AgroTarget;
            if (target == null)
                return false;

            float distance = Vector3.Distance(_controller.transform.position, target.position);
            if (distance > detectionRadius)
                return false;

            // Предположим, что у обоих есть компонент Health с полем CurrentHP
            var myHealth = _controller.GetComponent<Health>();
            var targetHealth = target.GetComponent<Health>();

            if (myHealth == null || targetHealth == null)
                return false;

            // Переходим в агро, если наше здоровье больше чем у врага
            if (myHealth.CurrentHealth > targetHealth.CurrentHealth)
            {
                // Сбрасываем кулдаун (потому что мы сейчас агрируем)
                _agroCooldown.ResetCooldown();
                return true;
            }

            return false;
        }
    }
}