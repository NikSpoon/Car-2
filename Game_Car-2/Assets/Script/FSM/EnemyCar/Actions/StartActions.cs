
namespace Assets.Script.FSM.EnemyCar.Actions
{
    class StartActions : BaseAction
    {
        public StartActions(BaseAIController controller) : base(controller)
        {
        }

        public override (float vertical, float horizontal, bool brake) Execute()
        {
            return (1f, 0f, false);
        }
    }
}
