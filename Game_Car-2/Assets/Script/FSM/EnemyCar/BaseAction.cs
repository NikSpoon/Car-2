namespace Assets.Script.FSM.EnemyCar
{
    public abstract class   BaseAction
    {
        protected BaseAIController _controller;

        public BaseAction(BaseAIController controller)
        {
            this._controller = controller;
        }
        public abstract void Execute();
    }
}
