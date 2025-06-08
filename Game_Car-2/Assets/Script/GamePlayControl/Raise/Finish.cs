using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private List<GameObject> _CarColliders = new List<GameObject>();

    [SerializeField] private string[] targetTags;

    [SerializeField] private Canvas _finishUI;

    private void Awake()
    {
        _finishUI.gameObject.SetActive(false);
    }
    void Start()
    {
        foreach (string tag in targetTags)
        {
            GameObject[] cars = GameObject.FindGameObjectsWithTag(tag);

            foreach (var item in cars)
            {
               
              _CarColliders.Add(item);
            }

            
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (var collider in _CarColliders)
        {
            if(other.tag == collider.tag)
            {
                _finishUI.gameObject.SetActive(true);
            }

        }
    }
}
