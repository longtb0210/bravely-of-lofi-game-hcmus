using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Stat damage;
    public Stat maxHealth;

    [SerializeField] public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth.GetValue();

        damage.AddModifier(5);
    }

    public void TakeDamge(int _damage)
    {
        currentHealth -= _damage;

        if (currentHealth < 0)
            Die();
    }

    private void Die()
    {
        
    }
}
