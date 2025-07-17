using UnityEngine;

namespace MetaEarthStrike
{
    public class UnitController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        
        [Header("Combat")]
        public float attackRange = 2f;
        public float attackDamage = 10f;
        public float attackCooldown = 1f;
        
        private Transform currentTarget;
        private float lastAttackTime;
        private bool isMoving = true;

        private void Start()
        {
            // Find the enemy base if we don't have a target
            if (currentTarget == null)
            {
                var enemyBase = GameObject.Find("EnemyBase");
                if (enemyBase != null)
                {
                    currentTarget = enemyBase.transform;
                }
            }
        }

        private void Update()
        {
            if (currentTarget == null) return;

            if (isMoving)
            {
                // Calculate direction to enemy base
                Vector3 direction = (currentTarget.position - transform.position).normalized;
                
                // Move towards enemy base
                transform.Translate(direction * moveSpeed * Time.deltaTime);

                // Check if we're in attack range
                float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
                if (distanceToTarget <= attackRange)
                {
                    isMoving = false;
                }
            }
            else
            {
                Attack();
            }
        }

        private void Attack()
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                
                // Get enemy health component and deal damage
                var enemyHealth = currentTarget.GetComponent<HealthSystem>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize attack range in editor
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}