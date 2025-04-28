using System;
using UnityEngine;


public class Coin : MonoBehaviour
{
    public static event Action<int> OnCoinUp;
    private int _coinValue = 1;

    private Collider _pickupCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (this.gameObject.CompareTag("Coin"))
            {
                OnCoinUp?.Invoke(_coinValue);
                gameObject.SetActive(false);
            }
           
        }
    }
}
