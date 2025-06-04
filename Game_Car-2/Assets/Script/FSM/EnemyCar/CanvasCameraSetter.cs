
using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    private Camera targetCamera;

    private void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            canvas.worldCamera = Camera.main;
            targetCamera = Camera.main;
        }
    }


    private void LateUpdate()
    {
        if (targetCamera != null)
        {
            transform.forward = targetCamera.transform.forward;

        }
    }


}

