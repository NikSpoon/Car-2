using System;

namespace Gameplay.App
{

    public interface IAppSystem
    {
         AppState CurrentState { get; }
         void Trigger(AppTriger trigger);
         event Action<StateChangeData<AppState, AppTriger>> OnStateChange;
    }

    public class AppSystem : IAppSystem 
    {
        private StateMashine<AppState, AppTriger> _stateMashine;
        public AppState CurrentState => _stateMashine.CurrentState;

         event Action<StateChangeData<AppState, AppTriger>> IAppSystem.OnStateChange
        {
            add =>_stateMashine.OnStateChange += value;
            remove => _stateMashine.OnStateChange -= value;
        }

        public AppSystem()
        {
            _stateMashine = new StateMashine<AppState, AppTriger>(AppState.Loading);

            _stateMashine.AddTransition(AppState.Loading, AppTriger.ToMainMenu, AppState.MainMenu);

            _stateMashine.AddTransition(AppState.MainMenu, AppTriger.ToGerage, AppState.Garage);
            
            _stateMashine.AddTransition(AppState.Garage, AppTriger.ToMainMenu, AppState.MainMenu);
            _stateMashine.AddTransition(AppState.Garage, AppTriger.ToGameplay, AppState.Gameplay);

            _stateMashine.AddTransition(AppState.Gameplay, AppTriger.ToFinish, AppState.Finish);

            _stateMashine.AddTransition(AppState.Finish, AppTriger.ToGerage, AppState.Garage);
            _stateMashine.AddTransition(AppState.Finish, AppTriger.ToMainMenu, AppState.MainMenu);

        }

        public void Trigger(AppTriger trigger)
        {
            _stateMashine.SetTrigger(trigger);

        }

    }
   
    public enum AppState
    {
        Loading,
        MainMenu,
        Garage,
        Gameplay,
        Finish
        
    }

    public enum AppTriger
    {
        ToMainMenu,
        ToGerage,
        ToGameplay,
        ToFinish
    }
}

