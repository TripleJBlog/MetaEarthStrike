using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaEarthStrike.Core
{
    [System.Serializable]
    public class HeroAbility
    {
        public string abilityName;
        public string description;
        public float cooldown;
        public int manaCost;
        public float range;
        public int damage;
        public AbilityType abilityType;
        public GameObject effectPrefab;
        
        [HideInInspector]
        public float lastUseTime;
        
        public enum AbilityType
        {
            Damage,
            Heal,
            Buff,
            AoE
        }
    }

    public class HeroManager : MonoBehaviour
    {
        public static HeroManager Instance { get; private set; }
        [Header("Hero Stats")]
        public GameObject heroPrefab;
        public string heroName = "Hero";
        public int maxHealth = 200;
        public int currentHealth;
        public int maxMana = 100;
        public int currentMana;
        public int level = 1;
        public int experience = 0;
        public int experienceToNextLevel = 100;
        
        [Header("Combat")]
        public int damage = 30;
        public float attackRange = 2f;
        public float attackSpeed = 1f;
        public float moveSpeed = 5f;
        
        [Header("Abilities")]
        public HeroAbility[] abilities = new HeroAbility[4];
        
        [Header("Hero Instance")]
        public GameObject heroInstance;
        public Unit heroUnit;
        
        [Header("Mana Regeneration")]
        public float manaRegenRate = 5f; // mana per second
        public float manaRegenTimer;
        
        private GameManager gameManager;
        private LaneManager laneManager;
        private Camera mainCamera;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
            laneManager = LaneManager.Instance != null ? LaneManager.Instance : FindFirstObjectByType<LaneManager>();
            mainCamera = Camera.main;
            
            InitializeAbilities();
        }
        
        private void Start()
        {
            InitializeHero();
        }
        
        private void Update()
        {
            if (GameManager.Instance.currentGameState == GameState.Gameplay && heroInstance != null)
            {
                HandleHeroInput();
                UpdateManaRegeneration();
            }
        }
        
        public void Initialize(FactionType faction)
        {
            // Set hero stats based on faction
            switch (faction)
            {
                case FactionType.Alliance:
                    heroName = "Paladin";
                    maxHealth = 250;
                    maxMana = 120;
                    damage = 35;
                    break;
                case FactionType.Horde:
                    heroName = "Warlord";
                    maxHealth = 300;
                    maxMana = 80;
                    damage = 40;
                    break;
                case FactionType.Scourge:
                    heroName = "Necromancer";
                    maxHealth = 180;
                    maxMana = 150;
                    damage = 25;
                    break;
            }
            
            currentHealth = maxHealth;
            currentMana = maxMana;
        }
        
        private void InitializeAbilities()
        {
            // Initialize default abilities
            abilities[0] = new HeroAbility
            {
                abilityName = "Basic Attack",
                description = "Deal damage to target enemy",
                cooldown = 1f,
                manaCost = 0,
                range = attackRange,
                damage = damage,
                abilityType = HeroAbility.AbilityType.Damage
            };
            
            abilities[1] = new HeroAbility
            {
                abilityName = "Heal",
                description = "Restore health to self or ally",
                cooldown = 8f,
                manaCost = 30,
                range = 5f,
                damage = -50, // Negative for healing
                abilityType = HeroAbility.AbilityType.Heal
            };
            
            abilities[2] = new HeroAbility
            {
                abilityName = "Power Strike",
                description = "Deal heavy damage to single target",
                cooldown = 12f,
                manaCost = 40,
                range = attackRange,
                damage = damage * 2,
                abilityType = HeroAbility.AbilityType.Damage
            };
            
            abilities[3] = new HeroAbility
            {
                abilityName = "Battle Cry",
                description = "Buff nearby allies",
                cooldown = 15f,
                manaCost = 50,
                range = 8f,
                damage = 0,
                abilityType = HeroAbility.AbilityType.Buff
            };
        }
        
        private void InitializeHero()
        {
            if (heroPrefab != null && laneManager != null)
            {
                // Spawn hero in the single lane
                Lane lane = laneManager.GetLaneByIndex(0);
                if (lane != null)
                {
                    Vector3 spawnPosition = lane.playerSpawnPoint.position;
                    heroInstance = Instantiate(heroPrefab, spawnPosition, Quaternion.identity);
                    
                    // Add hero unit component
                    heroUnit = heroInstance.GetComponent<Unit>();
                    if (heroUnit == null)
                    {
                        heroUnit = heroInstance.AddComponent<Unit>();
                    }
                    
                    // Initialize hero unit with special stats
                    heroUnit.unitName = heroName;
                    heroUnit.maxHealth = maxHealth;
                    heroUnit.currentHealth = currentHealth;
                    heroUnit.damage = damage;
                    heroUnit.attackRange = attackRange;
                    heroUnit.attackSpeed = attackSpeed;
                    heroUnit.moveSpeed = moveSpeed;
                    
                    // Add hero to lane
                    lane.activeUnits.Add(heroUnit);
                    heroUnit.currentLane = lane;
                    heroUnit.isPlayerUnit = true;
                }
            }
        }
        
        private void HandleHeroInput()
        {
            // Handle ability usage (keyboard for testing, UI buttons for mobile)
            if (Input.GetKeyDown(KeyCode.Q))
                UseAbility(0);
            if (Input.GetKeyDown(KeyCode.W))
                UseAbility(1);
            if (Input.GetKeyDown(KeyCode.E))
                UseAbility(2);
            if (Input.GetKeyDown(KeyCode.R))
                UseAbility(3);
            
            // Handle hero movement (touch/click to move)
            if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                HandleHeroMovement();
            }
        }
        
        private void HandleHeroMovement()
        {
            if (heroInstance == null) return;
            
            Vector3 inputPosition;
            
            // Handle touch input
            if (Input.touchCount > 0)
            {
                inputPosition = Input.GetTouch(0).position;
            }
            else
            {
                inputPosition = Input.mousePosition;
            }
            
            Ray ray = mainCamera.ScreenPointToRay(inputPosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                // Move hero to touched/clicked position
                Vector3 targetPosition = hit.point;
                StartCoroutine(MoveHeroToPosition(targetPosition));
            }
        }
        
        private IEnumerator MoveHeroToPosition(Vector3 targetPosition)
        {
            while (Vector3.Distance(heroInstance.transform.position, targetPosition) > 0.5f)
            {
                Vector3 direction = (targetPosition - heroInstance.transform.position).normalized;
                heroInstance.transform.position += direction * moveSpeed * Time.deltaTime;
                yield return null;
            }
        }
        
        public void UseAbility(int abilityIndex)
        {
            if (abilityIndex < 0 || abilityIndex >= abilities.Length)
                return;
                
            HeroAbility ability = abilities[abilityIndex];
            
            // Check cooldown
            if (Time.time - ability.lastUseTime < ability.cooldown)
                return;
                
            // Check mana cost
            if (currentMana < ability.manaCost)
                return;
                
            // Use ability
            currentMana -= ability.manaCost;
            ability.lastUseTime = Time.time;
            
            // Apply ability effect
            ApplyAbilityEffect(ability);
        }
        
        private void ApplyAbilityEffect(HeroAbility ability)
        {
            switch (ability.abilityType)
            {
                case HeroAbility.AbilityType.Damage:
                    // Find target and deal damage
                    Unit target = FindNearestEnemy();
                    if (target != null)
                    {
                        target.TakeDamage(ability.damage);
                        GainExperience(10);
                    }
                    break;
                    
                case HeroAbility.AbilityType.Heal:
                    // Heal self or nearby ally
                    currentHealth = Mathf.Min(currentHealth - ability.damage, maxHealth);
                    break;
                    
                case HeroAbility.AbilityType.Buff:
                    // Buff nearby allies
                    BuffNearbyAllies();
                    break;
                    
                case HeroAbility.AbilityType.AoE:
                    // Area of effect damage
                    DealAoEDamage(ability);
                    break;
            }
            
            // Spawn effect if available
            if (ability.effectPrefab != null)
            {
                Instantiate(ability.effectPrefab, heroInstance.transform.position, Quaternion.identity);
            }
        }
        
        private Unit FindNearestEnemy()
        {
            if (heroUnit == null || heroUnit.currentLane == null)
                return null;
                
            Unit nearestEnemy = null;
            float nearestDistance = float.MaxValue;
            
            foreach (Unit unit in heroUnit.currentLane.activeUnits)
            {
                if (unit.isPlayerUnit != heroUnit.isPlayerUnit && unit.currentHealth > 0)
                {
                    float distance = Vector3.Distance(heroInstance.transform.position, unit.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = unit;
                    }
                }
            }
            
            return nearestEnemy;
        }
        
        private void BuffNearbyAllies()
        {
            if (heroUnit == null || heroUnit.currentLane == null)
                return;
                
            foreach (Unit unit in heroUnit.currentLane.activeUnits)
            {
                if (unit.isPlayerUnit == heroUnit.isPlayerUnit && unit != heroUnit)
                {
                    float distance = Vector3.Distance(heroInstance.transform.position, unit.transform.position);
                    if (distance <= 8f)
                    {
                        // Apply buff effect (increase damage temporarily)
                        unit.damage += 10;
                        StartCoroutine(RemoveBuffAfterTime(unit, 10f));
                    }
                }
            }
        }
        
        private IEnumerator RemoveBuffAfterTime(Unit unit, float duration)
        {
            yield return new WaitForSeconds(duration);
            unit.damage -= 10;
        }
        
        private void DealAoEDamage(HeroAbility ability)
        {
            if (heroUnit == null || heroUnit.currentLane == null)
                return;
                
            foreach (Unit unit in heroUnit.currentLane.activeUnits)
            {
                if (unit.isPlayerUnit != heroUnit.isPlayerUnit && unit.currentHealth > 0)
                {
                    float distance = Vector3.Distance(heroInstance.transform.position, unit.transform.position);
                    if (distance <= ability.range)
                    {
                        unit.TakeDamage(ability.damage);
                    }
                }
            }
        }
        
        private void UpdateManaRegeneration()
        {
            manaRegenTimer += Time.deltaTime;
            if (manaRegenTimer >= 1f)
            {
                currentMana = Mathf.Min(currentMana + Mathf.RoundToInt(manaRegenRate), maxMana);
                manaRegenTimer = 0f;
            }
        }
        
        public void GainExperience(int amount)
        {
            experience += amount;
            
            // Check for level up
            while (experience >= experienceToNextLevel)
            {
                LevelUp();
            }
        }
        
        private void LevelUp()
        {
            experience -= experienceToNextLevel;
            level++;
            
            // Increase stats
            maxHealth += 20;
            maxMana += 10;
            damage += 5;
            
            // Restore health and mana
            currentHealth = maxHealth;
            currentMana = maxMana;
            
            // Increase experience requirement for next level
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f);
            
            // Update hero unit stats
            if (heroUnit != null)
            {
                heroUnit.maxHealth = maxHealth;
                heroUnit.currentHealth = currentHealth;
                heroUnit.damage = damage;
            }
            
            Debug.Log($"{heroName} reached level {level}!");
        }
        
        public void TakeDamage(int damageAmount)
        {
            currentHealth -= damageAmount;
            
            if (heroUnit != null)
            {
                heroUnit.currentHealth = currentHealth;
            }
            
            if (currentHealth <= 0)
            {
                // Hero died
                Debug.Log($"{heroName} has fallen!");
                // Could respawn after some time or end the game
            }
        }
        
        public float GetAbilityCooldown(int abilityIndex)
        {
            if (abilityIndex < 0 || abilityIndex >= abilities.Length)
                return 0f;
                
            HeroAbility ability = abilities[abilityIndex];
            float timeSinceLastUse = Time.time - ability.lastUseTime;
            return Mathf.Max(0f, ability.cooldown - timeSinceLastUse);
        }
        
        public bool CanUseAbility(int abilityIndex)
        {
            if (abilityIndex < 0 || abilityIndex >= abilities.Length)
                return false;
                
            HeroAbility ability = abilities[abilityIndex];
            return Time.time - ability.lastUseTime >= ability.cooldown && currentMana >= ability.manaCost;
        }
    }
} 