using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private Transform lastChek;
    public List<Transform> Checkpoints =>
           RaceManager.Instance != null && RaceManager.Instance.Checkpoints != null ? RaceManager.Instance.Checkpoints :
            new List<Transform>();


    private void OnTriggerEnter(Collider other)
    {
        foreach (Transform checkpoint in Checkpoints)
        {
            if (other.transform == checkpoint)
            {
                lastChek = checkpoint;
                Debug.Log($"[Respawn] Last checkpoint updated: {lastChek.name}");
                break;
            }
        }

    }

    public void Resp()
    {
        var car = gameObject.GetComponent<Rigidbody>();
        car.isKinematic = true;
        gameObject.transform.position = lastChek.position;
        gameObject.transform.forward = lastChek.forward;
        car.isKinematic = false;
    }
    
}
