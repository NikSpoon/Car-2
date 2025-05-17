
namespace Assets.Script.FSM.EnemyCar.Actions
{
    class StartActions : BaseAction
    {
        public StartActions(BaseAIController controller) : base(controller)
        {
        }

        public override (float vertical, float horizontal, bool brake) Execute()
        {
            float vertical = 1f;
            float horizontal = 0f;
            bool brake = true;
            
            return (vertical, horizontal, brake);
        }
    }
}
