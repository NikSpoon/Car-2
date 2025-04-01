 using UnityEngine;

public class InputServis : MonoBehaviour
{
    public Vector3 Direction { get; private set; } 
    public float VerticalInput { get; private set; }
    public float HorizontalInput { get; private set; }
   
    public bool Brake { get; private set; }
    void Update()
    {

        VerticalInput = Input.GetAxis("Vertical");
        HorizontalInput = Input.GetAxis("Horizontal");

        Brake = Input.GetKey(KeyCode.Space);
        

        Direction = new Vector3(VerticalInput, 0, HorizontalInput);

    }
}
