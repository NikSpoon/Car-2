using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;
    [SerializeField] private InputServis input;
    private GameObject lastPanel;
    private void Update()
    {
        if (input.Exit)
        {
            lastPanel = LastPanel(panels);
            lastPanel.SetActive(false);
        }
    }
    private GameObject LastPanel(GameObject[] panels)
    {

        for (int i = panels.Length - 1; i >= 0; i--)
        {
            if (panels[i] != null && panels[i].activeSelf)
            {
                return panels[i];
            }
        }
        return null;
    }
}
