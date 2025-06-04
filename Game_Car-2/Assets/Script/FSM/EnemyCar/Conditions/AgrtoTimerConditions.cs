using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Script.FSM.EnemyCar.Conditions
{
    public class AgrtoTimerConditions : BaseCondition
    {
        public bool _collision = false;
        public bool _canEnterAggro = false;
        public bool _isAggroActive = false;
        public bool _canExitAgro = false;

        private BaiseCar _car;
        private bool _start = true;
        private bool _cooldownRoutineRunning = false;
        private bool _agroRoutineRunning = false;
       
  
        public AgrtoTimerConditions(BaseAIController controller) : base(controller)
        {
            _car = controller as BaiseCar;
        }
          
        public override bool Evoluete()
        {
            if (!_controller.carSpawner.start)
            {
                // Гонка ещё не стартовала, не запускаем агро задержку
                return false;
            }


            if (_start)
            {
                _start = false;
            //    Debug.Log("⏳ Запускаю задержку перед агро");
                _car.StartCoroutine(StartDelayRoutine());
                
                
            }

            if (_collision && _isAggroActive)
            {
              //  Debug.Log("❌ Столкновение завершает агро");
                ExitAggroEarly();
                _collision = false;
            }

            if (_car.IsExecutingAgroAction)
            {
                if (!_agroRoutineRunning)
                {
                  //  Debug.Log("✅ Агро действие активировано, запускаю корутину агро");
                    _car.StartCoroutine(AgroRoutine());
                }
            }
            

            //Debug.Log($"Статусы: canEnterAggro={_canEnterAggro}, isAggroActive={_isAggroActive}");

            return _canEnterAggro && !_isAggroActive;
        }


        private void ExitAggroEarly()
        {
            _isAggroActive = false;
            _canExitAgro = true;

            if (!_cooldownRoutineRunning)
            {
                _car.StartCoroutine(CooldownRoutine());
            }
        }

        private IEnumerator StartDelayRoutine()
        {
         //   Debug.Log("⏳ Задержка перед первым агро стартует");
            yield return new WaitForSeconds(10f);
            _canEnterAggro = true;
         //   Debug.Log("✅ Агро теперь можно активировать");
        }

        private IEnumerator AgroRoutine()
        {
            _agroRoutineRunning = true;
            _isAggroActive = true;
            _canEnterAggro = false;
            _canExitAgro = false;

          //  Debug.Log("▶️ Агро начался");

            for (int i = _car.AgroTime; i > 0; i--)
            {
                _car.InvokeAgroCooldownEvent(i, 0); 
             //   Debug.Log($"[UI] Агро: {i} сек");
                yield return new WaitForSeconds(1f);

                // Прерывание, если агро отменён раньше
                if (!_isAggroActive)
                {
                  //  Debug.Log("❌ Агро прерван досрочно");
                    _car.InvokeAgroCooldownEvent(0, _car.AgroCooldownTime); // Переход в кулдаун
                    _agroRoutineRunning = false;
                    yield break;
                }
            }

          //  Debug.Log("⏱ Агро завершён по таймеру");
            _isAggroActive = false;
            _canExitAgro = true;

            _car.StartCoroutine(CooldownRoutine());
            _agroRoutineRunning = false;
        }

        private IEnumerator CooldownRoutine()
        {
            _cooldownRoutineRunning = true;
            _canEnterAggro = false;
            _canExitAgro = true;
          
           // Debug.Log("🕒 Кулдаун агро начат");


            for (int i = _car.AgroCooldownTime; i > 0; i--)
            {
                _car.InvokeAgroCooldownEvent(0, i); // Только кулдаун активен
             //   Debug.Log($"[UI] Кулдаун: {i} сек");
                yield return new WaitForSeconds(1f);
            }

          //  Debug.Log("🔁 Кулдаун окончен, агро снова доступен");
            _canEnterAggro = true;
            _cooldownRoutineRunning = false;
        }
    }
}
