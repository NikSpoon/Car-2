using System.Collections;
using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Conditions
{
   
        public class AgrtoTimerConditions : BaseCondition
        {
            private BaiseCar _car;
            private bool _canEnterAggro = true;
            private bool _isAggroActive = false;
            private float _agroStartTime;

            public AgrtoTimerConditions(BaseAIController controller) : base(controller)
            {
                _car = controller as BaiseCar;
            }

            public bool IsAggroActive => _isAggroActive;
            public bool IsCooldown => !_canEnterAggro;

            public void StartAggro()
            {
                if (!_canEnterAggro || _isAggroActive) return;

                _car.StartCoroutine(TimerRoutine());
            }

            private IEnumerator TimerRoutine()
            {
                _canEnterAggro = false;
                _isAggroActive = true;
                _agroStartTime = Time.time;

                _car.InvokeAgroCooldownEvent(_car.AgroTime, _car.AgroCooldownTime);

                yield return new WaitForSeconds(_car.AgroTime);
                _isAggroActive = false;

                yield return new WaitForSeconds(_car.AgroCooldownTime);
                _canEnterAggro = true;
            }

            public override bool Evoluete()
            {
                // Эта проверка используется как ограничение на вход в агро
                return _canEnterAggro;
            }

            public bool ShouldExitAggro()
            {
                return !_isAggroActive || _car.HasCollidedWithEnemy;
            }
        }
    }