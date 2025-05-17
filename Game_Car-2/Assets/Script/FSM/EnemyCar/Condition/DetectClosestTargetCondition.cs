

using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class DetectClosestTargetCondition : BaseCondition
    {
        public DetectClosestTargetCondition(BaseAIController controller) : base(controller)
        {
        }

        public override bool Evoluete()
        {
            SetClosestPlayerAsTarget(_controller);
            return true; // или логика "если игрок ближе X метров"
        }
        private void SetClosestPlayerAsTarget(BaseAIController ai)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            Transform closest = null;
            float minDistance = Mathf.Infinity;

            foreach (var player in players)
            {
                float dist = Vector3.Distance(ai.transform.position, player.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = player.transform;
                }
            }

            if (closest != null)
            {
                ai.AgroTarget = closest;
            }
        }
    }

}
