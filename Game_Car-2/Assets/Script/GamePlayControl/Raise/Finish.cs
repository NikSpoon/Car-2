using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private List<GameObject> _CarColliders = new List<GameObject>();

    [SerializeField] private string[] targetTags;
    private CarGamePanel _gamePanel;

    private void Start()
    {
        StartCoroutine(UpdateCarCollidersRoutine());
    }

    private IEnumerator UpdateCarCollidersRoutine()
    {
        while (true)
        {
            _CarColliders.Clear();
            foreach (string tag in targetTags)
            {
                GameObject[] cars = GameObject.FindGameObjectsWithTag(tag);
               // Debug.Log($"Обновлено: найдено {cars.Length} объектов с тегом {tag}");
                _CarColliders.AddRange(cars);
            }
            yield return new WaitForSeconds(20f);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: объект с тегом {other.tag} вошел в триггер");
        foreach (var collider in _CarColliders)
        {
            Debug.Log($"Проверяем тег {collider.tag}");
            if (other.tag == collider.tag)
            {
                _gamePanel = other.gameObject.GetComponent<CarGamePanel>();
                if (_gamePanel != null)
                {
                    _gamePanel.ActivePanel(true);

                    break;
                }
                
            }
        }
    }
}