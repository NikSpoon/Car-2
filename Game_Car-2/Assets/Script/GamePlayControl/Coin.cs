using System;
using UnityEngine;


public class Coin : MonoBehaviour
{
    public static event Action<int> OnCoinUp;
    private int _coinValue = 1;
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {

            if (this.gameObject.CompareTag("Coin"))
            {

                gameObject.SetActive(false);

                OnCoinUp?.Invoke(_coinValue);


            }

        }
    }
}