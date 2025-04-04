using UnityEngine;
using System;


public class Health : MonoBehaviour
{

    [SerializeField] private int _maxHealth;
    public int CurrentHeath { get; private set; }
    private bool _isPlayer = false;

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {

        if (gameObject.tag == "Player")
            _isPlayer = true;

        CurrentHeath = _maxHealth;

    }

    public void Damage(int damade)
    {
        Debug.Log(damade);

        if (CurrentHeath < 0)
            OnDie();

        CurrentHeath -= damade;
        OnHealthChanged?.Invoke(CurrentHeath, _maxHealth);
    }
    private void OnDie()
    {
        if (_isPlayer)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }
}
