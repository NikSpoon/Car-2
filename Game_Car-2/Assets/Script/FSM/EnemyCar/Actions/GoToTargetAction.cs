

using UnityEngine;


namespace Assets.Script.FSM.EnemyCar.Actions
{
    public class GoToTargetAction : BaseAction
    {
        private int _currentCheckpointIndex = 0;
        public GoToTargetAction(BaseAIController controller) : base(controller)
        {
            
            if (_controller.Checkpoints.Count > 0)
            {
                _controller.agent.SetDestination(_controller.Checkpoints[_currentCheckpointIndex].position);
            }
        }

        public override (float vertical, float horizontal, bool brake) Execute()
        {

            if (!_controller.agent.pathPending && _controller.agent.remainingDistance < 1.0f)
            {
                _currentCheckpointIndex++;

                if (_currentCheckpointIndex < _controller.Checkpoints.Count)
                {
                    _controller.agent.SetDestination(_controller.Checkpoints[_currentCheckpointIndex].position);
                }
                
            }
                Vector3 localVelocity = _controller.transform.InverseTransformDirection(_controller.agent.desiredVelocity.normalized);

                float vertical = localVelocity.z;    // вперёд/назад
                float horizontal = localVelocity.x;  // поворот
                bool brake = false;                  // опционально: тормоз при необходимости

            return (vertical, horizontal, brake);
        }
        public bool IsAtLastCheckpoint()
        {
            return _currentCheckpointIndex == _controller.Checkpoints.Count - 1 &&
                   _controller.agent.remainingDistance < 1.0f;
        }

    }
}
