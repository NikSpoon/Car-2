using Assets.Script.FSM.EnemyCar.Actions;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class ReachedFinishCondition : BaseCondition
    {
        private GoToTargetAction _goAction;

        public ReachedFinishCondition(BaseAIController controller, GoToTargetAction goAction) : base(controller)
        {
            _goAction = goAction;
        }

        public override bool Evoluete()
        {
            return _goAction.IsAtLastCheckpoint();
        }
    }
}