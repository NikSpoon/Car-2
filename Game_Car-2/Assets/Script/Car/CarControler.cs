using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using static UnityEngine.LightTransport.InputExtraction;

public class CarControler : MonoBehaviour
{
    [SerializeField] private CarMovement _carMovement;
    [SerializeField] private InputServis _inputServis;

    [SerializeField] private bool IsPlayerControl;
    [SerializeField] private bool IsEnamyControl;

    private void Start()
    {
        if (gameObject.tag == ("Player"))
            IsPlayerControl = true;

    }
    void Update()
    {
        if (IsPlayerControl)
            IsPlayer();
        else
            IsEnamy();

    }
    private void IsPlayer()
    {
        _carMovement.Move(_inputServis.VerticalInput);
        _carMovement.Steering(_inputServis.HorizontalInput);
        _carMovement.Brake(_inputServis.Brake);

    }

    private void IsEnamy()
    {

    }


}
