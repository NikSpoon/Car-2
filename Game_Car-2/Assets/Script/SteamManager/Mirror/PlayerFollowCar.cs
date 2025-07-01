using Mirror;
using UnityEngine;

public class PlayerFollowCar : MonoBehaviour
{
    
    private Transform bodyTransform; 

    public void FindRoot(Transform root)
    {
        bodyTransform = root;
    }

    private void Update()
    {
        if (PlayerDataManager.Instance.AppSystem.CurrentState == FSM.App.AppState.Gameplay)
        {
            if (bodyTransform != null)
            {
                // Привязываем позицию и поворот объекта с этим скриптом к телу машины
                transform.position = bodyTransform.position;
                transform.rotation = bodyTransform.rotation;
            }
            
        }
       
    }
}