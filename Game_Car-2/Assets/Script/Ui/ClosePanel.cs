using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ClosePanel : MonoBehaviour
{

    public static ClosePanel Instance { get; set; }

    [SerializeField] private GameObject[] panels;
    [SerializeField] private InputServis input;
    private GameObject lastPanel;

    public List<GameObject> openPanels = new List<GameObject>();
    private void Update()
    {
        if (input.Exit && openPanels.Count > 0)
        {
            StartCoroutine(DelayRemoveLastPanel());
        }
        foreach (var panel in panels)
        {
             if (panel.activeSelf && !openPanels.Contains(panel))
                    openPanels.Add(panel);
        }
    }

    private IEnumerator DelayRemoveLastPanel()
    {
        lastPanel = openPanels[openPanels.Count - 1];
        lastPanel.SetActive(false);

        yield return new WaitForSeconds(1f);

        if (openPanels.Count > 0)
        {
            openPanels.RemoveAt(openPanels.Count - 1);
        }
    }
}
