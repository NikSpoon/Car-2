using UnityEngine;
using UnityEngine.InputSystem;

public class StartRaisr : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;
    [SerializeField] private Transform[] _startPoints;
    [SerializeField] private GameObject _player;
    [SerializeField] private CheckpointManager _checkpointManager;
    
    private int _randomValue;

    private void SpawnEnemies()
    {
        foreach (var point in _startPoints)
        {

            if (point == _startPoints[_randomValue])
            {
                StartPoint(point);
            }
            else
            {
                var enemy  = Instantiate(_enemy, point.position, point.rotation);
                var com = enemy.gameObject.GetComponent<EnemyCar>();
                com.enabled = true;
                _checkpointManager.CheckpointsWithStart[0] = point;

                for (int i = 0; i < _checkpointManager.Len; i++)
                {
                    _checkpointManager.CheckpointsWithStart[i + 1] = _checkpointManager.Checkpoints[i];
                }
            }
        }
    }

    private void StartPoint(Transform point)
    {
        _player.transform.position = point.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _randomValue = Random.Range(0, _startPoints.Length);
            SpawnEnemies();
        }
    }
}