using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaEarthStrike.Core
{
    public enum UnitType
    {
        Melee,
        Ranged,
        Siege
    }

    public class Unit : MonoBehaviour
    {
        [Header("Unit Stats")]
        public string unitName = "Unit";
        public UnitType unitType = UnitType.Melee;
        public int maxHealth = 100;
        public int currentHealth;
        public int damage = 20;
        public float attackRange = 1f;
        public float attackSpeed = 1f;
        public float moveSpeed = 3f;
        
        [Header("Combat")]
        public float lastAttackTime;
        public Unit currentTarget;
        public bool isAttacking;
        
        [Header("Movement")]
        public List<Transform> waypoints = new List<Transform>();
        public int currentWaypointIndex = 0;
        public float waypointReachDistance = 0.5f;
        
        [Header("Lane Info")]
        public Lane currentLane;
        public bool isPlayerUnit;
        
        [Header("Visual")]
        public SpriteRenderer spriteRenderer;
        public Animator animator;
        public GameObject healthBarPrefab;
        private GameObject healthBar;
        
        // Events
        public System.Action<Unit> OnUnitDeath;
        public System.Action<Unit> OnUnitReachedBase;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }
        
        public void Initialize(Lane lane, bool isPlayer, List<Transform> waypoints)
        {
            currentLane = lane;
            isPlayerUnit = isPlayer;
            this.waypoints = new List<Transform>(waypoints);
            
            // Reverse waypoints for enemy units
            if (!isPlayer)
            {
                this.waypoints.Reverse();
            }
            
            currentHealth = maxHealth;
            currentWaypointIndex = 0;
            lastAttackTime = 0f;
            currentTarget = null;
            isAttacking = false;
            
            // Set visual direction
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !isPlayer;
                spriteRenderer.color = isPlayer ? Color.blue : Color.red;
            }
            
            // Create health bar
            CreateHealthBar();
            
            // Start movement
            StartCoroutine(MoveAlongWaypoints());
        }
        
        private void Update()
        {
            if (GameManager.Instance.currentGameState != GameState.Gameplay)
                return;
                
            UpdateCombat();
            UpdateHealthBar();
        }
        
        private void UpdateCombat()
        {
            if (isAttacking && currentTarget != null)
            {
                // Check if target is still in range
                float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
                if (distanceToTarget > attackRange)
                {
                    // Target moved out of range, resume movement
                    isAttacking = false;
                    currentTarget = null;
                    return;
                }
                
                // Attack if cooldown is ready
                if (Time.time - lastAttackTime >= 1f / attackSpeed)
                {
                    Attack(currentTarget);
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // Look for enemies in range
                FindTarget();
            }
        }
        
        private void FindTarget()
        {
            if (currentLane == null) return;
            
            List<Unit> enemiesInLane = new List<Unit>();
            foreach (Unit unit in currentLane.activeUnits)
            {
                if (unit.isPlayerUnit != isPlayerUnit && unit.currentHealth > 0)
                {
                    enemiesInLane.Add(unit);
                }
            }
            
            // Find closest enemy in attack range
            Unit closestEnemy = null;
            float closestDistance = attackRange;
            
            foreach (Unit enemy in enemiesInLane)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance <= attackRange && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
            
            if (closestEnemy != null)
            {
                currentTarget = closestEnemy;
                isAttacking = true;
            }
        }
        
        private void Attack(Unit target)
        {
            if (target == null || target.currentHealth <= 0) return;
            
            // Deal damage
            target.TakeDamage(damage);
            
            // Play attack animation
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // Check if target died
            if (target.currentHealth <= 0)
            {
                currentTarget = null;
                isAttacking = false;
            }
        }
        
        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            
            // Play damage animation
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        private void Die()
        {
            // Remove from lane
            if (currentLane != null)
            {
                currentLane.activeUnits.Remove(this);
            }
            
            // Notify listeners
            OnUnitDeath?.Invoke(this);
            
            // Return to pool or destroy
            ObjectPool pool = FindFirstObjectByType<ObjectPool>();
            if (pool != null)
            {
                pool.ReturnObject(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private IEnumerator MoveAlongWaypoints()
        {
            while (currentWaypointIndex < waypoints.Count && currentHealth > 0)
            {
                if (!isAttacking)
                {
                    Transform targetWaypoint = waypoints[currentWaypointIndex];
                    Vector3 targetPosition = targetWaypoint.position;
                    
                    // Move towards waypoint
                    while (Vector3.Distance(transform.position, targetPosition) > waypointReachDistance)
                    {
                        if (isAttacking) break; // Stop moving if attacking
                        
                        Vector3 direction = (targetPosition - transform.position).normalized;
                        transform.position += direction * moveSpeed * Time.deltaTime;
                        
                        yield return null;
                    }
                    
                    // Reached waypoint
                    currentWaypointIndex++;
                }
                
                yield return null;
            }
            
            // Reached end of waypoints (enemy base)
            if (currentHealth > 0)
            {
                ReachEnemyBase();
            }
        }
        
        private void ReachEnemyBase()
        {
            // Deal damage to enemy base
            if (isPlayerUnit)
            {
                GameManager.Instance.DamageEnemyBase(damage);
            }
            else
            {
                GameManager.Instance.DamagePlayerBase(damage);
            }
            
            // Notify listeners
            OnUnitReachedBase?.Invoke(this);
            
            // Remove unit
            if (currentLane != null)
            {
                currentLane.activeUnits.Remove(this);
            }
            
            // Return to pool or destroy
            ObjectPool pool = FindFirstObjectByType<ObjectPool>();
            if (pool != null)
            {
                pool.ReturnObject(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void CreateHealthBar()
        {
            if (healthBarPrefab != null)
            {
                healthBar = Instantiate(healthBarPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
                healthBar.transform.SetParent(transform);
            }
        }
        
        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                // Update health bar fill
                UnityEngine.UI.Image healthBarFill = healthBar.GetComponentInChildren<UnityEngine.UI.Image>();
                if (healthBarFill != null)
                {
                    float healthPercentage = (float)currentHealth / maxHealth;
                    healthBarFill.fillAmount = healthPercentage;
                }
                
                // Update position to follow unit
                healthBar.transform.position = transform.position + Vector3.up * 1.5f;
            }
        }
        
        private void OnDestroy()
        {
            if (healthBar != null)
            {
                Destroy(healthBar);
            }
        }
    }
} 