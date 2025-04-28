using UnityEngine;


public class CoinSpawn : MonoBehaviour
{

   
        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private int _value = 5;          
        [SerializeField] private float _spawnRadius = 1.0f; 

    private bool _hasSpawned = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (_hasSpawned) return;
        _hasSpawned = true;

        for (int i = 0; i < _value; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * _spawnRadius;
            randomOffset.y = Mathf.Abs(randomOffset.y); 

            Vector3 spawnPosition = transform.position + randomOffset;
            Instantiate(_coinPrefab, spawnPosition, Quaternion.identity);
        }
    }
}

