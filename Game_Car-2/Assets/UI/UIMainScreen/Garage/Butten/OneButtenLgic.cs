using UnityEngine;

public class OneButtenLgic : MonoBehaviour
{
    [SerializeField] private GameObject This;

    public void OnPressed()
    {
        if (This.activeSelf)
            This.SetActive(false);
        else
            This.SetActive(true);
    }
}
