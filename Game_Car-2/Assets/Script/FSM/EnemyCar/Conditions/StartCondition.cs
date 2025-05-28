using UnityEngine;
using UnityEngine.WSA;
namespace Assets.Script.FSM.EnemyCar.Conditions
{
    public class StartCondition : BaseCondition
    {
        public StartCondition(BaseAIController controller) : base(controller) { }

        public override bool Evoluete()
        {
            if (_controller.carSpawner == null)
            {
                Debug.Log("_controller.carSpawner is NULL");
                return false;
            }

            Debug.Log(_controller.carSpawner.start + " _controller.carSpawner.start");

            return _controller.carSpawner.start;
        }
    }
}