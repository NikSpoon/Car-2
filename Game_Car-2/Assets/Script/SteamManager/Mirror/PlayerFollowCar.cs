using Mirror;
using System.Security.Principal;
using UnityEngine;

public class PlayerFollowCar : NetworkBehaviour
{
    
    private Transform bodyTransform;
    
    [SerializeField] private float positionSmoothSpeed = 5f;
    [SerializeField] private float rotationSmoothSpeed = 5f;
    private NetworkIdentity _identity;
    public void FindRoot(Transform root)
    {
        bodyTransform = root;
        
    }
    private void FindIdentity()
    {
        if (bodyTransform == null)
        {

            return;
        }

        Transform current = bodyTransform;
        while (current != null)
        {
            NetworkIdentity identity = current.GetComponent<NetworkIdentity>();
            if (identity != null)
            {
                _identity = identity;
                Debug.Log("NetworkIdentity найден на объекте: " + current.name);
                return;
            }
            current = current.parent;
        }

        Debug.LogWarning("NetworkIdentity не найден в родителях bodyTransform!");
    }
    private void Update()
    {
        if (PlayerDataManager.Instance.AppSystem.CurrentState == FSM.App.AppState.Gameplay)
        {
            if (_identity == null)
            {
                FindIdentity();
                return;
            }
           
            if (!_identity.isOwned) return;
            
            if (bodyTransform != null)
            {
                transform.position = Vector3.Lerp(transform.position, bodyTransform.position, positionSmoothSpeed * Time.deltaTime);

                transform.rotation = Quaternion.Slerp(transform.rotation, bodyTransform.rotation, rotationSmoothSpeed * Time.deltaTime);
            }

        }
       
    }
}