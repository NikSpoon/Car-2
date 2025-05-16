
using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class CheckPlayerNear : BaseCondition
    {
        
        public CheckPlayerNear(BaseAIController controller) : base(controller) { }

        public override bool Evoluete()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return false;

            float dist = Vector3.Distance(_controller.transform.position, player.transform.position);
            return dist < 10f; 
        }
    }
}
