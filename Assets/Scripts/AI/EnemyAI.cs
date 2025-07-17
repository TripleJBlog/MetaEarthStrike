using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MetaEarthStrike.Core; // Required for GameManager, EconomyManager, LaneManager, HeroManager, etc.

namespace MetaEarthStrike.AI
{
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI Settings")]
        public float decisionInterval = 2f;
        public float upgradeChance = 0.3f;
        public float aggressiveChance = 0.4f;
        
        [Header("Strategy")]
        public AIStrategy currentStrategy = AIStrategy.Balanced;
        public float strategyChangeInterval = 30f;
        
        [Header("Targeting")]
        public bool prioritizeHero = true;
        public bool focusWeakestLane = true;
        
        private GameManager gameManager;
        private EconomyManager economyManager;
        private LaneManager laneManager;
        private HeroManager heroManager;
        
        private float lastDecisionTime;
        private float lastStrategyChangeTime;
        private float matchStartTime;
        
        public enum AIStrategy
        {
            Aggressive,    // Focus on damage and pushing
            Defensive,     // Focus on base defense and income
            Balanced,      // Mix of both
            Economic,      // Focus on economy first
            Rush          // Early game aggression
        }
        
        private void Awake()
        {
            gameManager = GameManager.Instance;
            economyManager = EconomyManager.Instance;
            laneManager = FindFirstObjectByType<LaneManager>();
            heroManager = FindFirstObjectByType<HeroManager>();
        }
        
        private void Start()
        {
            matchStartTime = Time.time;
            lastDecisionTime = Time.time;
            lastStrategyChangeTime = Time.time;
            
            // Start with balanced strategy
            currentStrategy = AIStrategy.Balanced;
        }
        
        private void Update()
        {
            if (GameManager.Instance.currentGameState == GameState.Gameplay)
            {
                UpdateAI();
            }
        }
        
        private void UpdateAI()
        {
            // Update strategy periodically
            if (Time.time - lastStrategyChangeTime >= strategyChangeInterval)
            {
                UpdateStrategy();
                lastStrategyChangeTime = Time.time;
            }
            
            // Make decisions periodically
            if (Time.time - lastDecisionTime >= decisionInterval)
            {
                MakeDecision();
                lastDecisionTime = Time.time;
            }
        }
        
        private void UpdateStrategy()
        {
            float matchTime = Time.time - matchStartTime;
            float playerBaseHealthPercent = (float)gameManager.playerBaseHealth / 1000f;
            float enemyBaseHealthPercent = (float)gameManager.enemyBaseHealth / 1000f;
            
            // Early game (first 2 minutes)
            if (matchTime < 120f)
            {
                currentStrategy = AIStrategy.Rush;
            }
            // Mid game (2-8 minutes)
            else if (matchTime < 480f)
            {
                if (enemyBaseHealthPercent < 0.5f)
                {
                    currentStrategy = AIStrategy.Defensive;
                }
                else if (playerBaseHealthPercent < 0.7f)
                {
                    currentStrategy = AIStrategy.Aggressive;
                }
                else
                {
                    currentStrategy = AIStrategy.Balanced;
                }
            }
            // Late game (8+ minutes)
            else
            {
                if (enemyBaseHealthPercent < 0.3f)
                {
                    currentStrategy = AIStrategy.Defensive;
                }
                else if (playerBaseHealthPercent < 0.5f)
                {
                    currentStrategy = AIStrategy.Aggressive;
                }
                else
                {
                    currentStrategy = AIStrategy.Economic;
                }
            }
        }
        
        private void MakeDecision()
        {
            switch (currentStrategy)
            {
                case AIStrategy.Aggressive:
                    MakeAggressiveDecision();
                    break;
                case AIStrategy.Defensive:
                    MakeDefensiveDecision();
                    break;
                case AIStrategy.Balanced:
                    MakeBalancedDecision();
                    break;
                case AIStrategy.Economic:
                    MakeEconomicDecision();
                    break;
                case AIStrategy.Rush:
                    MakeRushDecision();
                    break;
            }
        }
        
        private void MakeAggressiveDecision()
        {
            // Focus on damage upgrades and pushing
            if (Random.Range(0f, 1f) < upgradeChance)
            {
                // Prioritize damage upgrades
                if (economyManager.enemyGold >= 100)
                {
                    PurchaseRandomUpgrade(new string[] { "Melee Units", "Ranged Units", "Siege Units" });
                }
            }
        }
        
