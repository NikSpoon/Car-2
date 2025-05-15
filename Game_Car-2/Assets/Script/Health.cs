using UnityEngine;
using System;


public class Health : MonoBehaviour
{

    [SerializeField] private int _maxHealth;
    public int CurrentHealth { get; private set; }
    private bool _isPlayer = false;

    public event Action<int, int> OnHealthChanged;

    private void Awake()
    {

        if (gameObject.tag == "Player")
            _isPlayer = true;

        CurrentHealth = _maxHealth;

    }

    public void Damage(int damade)
    {
        Debug.Log(damade);

        if (CurrentHealth < 0)
            OnDie();

        CurrentHealth -= damade;
        OnHealthChanged?.Invoke(CurrentHealth, _maxHealth);
    }
    private void OnDie()
    {
        if (_isPlayer)
            gameObject.SetActive(false);
        else
            Destroy(gameObject);
    }
}
