using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartRaisr : MonoBehaviour
{
    [SerializeField] private Transform _enemiesParent; 



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
                _checkpointManager.CheckpointsWithStart[0] = point;

                for (int i = 0; i < _checkpointManager.Len; i++)
                {
                    _checkpointManager.CheckpointsWithStart[i + 1] = _checkpointManager.Checkpoints[i];
                }


                var enemy = Instantiate(_enemy, point.position, point.rotation, _enemiesParent);
                var enemyCar = enemy.GetComponent<EnemyCar>();
                enemyCar.InitializeTargets(ConvertToGameObjects(_checkpointManager.CheckpointsWithStart));
                enemyCar.enabled = true;
            }
        }
    }
    private GameObject[] ConvertToGameObjects(Transform[] transforms)
    {
        GameObject[] result = new GameObject[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            result[i] = transforms[i].gameObject;
        }
        return result;
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