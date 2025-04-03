
using UnityEngine;


public class CarControler : MonoBehaviour
{
    [SerializeField] private CarMovement _carMovement;
    [SerializeField] private InputServis _inputServis;
    [SerializeField] private EnemyCar _enemyCar;

    [SerializeField] private bool IsPlayerControl;
    [SerializeField] private bool IsEnamyControl;

    private void Start()
    {
        if (gameObject.tag == ("Player"))
            IsPlayerControl = true;
        else if (gameObject.tag == "Enemy")
            IsEnamyControl = true;
    }
    void Update()
    {
        if (IsPlayerControl)
        {
            IsPlayer();
        }
      
        else if (IsEnamyControl)
        {
            IsEnamy();
        }
    }
    private void IsPlayer()
    {
        _carMovement.Move(_inputServis.VerticalInput);
        _carMovement.Steering(_inputServis.HorizontalInput);
        _carMovement.Brake(_inputServis.Brake);

    }

    private void IsEnamy()
    {
        _carMovement.LookAtCheckpoint((_enemyCar.CurrentTarget.position - transform.position).normalized);
        _carMovement.Move(1f);
       // _carMovement.Steering(_enemyCar.HorizontalInput);
        _carMovement.Brake(false);
    }


}
