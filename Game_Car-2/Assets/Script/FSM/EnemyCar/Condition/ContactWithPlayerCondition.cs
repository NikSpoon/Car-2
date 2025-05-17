using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Condition
{
    public class ContactWithPlayerCondition : BaseCondition
    {
        public ContactWithPlayerCondition(BaseAIController controller) : base(controller) { }

        public override bool Evoluete()
        {
            float contactDistance = 2f; // примерное расстояние контакта
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return false;

            float dist = Vector3.Distance(_controller.transform.position, player.transform.position);
            bool contacted = dist < contactDistance;

            if (contacted)
            {
                // Сброс таймера агро, если хочешь
                var aggroCooldown = _controller.GetComponent<BaiseCar>()?.AgroCooldown; // Если доступен
                aggroCooldown?.SetExitTime();
            }

            return contacted;
        }
    }
}
