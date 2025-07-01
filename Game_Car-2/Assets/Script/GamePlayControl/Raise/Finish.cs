using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private List<GameObject> _CarColliders = new List<GameObject>();

    [SerializeField] private string[] targetTags;

    [SerializeField] private Canvas _finishUI;

    private void Awake()
    {
        if (_finishUI == null)
        {
            Debug.LogError("_finishUI не назначен!");
        }
        else
        {
            _finishUI.gameObject.SetActive(false);
            Debug.Log("_finishUI отключён");
        }
    }

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
                Debug.Log("Теги совпали! Показываем UI.");
                _finishUI.gameObject.SetActive(true);
                break;
            }
        }
    }
}