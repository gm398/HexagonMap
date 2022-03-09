using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField] float maxHealth = 100;
    [SerializeField] float currentHealth;
    [SerializeField] float armor = 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    [ContextMenu("Damage")]
    void take5damage() { TakeDamage(20); }

    public void TakeDamage(float damage)
    {
        damage -= armor;
        if(damage < 1) { damage = 1; }
        currentHealth -= damage;
        if(currentHealth <= 0) { currentHealth = 0; Die(); }
        if(currentHealth > maxHealth) { currentHealth = maxHealth; }
        BroadcastMessage("DamageTaken", SendMessageOptions.DontRequireReceiver);
    }


    public void Die()
    {
        
        BroadcastMessage("Dead", SendMessageOptions.DontRequireReceiver);
    }


    public float GetMaxHealth() { return maxHealth; }
    public float GetCurrentHealth() { return currentHealth; }
}
