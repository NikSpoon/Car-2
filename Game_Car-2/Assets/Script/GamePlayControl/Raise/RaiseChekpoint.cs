using UnityEngine;
using System.Collections.Generic;
public class RaiseChekpoint : MonoBehaviour
{
    [SerializeField] private Transform _firstPoint;
    [SerializeField] private Transform _Finish;

    [SerializeField] private List<Transform> _chekpoint;

    public Transform CurentPoint { get;  private set; }

    public void Awake()
    {
        CurentPoint = _firstPoint; 
        
    }
    private void SetCurentPoint()
    {
       
    }

}
