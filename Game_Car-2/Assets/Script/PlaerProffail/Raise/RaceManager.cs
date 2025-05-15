

using System.Collections.Generic;
using UnityEngine;

class RaceManager : MonoBehaviour
{
    [SerializeField] private CarSpawner _carSpawner;

    private List<Transform> _cheakPoint = new();
    private GameObject[] ollcars;

    private void Awake()
    {
       
    }


}

