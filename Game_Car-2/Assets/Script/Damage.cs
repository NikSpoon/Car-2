using Unity.Cinemachine;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] public int _damage;



    private void OnCollisionEnter(Collision collision)
    {
       
        if (((1 << collision.gameObject.layer) & _layerMask) != 0)
        {
            var healthcomponent = collision.gameObject.GetComponent<Health>();
            if (healthcomponent)
            {
                healthcomponent.Damage(_damage);
            }
        }
        if (gameObject.tag == "Buulet")
            Destroy(gameObject);

    }
}
