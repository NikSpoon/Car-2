using System.Collections.Generic;
using UnityEngine;

public class Chekpoint : MonoBehaviour
{
    [SerializeField] private string _targetTags = "Player";
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameObject.FindGameObjectWithTag(_targetTags))
        {
            gameObject.SetActive(false);
        }
    }
  
}
