using UnityEngine;

public class CarGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
     public void ActivePanel(bool setBool)
    {
        _panel.SetActive(setBool);
    }
}
