using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    private GameObject _car;
    [SerializeField] private GameObject _Bullet;
    [SerializeField] private TargetRange _target;

    [SerializeField] private Transform _gun;
    [SerializeField] private Transform _gunPointer;
    
    [SerializeField] private float delley;
    private float lastShotTime;
   



    void Start()
    {
        _car = GameObject.FindGameObjectWithTag("Player");
    }

    
    void Update()
    {

        LookAt();
        
        if (_target.isInRange)
            {
            
            if (Time.time >= lastShotTime + delley)
                {
                    Shot();
                    lastShotTime = Time.time;
                }
            }
        
    }
 
    private void LookAt()
    {
        _gun.rotation = Quaternion.LookRotation(_car.transform.position - _gun.position);
    }
    private void Shot()
    {

        GameObject bullet = Instantiate(_Bullet, _gunPointer.position,_gunPointer.rotation);
        var mov = bullet.GetComponent<Movement>();
        mov.direction = _gunPointer.forward;
        StartCoroutine(DelleyShot());
    }

    private IEnumerator DelleyShot()
    {
        
        yield return new WaitForSeconds(delley);

    }
  
   
}
