using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField] float maxHealth = 100;
    float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0) { currentHealth = 0; Die(); }
        if(currentHealth > maxHealth) { currentHealth = maxHealth; }
        BroadcastMessage("DamageTaken", SendMessageOptions.DontRequireReceiver);
    }


    public void Die()
    {
        Debug.Log("unit is dead");
        BroadcastMessage("Dead", SendMessageOptions.DontRequireReceiver);
    }
}
