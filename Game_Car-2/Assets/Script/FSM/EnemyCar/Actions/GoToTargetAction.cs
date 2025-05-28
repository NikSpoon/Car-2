using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

namespace Assets.Script.FSM.EnemyCar.Actions
{
    public class GoToTargetAction : BaseAction
    {
       
        private float _currentThrottle = 0f;
        public GoToTargetAction(BaseAIController controller) : base(controller) { }

        public override (float vertical, float horizontal, bool brake) Execute()
        {
            if (_controller.Target == null)
                return (0f, 0f, false);
          
            
            float vertical = CalculateThrottle();
            float horizontal = SteerTowardsTarget();
            bool brake = ShouldBrake();


            return (vertical, horizontal, brake);

        }


        private float CalculateThrottle()
        {
            var car = _controller.GetComponent<CarPhysic>();
            if (car == null)
                return 0.2f;  // или какое-то дефолтное значение газа

            float targetSpeed = 180f;       // максимальная желаемая скорость
            float maxThrottle = 0.6f;      // максимальное значение газа
            float minThrottle = 0f;        // минимальное значение газа (газ выключен)

            float currentSpeed = car._speed;

            // Если скорость выше targetSpeed — снижаем газ к minThrottle
            // Если ниже — увеличиваем газ к maxThrottle
            // Чем дальше от целевой скорости, тем сильнее газ
            float desiredThrottle = currentSpeed < targetSpeed ? maxThrottle : minThrottle;

            // Плавно изменяем текущее значение газа к desiredThrottle
            // Можно сохранить текущее throttle в поле класса, например _currentThrottle, чтобы интерполяция была плавнее
            // Но для упрощения — используем MoveTowards к желаемому значению с фиксированной скоростью изменения

            // Предположим, что у тебя нет переменной _currentThrottle, создадим её:
            // В классе GoToTargetAction добавь:
            // private float _currentThrottle = 0f;

            _currentThrottle = Mathf.MoveTowards(_currentThrottle, desiredThrottle, Time.deltaTime * 0.5f);

            return _currentThrottle;
        }

        private float SteerTowardsTarget()
        {
            
            if (_controller.Target == null)
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