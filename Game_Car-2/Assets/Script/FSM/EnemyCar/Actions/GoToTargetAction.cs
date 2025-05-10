
using UnityEngine;
using UnityEditor.Search;

namespace Assets.Script.FSM.EnemyCar.Actions
{
    public class GoToTargetAction : BaseAction
    {
        public StartRaisr raisr;
        public GoToTargetAction(BaseAIController controller) : base(controller)
        {

        }

        public override void Execute()
        {
            _controller.agent.destination = _controller.target.position;
        }

    }
}
