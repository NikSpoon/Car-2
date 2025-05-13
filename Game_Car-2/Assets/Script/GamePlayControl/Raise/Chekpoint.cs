using System.Collections.Generic;
using UnityEngine;

public class Chekpoint : MonoBehaviour
{
    [SerializeField] private string _targetTags = "Player";
    
    public void OnTriggerEnter(Collider other)
    {
       if(other.gameObject == GameObject.FindGameObjectWithTag(_targetTags))
        {
            RaiseChekpoint manager = FindFirstObjectByType<RaiseChekpoint>();
            if (manager != null)
            {
                manager.UpdateCheckpoint(transform);
            }

            gameObject.SetActive(false);
       
        }


    }
  
}
