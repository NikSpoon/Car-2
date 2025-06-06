using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] private NoCollision _noCollision;
    private Transform lastChek;
    public List<Transform> Checkpoints =>
           RaceManager.Instance != null && RaceManager.Instance.Checkpoints != null ? RaceManager.Instance.Checkpoints :
            new List<Transform>();


    private void Start()
    {
        StartCoroutine(InitCheckpoints());
    }
    private void OnTriggerEnter(Collider other)
    {
        foreach (Transform checkpoint in Checkpoints)
        {
            if (other.transform == checkpoint)
            {
                lastChek = checkpoint;
               // Debug.Log($"[Respawn] Last checkpoint updated: {lastChek.name}");
                break;
            }
        }

    }

    private IEnumerator InitCheckpoints()
    {
        while(Checkpoints != null)
        {
            yield return new WaitUntil(() =>
        RaceManager.Instance != null &&
        RaceManager.Instance.Checkpoints != null &&
        RaceManager.Instance.Checkpoints.Count > 0
    );
        }
        lastChek = Checkpoints[0];
    }
    public void Resp()
    {
        var car = gameObject.GetComponent<Rigidbody>();
        car.isKinematic = true;
        
        
        _noCollision.Respawn();
        
        gameObject.transform.position = lastChek.position;
        gameObject.transform.forward = lastChek.forward;
        
        car.isKinematic = false;
    }
    
}
