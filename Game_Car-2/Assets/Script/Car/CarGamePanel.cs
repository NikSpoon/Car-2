using Mirror;
using UnityEngine;

public class CarGamePanel : NetworkBehaviour
{
    [SerializeField] private GameObject _panel;

     public void ActivePanel(bool setBool)
    {
        if (isOwned)
        {
            _panel.SetActive(setBool);
        }
      
    }
}
