
using Assets.Script.FSM.EnemyCar;
using UnityEngine;


public class CarControler : MonoBehaviour
{
    
    [SerializeField] private CarPhysic _carPhysic;
    [SerializeField] private InputServis _inputServis;

    [SerializeField] private BaseAIController AI;


    public bool IsPlayerControl { get; set; }
    public bool IsEnamyControl { get; set; }
    private void Start()
    {
        if (gameObject.tag == ("Player"))
            IsPlayerControl = true;
        else if (gameObject.tag == "Enemy")
            IsEnamyControl = true;
    }
    void FixedUpdate()
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
        _carPhysic.Move(_inputServis.VerticalInput,_inputServis.HorizontalInput, _inputServis.Brake);
    }

    private void IsEnamy()
    {
        if (AI == null)
        {
            Debug.LogWarning("AI controller is not assigned!");
            return;
        }

        _carPhysic.Move(AI.VerticalInput, AI.HorizontalInput, AI.Brake);
    }


}
