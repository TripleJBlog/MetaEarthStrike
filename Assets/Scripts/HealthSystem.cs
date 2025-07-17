using UnityEngine;
using UnityEngine.Events;

namespace MetaEarthStrike
{
    public class HealthSystem : MonoBehaviour
    {
        public float maxHealth = 100f;
        public float currentHealth;
        
        public UnityEvent onDeath;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            onDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}