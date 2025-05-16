
using UnityEngine;
using UnityEditor.Search;

namespace Assets.Script.FSM.EnemyCar.Actions
{
    public class GoToTargetAction : BaseAction
    {
       
        public GoToTargetAction(BaseAIController controller) : base(controller)
        {

        }

        public override (float vertical, float horizontal, bool brake) Execute()
        {
            
            float vertical = 1f; 
            float horizontal = 0f; 
            bool brake = false;

            return (vertical, horizontal, brake);
        }

    }
}
