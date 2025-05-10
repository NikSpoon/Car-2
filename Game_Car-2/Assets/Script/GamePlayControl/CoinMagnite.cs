using UnityEngine;
using System.Collections.Generic;

public class CoinMagnite : MonoBehaviour
{
    private List<Rigidbody> _coinsInRange = new List<Rigidbody>();

    [SerializeField] private float _baseSpeed = 5f;
    [SerializeField] private float _accelerationFactor = 2f; 
    private void FixedUpdate()
    {
        foreach (var coinRb in _coinsInRange)
        {
            if (coinRb != null)
            {
                
                Vector3 direction = (transform.position - coinRb.transform.position).normalized;

                
                float distance = Vector3.Distance(transform.position, coinRb.transform.position);

               
                float speed = _baseSpeed * Mathf.Clamp(1f / distance, 0.1f, 1f) * _accelerationFactor;

                
                coinRb.MovePosition(coinRb.transform.position + direction * speed * Time.fixedDeltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin")) 
        {
            Rigidbody coinRb = other.attachedRigidbody;
            if (coinRb != null && !_coinsInRange.Contains(coinRb))
            {
                _coinsInRange.Add(coinRb);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Rigidbody coinRb = other.attachedRigidbody;
            if (coinRb != null && _coinsInRange.Contains(coinRb))
            {
                _coinsInRange.Remove(coinRb);
            }
        }
    }
}