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
            return null; // Не создаём поведение если нет чекпоинтов
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

    public void Update()
    {
        base.Update();

        // Проверяем расстояние до текущей цели (чекпоинта)
        if (Target != null)
        {
            float distance = Vector3.Distance(transform.position, Target.position);
            if (distance < 3f)  // Достигли чекпоинта
            {
                currentCheckpointIndex++;
                if (currentCheckpointIndex >= Checkpoints.Count)
                {
                    currentCheckpointIndex = 0; // Цикл по чекпоинтам
                }
                Target = Checkpoints[currentCheckpointIndex];
                Debug.Log($"Next checkpoint set: {currentCheckpointIndex}");
            }
        }
    }
}