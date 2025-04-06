using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManuControl : MonoBehaviour
{
    [SerializeField] private GameObject _options;
    [SerializeField] private GameObject _save;
    [SerializeField] private GameObject _Multiplaer;
     
     public void OnClickPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OnClickOptions()
    {
        if (_save.activeSelf != true && _Multiplaer.activeSelf != true)
        {
            if (_options.activeSelf)
                _options.SetActive(false);
            else
                _options.SetActive(true);
        }
    }

    public void OnClickSave()
    {
        if(_options.activeSelf != true && _Multiplaer.activeSelf != true)
        {

        if (_save.activeSelf)
            _save.SetActive(false);
        else
            _save.SetActive(true);
        }
    }

    public void OnClickMultiplaer()
    {
        if (_save.activeSelf != true && _options.activeSelf != true)
        {
            if (_Multiplaer.activeSelf)
                _Multiplaer.SetActive(false);
            else
                _Multiplaer.SetActive(true);
        }
    }

    public void OnClickExit()
    {
        
    }

}
