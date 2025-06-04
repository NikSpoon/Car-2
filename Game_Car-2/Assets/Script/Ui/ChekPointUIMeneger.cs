using System.Collections;
using TMPro;
using UnityEngine;

public class ChekPointUIMeneger : MonoBehaviour
{
    private RaiseChekpoint _chekpointMeneger;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float time = 2f;

    private void Start()
    {
        _chekpointMeneger = FindAnyObjectByType<RaiseChekpoint>();

        gameObject.SetActive(false);
        _chekpointMeneger.OnChekPointChenge += OnChekPointChenge;

    }
    private void OnDisable()
    {
        _chekpointMeneger.OnChekPointChenge -= OnChekPointChenge;
    }

    private void OnChekPointChenge(int index, int rem)
    {
        _text.text = $"Chepoint {index}. left {rem}.";
        gameObject.SetActive(true);
        Time();
        gameObject.SetActive(false);
    }
    private IEnumerator Time()
    {
        yield return new WaitForSeconds(time);
        
    }
}
