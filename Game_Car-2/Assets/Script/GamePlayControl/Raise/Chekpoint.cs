using System.Collections.Generic;
using UnityEngine;

public class Chekpoint : MonoBehaviour
{
    [SerializeField] private string _targetTags = "Player";
    public int checkpointIndex = 0;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameObject.FindGameObjectWithTag(_targetTags))
        {
            RaiseChekpoint manager = FindFirstObjectByType<RaiseChekpoint>();
            if (manager != null)
            {
                manager.UpdateCheckpoint(transform);
            }

            gameObject.GetComponent<MeshRenderer>().enabled = false;

        }


        if (other.CompareTag("Enemy"))
        {
            var aiController = other.GetComponent<BaiseCar>();
            if (aiController != null)
            {
                
                var car = other.GetComponent<BaiseCar>();
                if (car != null)
                {
                   

                }
            }
        }
    }
}



