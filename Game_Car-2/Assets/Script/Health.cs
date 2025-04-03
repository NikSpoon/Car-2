using System;
using Unity.VisualScripting;
using UnityEngine;

public class Health : MonoBehaviour
{
    
    [SerializeField] private int _maxHealth;
    public int CurrentHeath { get; private set; }
    private bool _isPlayer = false;

    public event Action<int, int> OnHealthChanged;
    
    private void Start()
    {
         if (gameObject.tag == "Player")
        _isPlayer = true;

        CurrentHeath = _maxHealth;
    }

    public void Damage(int damade)
    {
        Debug.Log(damade);
        if (CurrentHeath >= 0)
            CurrentHeath -= damade;
        else
            OnDie();
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
