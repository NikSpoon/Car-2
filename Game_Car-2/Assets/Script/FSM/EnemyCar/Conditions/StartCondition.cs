

namespace Assets.Script.FSM.EnemyCar.Conditions
{
    public class StartCondition : BaseCondition
    {
        public StartCondition(BaseAIController controller) : base(controller) { }

        public override bool Evoluete()
        {
            
            return _controller.carSpawner != null && _controller.carSpawner.start;
        }
    }
}