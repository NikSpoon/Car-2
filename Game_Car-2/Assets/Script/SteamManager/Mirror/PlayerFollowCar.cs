using Mirror;
using UnityEngine;

public class PlayerFollowCar : NetworkBehaviour
{
    
    private Transform bodyTransform;
    
    [SerializeField] private float positionSmoothSpeed = 5f;
    [SerializeField] private float rotationSmoothSpeed = 5f;
    public void FindRoot(Transform root)
    {
        bodyTransform = root;
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (PlayerDataManager.Instance.AppSystem.CurrentState == FSM.App.AppState.Gameplay)
        {
            if (bodyTransform != null)
            {
                transform.position = Vector3.Lerp(transform.position, bodyTransform.position, positionSmoothSpeed * Time.deltaTime);

                transform.rotation = Quaternion.Slerp(transform.rotation, bodyTransform.rotation, rotationSmoothSpeed * Time.deltaTime);
            }

        }
       
    }
}