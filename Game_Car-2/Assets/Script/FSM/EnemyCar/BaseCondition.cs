

namespace Assets.Script.FSM.EnemyCar
{
    public abstract class BaseCondition
    {
        protected BaseAIController _controller;

        public BaseCondition(BaseAIController controller)
        {   
            this._controller = controller;
        }

        public abstract bool Evoluete();

    }
}
