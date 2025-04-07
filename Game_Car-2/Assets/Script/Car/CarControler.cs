
using UnityEngine;


public class CarControler : MonoBehaviour
{
    //[SerializeField] private CarMovement _carMovement;
    [SerializeField] private CarPhysic _carPhysic;
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
        _carPhysic.Move(_inputServis.VerticalInput);
    }

    private void IsEnamy()
    {

    }


}
