
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCollision : MonoBehaviour
{
    private List<Collider> _otherColliders = new List<Collider>(); 
    private List<Collider> _myColliders = new List<Collider>();

    [SerializeField] private MeshRenderer _mesh;

    [Header("Ghost Settings")]
    [SerializeField] private string[] targetTags;
    [SerializeField] private float ghostDuration = 3f;
    [SerializeField] private float checkInterval = 0.5f;
    [SerializeField] private float blinkInterval = 0.2f;

    private Coroutine _ghostRoutine;
    private bool IsGhostActive  = false;
    public event Action<float,bool> OnNoCollision;
    public void Respawn()
    {
         
        _myColliders.Clear();
        _myColliders.AddRange(GetComponentsInChildren<Collider>());

        _otherColliders.Clear();
        FindOtherColliders();

       
        if (_ghostRoutine != null)
        {
            StopCoroutine(_ghostRoutine);
        }

        _ghostRoutine = StartCoroutine(GhostRoutine());

    }

    private void FindOtherColliders()
    {
        foreach (string tag in targetTags)
        {
            GameObject[] cars = GameObject.FindGameObjectsWithTag(tag);
            
            foreach (GameObject car in cars)
            {
                if (car == gameObject) continue;
                Collider[] colliders = car.GetComponentsInChildren<Collider>();
                _otherColliders.AddRange(colliders); 
            }
        }
    }

    private IEnumerator GhostRoutine()
    {
        IsGhostActive = true;
        float timer = 0f;
        SetCollision(false); 

        Coroutine blink = StartCoroutine(BlinkMesh());

        
        while (timer < ghostDuration)
        {
           
            yield return new WaitForSeconds(checkInterval);
            OnNoCollision?.Invoke(ghostDuration - timer, IsGhostActive);
            timer += checkInterval;

            if (!IsOverlappingOtherCars())
            {
                break;
            }
        }

       
        SetCollision(true);

       
        if (blink != null)
        {
            StopCoroutine(blink);
        }

        
        SetMeshVisible(true);
        IsGhostActive = false;
        OnNoCollision?.Invoke(timer, IsGhostActive);
    }

    private void SetCollision(bool enabled)
    {
        
        foreach (var otherCol in _otherColliders)
        {
            if (otherCol != null)
            {
                foreach (var myCol in _myColliders)
                {
                    if (myCol != null)
                    {
                        Physics.IgnoreCollision(myCol, otherCol, !enabled);
                    }
                }
            }
        }
    }

    private bool IsOverlappingOtherCars()
    {
      
        foreach (var otherCol in _otherColliders)
        {
            if (otherCol != null)
            {
                foreach (var myCol in _myColliders)
                {
                    if (myCol != null && myCol.bounds.Intersects(otherCol.bounds))
                    {
                        return true; 
                    }
                }
            }
        }
        return false; 
    }

    private IEnumerator BlinkMesh()
    {
      
        while (true)
        {
            SetMeshVisible(false);
            yield return new WaitForSeconds(blinkInterval);
            SetMeshVisible(true);
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void SetMeshVisible(bool visible)
    {
      
        if (_mesh != null)
            _mesh.enabled = visible;
    }
}
