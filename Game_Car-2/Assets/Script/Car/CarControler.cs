using Mirror;
using Assets.Script.FSM.EnemyCar;
using UnityEngine;
using System.Security.Principal;



public class CarControler : MonoBehaviour
{
    
    [SerializeField] private CarPhysic _carPhysic;
    [SerializeField] private InputServis _inputServis;
    [SerializeField] private Respawn _respawn;
    [SerializeField] private BaseAIController AI;

    [SerializeField] private NetworkIdentity _identity;

    public bool IsPlayerControl { get; set; }
    public bool IsEnamyControl { get; set; }
    private void Awake()
    {
        if(_identity == null)
            _identity = GetComponent<NetworkIdentity>();

        if (_carPhysic == null)
            _carPhysic = GetComponent<CarPhysic>();

        if (_inputServis == null)
            _inputServis = GetComponent<InputServis>();

        if (_respawn == null)
            _respawn = GetComponent<Respawn>();

        if (AI == null)
        {
            AI = GetComponent<BaseAIController>();
            if (AI == null)
                AI = GetComponentInChildren<BaseAIController>();
        }
    }

    private void Start()
    {
        if (gameObject.tag == ("Player"))
            IsPlayerControl = true;
        else if (gameObject.tag == "Enemy")
            IsEnamyControl = true;
    }
    void FixedUpdate()
    {

      
        if (IsEnamyControl)
        {
            if (!_identity.isServer) return;
            IsEnamy();
            return;
        }
       
        if (_identity == null || !_identity.isOwned) return;

        if (IsPlayerControl)
        {
            IsPlayer();
        }
    }
    private void IsPlayer()
    {
        if (_carPhysic == null || _inputServis == null || _respawn == null)
        {
            Debug.LogError($"❌ IsPlayer: Отсутствуют компоненты! CarPhysic: {_carPhysic}, InputServis: {_inputServis}, Respawn: {_respawn}");
            return;
        }
        _carPhysic.Move(_inputServis.VerticalInput,_inputServis.HorizontalInput, _inputServis.Brake);
       
        if (_inputServis.Respawn)
        {
            _respawn.Resp();
        }
    }

    private void IsEnamy()
    {
        if (AI == null)
        {
            Debug.LogWarning("AI controller is not assigned!");
            return;
        }
       // Debug.Log((AI.VerticalInput, AI.HorizontalInput, AI.Brake));
        _carPhysic.Move(AI.VerticalInput, AI.HorizontalInput, AI.Brake);
    }


}
