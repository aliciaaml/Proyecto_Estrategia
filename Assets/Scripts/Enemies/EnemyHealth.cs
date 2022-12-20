using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth;
    public float currentHealth;
    HealthBar healthBar;

    void Start()
    {
        healthBar = gameObject.GetComponentInChildren<HealthBar>();
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Test.isIATurn)
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(20);
        }
    }

    void Update()
    {
        if (currentHealth <= 0) gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.setHealth(currentHealth);
    }
}
