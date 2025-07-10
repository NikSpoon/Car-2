using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class CarUiChenger : NetworkBehaviour
{
    [SerializeField] private GameObject _lokal;
    [SerializeField] private GameObject _other;
    [SerializeField] private GameObject _bot;
    [SerializeField] private GameObject _canvasForEnemy;

    private void Awake()
    {
        _lokal.SetActive(false);
        _other.SetActive(false);
        _bot.SetActive(false);
        _canvasForEnemy.SetActive(false);
    }

    private void Start()
    {
        var car = gameObject.GetComponent<CarControler>();
        if (car.IsEnamyControl)
        {
            _canvasForEnemy.SetActive(true);
            _bot.SetActive(true);
        }
        else if (car.IsPlayerControl)
        {
            _lokal.SetActive(true);
           
        }
        else if (car.IsPlayerControl && !isLocalPlayer)
        {
            _canvasForEnemy.SetActive(true);
            _other.SetActive(true);
        }
    }
}
