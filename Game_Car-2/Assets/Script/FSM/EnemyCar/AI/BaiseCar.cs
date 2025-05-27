using Assets.Script.FSM.EnemyCar.Actions;
using Assets.Script.FSM.EnemyCar.Condition;
using Assets.Script.FSM.EnemyCar.Conditions;
using Assets.Script.FSM.EnemyCar;
using UnityEngine;

public class BaiseCar : BaseAIController
{
    public AgroCooldownCondition AgroCooldown { get; private set; }

    private int currentCheckpointIndex = 0;
    
   
    public override StateMashine<State, object> GetBehavior()
    {
        if (Checkpoints.Count > 0)
        {
            Target = Checkpoints[currentCheckpointIndex];
            
        }
        else
        {
            Debug.LogError("Checkpoints list is empty!");
            
        }

        if (Target != null)
        {
            agent.SetDestination(Target.position);
        }

        AgroCooldown = new AgroCooldownCondition(this);
       

        var startState = new State("StartState");
        var raceState = new State("RaceState");
        var aggroState = new State("AggroState");
        var finishState = new State("FinishState");

        var goToCheckpoint = new GoToTargetAction(this);
        var agroToEnemy = new AgroTargetAction(this);
        var burnout = new StartActions(this);

        // Состояния
        startState.actions.Add(burnout);
        
        raceState.actions.Add(goToCheckpoint);
        aggroState.actions.Add(agroToEnemy);
        finishState.actions.Add(burnout);

        // Переходы
        startState.transitions.Add(new Transition<State, BaseCondition>(raceState, new StartCondition(this)));

        raceState.transitions.Add(new Transition<State, BaseCondition>(aggroState, new DetectClosestTargetCondition(this, AgroCooldown)));
        raceState.transitions.Add(new Transition<State, BaseCondition>(finishState, new ReachedFinishCondition(this)));

        aggroState.transitions.Add(new Transition<State, BaseCondition>(raceState, AgroCooldown));
        aggroState.transitions.Add(new Transition<State, BaseCondition>(finishState, new ReachedFinishCondition(this)));

        return new StateMashine<State, object>(startState);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что это чекпоинт
        Transform checkpointTransform = other.transform;

        // Проверяем, есть ли этот чекпоинт в нашем списке
        int index = Checkpoints.IndexOf(checkpointTransform);
        if (index == -1)
        {
            // Этот чекпоинт не из нашего списка — игнорируем
            return;
        }

        // Проверяем, что это текущий чекпоинт, который мы ждем
        if (index == currentCheckpointIndex)
        {
            OnCheckpointReached(checkpointTransform);

            // Убираем пройденный чекпоинт из списка или деактивируем его
            // Например, просто деактивируем объект:
            Checkpoints[index].gameObject.SetActive(false);

            // Или удаляем из списка, если хочешь:
            // Checkpoints.RemoveAt(index);
        }
        else
        {
            Debug.Log($"Пройден чекпоинт {checkpointTransform.name}, но мы ждём {Checkpoints[currentCheckpointIndex].name}");
        }
    }

    public void OnCheckpointReached(Transform checkpoint)
    {
        Debug.Log($"Checkpoint reached: {checkpoint.name}, next target: {(currentCheckpointIndex + 1 < Checkpoints.Count ? Checkpoints[currentCheckpointIndex + 1].name : "Finish")}");

        currentCheckpointIndex++;
        if (currentCheckpointIndex >= Checkpoints.Count)
        {
            Debug.Log("Все чекпоинты пройдены!");
            // Здесь можно переключиться в состояние финиша или что-то ещё
        }
        else
        {
            Target = Checkpoints[currentCheckpointIndex];
           
                agent.SetDestination(Target.position);
            
        }
    }
}
