
namespace Assets.Script.FSM.EnemyCar.Condition
{
    class StartCondition : BaseCondition
    {
       
        public StartCondition(BaseAIController controller) : base(controller)
        {
            
        }

        public override bool Evoluete()
        {
            return false;
        }
    }
}