        private void MakeDefensiveDecision()
        {
            // Focus on base defense and income
            if (Random.Range(0f, 1f) < upgradeChance)
            {
                if (economyManager.enemyGold >= 150)
                {
                    // Prioritize base defense and income
                    PurchaseRandomUpgrade(new string[] { "Base Defense", "Income Boost" });
                }
            }
        }
        
        private void MakeBalancedDecision()
        {
            // Mix of all upgrades
            if (Random.Range(0f, 1f) < upgradeChance)
            {
                string[] allUpgrades = { "Income Boost", "Melee Units", "Ranged Units", "Siege Units", "Base Defense" };
                PurchaseRandomUpgrade(allUpgrades);
            }
        }
        
        private void MakeEconomicDecision()
        {
            // Focus on economy first
            if (Random.Range(0f, 1f) < upgradeChance)
            {
                if (economyManager.enemyGold >= 50)
                {
                    // Prioritize income
                    if (Random.Range(0f, 1f) < 0.7f)
                    {
                        PurchaseUpgrade("Income Boost");
                    }
                    else
                    {
                        PurchaseRandomUpgrade(new string[] { "Melee Units", "Ranged Units" });
                    }
                }
            }
        }
        
        private void MakeRushDecision()
        {
            // Early game aggression
            if (Random.Range(0f, 1f) < upgradeChance * 1.5f) // More frequent decisions
            {
                if (economyManager.enemyGold >= 75)
                {
                    // Focus on early units
                    PurchaseRandomUpgrade(new string[] { "Melee Units", "Ranged Units" });
                }
            }
        }
        
        private void PurchaseRandomUpgrade(string[] upgradeNames)
        {
            if (upgradeNames.Length == 0) return;
            
            string upgradeName = upgradeNames[Random.Range(0, upgradeNames.Length)];
            PurchaseUpgrade(upgradeName);
        }
        
        private void PurchaseUpgrade(string upgradeName)
        {
            // Simulate enemy purchasing upgrades
            UpgradeData upgrade = economyManager.GetUpgradeData(upgradeName);
            if (upgrade != null && economyManager.enemyGold >= upgrade.cost && upgrade.level < upgrade.maxLevel)
            {
                economyManager.enemyGold -= upgrade.cost;
                upgrade.level++;
                
                // Apply upgrade effect for enemy
                ApplyEnemyUpgradeEffect(upgrade);
                
                Debug.Log($"Enemy purchased: {upgradeName} (Level {upgrade.level})");
            }
        }
        
        private void ApplyEnemyUpgradeEffect(UpgradeData upgrade)
        {
            switch (upgrade.upgradeName)
            {
                case "Income Boost":
                    economyManager.enemyIncome += 5;
                    break;
                case "Base Defense":
                    gameManager.enemyBaseHealth += 200;
                    break;
                // Unit upgrades would be handled by the spawning system
            }
        }
        
        public int GetWeakestLane()
        {
            if (laneManager == null) return 0; // Default to single lane
            
            // For single lane, always return 0
            return 0;
        }
        
        public int GetStrongestLane()
        {
            if (laneManager == null) return 0;
            
            // For single lane, always return 0
            return 0;
        }
        
        public bool ShouldFocusHero()
        {
            if (!prioritizeHero || heroManager == null || heroManager.heroInstance == null)
                return false;
                
            // Check if hero is low health
            float heroHealthPercent = (float)heroManager.currentHealth / heroManager.maxHealth;
            return heroHealthPercent < 0.5f;
        }
        
        public AIStrategy GetCurrentStrategy()
        {
            return currentStrategy;
        }
        
        public float GetStrategyEffectiveness()
        {
            float matchTime = Time.time - matchStartTime;
            float playerBaseHealthPercent = (float)gameManager.playerBaseHealth / 1000f;
            float enemyBaseHealthPercent = (float)gameManager.enemyBaseHealth / 1000f;
            
            // Calculate effectiveness based on current game state
            float effectiveness = 0f;
            
            switch (currentStrategy)
            {
                case AIStrategy.Aggressive:
                    effectiveness = (1f - playerBaseHealthPercent) * 100f;
                    break;
                case AIStrategy.Defensive:
                    effectiveness = enemyBaseHealthPercent * 100f;
                    break;
                case AIStrategy.Balanced:
                    effectiveness = (enemyBaseHealthPercent + (1f - playerBaseHealthPercent)) * 50f;
                    break;
                case AIStrategy.Economic:
                    effectiveness = (economyManager.enemyIncome / 20f) * 100f;
                    break;
                case AIStrategy.Rush:
                    effectiveness = matchTime < 120f ? 100f : 50f;
                    break;
            }
            
            return Mathf.Clamp(effectiveness, 0f, 100f);
        }
    }
} 