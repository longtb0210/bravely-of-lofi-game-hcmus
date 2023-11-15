using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Stat strength;
    public Stat damage;
    public Stat maxHealth;

    [SerializeField] public float currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
    }

    public virtual void DoDamge(CharacterStats _targetStats)
    {
        int totalDamge = damage.GetValue() + strength.GetValue();

        _targetStats.TakeDamge(totalDamge);
    }

    public virtual void TakeDamge(int _damage)
    {
        currentHealth -= _damage;

        if (currentHealth < 0)
            Die();
    }

    protected virtual void Die()
    {
        
    }
}
