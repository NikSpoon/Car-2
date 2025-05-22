
using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    class StartCondition : BaseCondition
    {
        private CarSpawner start;


        public StartCondition(BaseAIController controller) : base(controller)
        {

            
        }

        public override bool Evoluete()
        {
          
                return start.start;
            
            
        }
    }
}
