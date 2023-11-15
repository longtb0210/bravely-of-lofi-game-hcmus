using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int damage;
    public float maxHealth;

    [SerializeField] public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamge(int _damage)
    {
        currentHealth -= _damage;   
    }
}
