 using UnityEngine;

public class InputServis : MonoBehaviour
{
    public Vector3 Direction { get; private set; } 
    public float VerticalInput { get; private set; }
    public float HorizontalInput { get; private set; }
   
    public bool Respawn { get; private set; }
    public bool Brake { get; private set; }

    public bool Exit { get; private set; }
    private void Awake()
    {
        Debug.Log("InputServis: Я создан на объекте " + gameObject.name);
    }
    void Update()
    {

        VerticalInput = Input.GetAxis("Vertical");
        HorizontalInput = Input.GetAxis("Horizontal");

        Brake = Input.GetKey(KeyCode.Space);

        Respawn = Input.GetKey(KeyCode.R);

        Direction = new Vector3(VerticalInput, 0, HorizontalInput);

        Exit = Input.GetKey(KeyCode.Escape);
    }
}
