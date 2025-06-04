using UnityEngine;
using UnityEngine.AI;

namespace Assets.Script.FSM.EnemyCar.Actions
{
    class AgroTargetAction : BaseAction
    {
        private GameObject visualObject;

        public AgroTargetAction(BaseAIController controller) : base(controller) 
        {
          
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (_controller is BaiseCar car)
            {
                car.IsExecutingAgroAction = true;
            }
            if (_controller.AgroTarget != null)
            {
                Transform visual = _controller.AgroTarget.transform.Find("AgroMesh");
                Debug.Log(visual.name);
                if (visual != null)
                {
                    visual.gameObject.SetActive(true);
                    visualObject = visual.gameObject;
                }

            }
            
        }

        public override void OnExit()
        {
            base.OnExit();
          
            if (_controller is BaiseCar car)
            {
                car.IsExecutingAgroAction = false;
            }
            if (visualObject != null)
                visualObject.SetActive(false);
        }
        public override (float vertical, float horizontal, bool brake) Execute()
        {
            if (_controller.AgroTarget == null)
                return (0f, 0f, false);
           
            _controller.agent.SetDestination(_controller.AgroTarget.position);

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