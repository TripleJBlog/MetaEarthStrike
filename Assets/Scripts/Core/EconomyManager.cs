using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MetaEarthStrike.Core
{
    [System.Serializable]
    public class UpgradeData
    {
        public string upgradeName;
        public int cost;
        public int level = 0;
        public int maxLevel = 5;
        public float costMultiplier = 1.5f;
        public string description;
    }

    public class EconomyManager : MonoBehaviour
    {
        public static EconomyManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            // Use singleton pattern for GameManager and LaneManager if available
            gameManager = GameManager.Instance != null ? GameManager.Instance : FindFirstObjectByType<GameManager>();
            laneManager = LaneManager.Instance != null ? LaneManager.Instance : FindFirstObjectByType<LaneManager>();
        }
        
        [Header("Player Economy")]
        public int playerGold = 100;
        public int playerIncome = 10;
        public int playerIncomeLevel = 1;
        
        [Header("Enemy Economy")]
        public int enemyGold = 100;
        public int enemyIncome = 10;
        public int enemyIncomeLevel = 1;
        
        [Header("Upgrades")]
        public UpgradeData incomeUpgrade = new UpgradeData
        {
            upgradeName = "Income Boost",
            cost = 50,
            description = "Increase gold income per second"
        };
        
        public UpgradeData meleeUpgrade = new UpgradeData
        {
            upgradeName = "Melee Units",
            cost = 75,
            description = "Upgrade melee unit tier"
        };
        
        public UpgradeData rangedUpgrade = new UpgradeData
        {
            upgradeName = "Ranged Units",
            cost = 75,
            description = "Upgrade ranged unit tier"
        };
        
        public UpgradeData siegeUpgrade = new UpgradeData
        {
            upgradeName = "Siege Units",
            cost = 100,
            description = "Upgrade siege unit tier"
        };
        
        public UpgradeData baseDefenseUpgrade = new UpgradeData
        {
            upgradeName = "Base Defense",
            cost = 150,
            description = "Increase base health"
        };
        
        [Header("Events")]
        public UnityEvent<int> OnPlayerGoldChanged;
        public UnityEvent<int> OnPlayerIncomeChanged;
        public UnityEvent<string> OnUpgradePurchased;
        
        private GameManager gameManager;
        private LaneManager laneManager;
        
        public void Initialize(int startingGold, int startingIncome, int enemyStartingGold, int enemyStartingIncome)
        {
            playerGold = startingGold;
            playerIncome = startingIncome;
            enemyGold = enemyStartingGold;
            enemyIncome = enemyStartingIncome;
            
            // Initialize upgrade costs
            UpdateUpgradeCosts();
        }
        
        public void AddIncome()
        {
            playerGold += playerIncome;
            enemyGold += enemyIncome;
            
            OnPlayerGoldChanged?.Invoke(playerGold);
        }
        
        public bool CanAffordUpgrade(UpgradeData upgrade)
        {
            return playerGold >= upgrade.cost;
        }
        
        public bool PurchaseUpgrade(UpgradeData upgrade)
        {
            if (!CanAffordUpgrade(upgrade) || upgrade.level >= upgrade.maxLevel)
                return false;
            
            playerGold -= upgrade.cost;
            upgrade.level++;
            
            // Apply upgrade effects
            ApplyUpgradeEffect(upgrade);
            
            // Update cost for next level
            UpdateUpgradeCost(upgrade);
            
            OnPlayerGoldChanged?.Invoke(playerGold);
            OnUpgradePurchased?.Invoke(upgrade.upgradeName);
            
            return true;
        }
        
        private void ApplyUpgradeEffect(UpgradeData upgrade)
        {
            switch (upgrade.upgradeName)
            {
                case "Income Boost":
                    playerIncome += 5;
                    OnPlayerIncomeChanged?.Invoke(playerIncome);
                    break;
                    
                case "Melee Units":
                    // This would be handled by the unit spawning system
                    // LaneManager would check the upgrade level when spawning units
                    break;
                    
                case "Ranged Units":
                    // This would be handled by the unit spawning system
                    break;
                    
                case "Siege Units":
                    // This would be handled by the unit spawning system
                    break;
                    
                case "Base Defense":
                    if (gameManager != null)
                    {
                        gameManager.playerBaseHealth += 200;
                    }
                    break;
            }
        }
        
        private void UpdateUpgradeCost(UpgradeData upgrade)
        {
            upgrade.cost = Mathf.RoundToInt(upgrade.cost * upgrade.costMultiplier);
        }
        
        private void UpdateUpgradeCosts()
        {
            UpdateUpgradeCost(incomeUpgrade);
            UpdateUpgradeCost(meleeUpgrade);
            UpdateUpgradeCost(rangedUpgrade);
            UpdateUpgradeCost(siegeUpgrade);
            UpdateUpgradeCost(baseDefenseUpgrade);
        }
        
        public int GetUpgradeLevel(string upgradeName)
        {
            switch (upgradeName)
            {
                case "Income Boost": return incomeUpgrade.level;
                case "Melee Units": return meleeUpgrade.level;
                case "Ranged Units": return rangedUpgrade.level;
                case "Siege Units": return siegeUpgrade.level;
                case "Base Defense": return baseDefenseUpgrade.level;
                default: return 0;
            }
        }
        
        public UpgradeData GetUpgradeData(string upgradeName)
        {
            switch (upgradeName)
            {
                case "Income Boost": return incomeUpgrade;
                case "Melee Units": return meleeUpgrade;
                case "Ranged Units": return rangedUpgrade;
                case "Siege Units": return siegeUpgrade;
                case "Base Defense": return baseDefenseUpgrade;
                default: return null;
            }
        }
        
        public void SpendGold(int amount)
        {
            if (playerGold >= amount)
            {
                playerGold -= amount;
                OnPlayerGoldChanged?.Invoke(playerGold);
            }
        }
        
        public void AddGold(int amount)
        {
            playerGold += amount;
            OnPlayerGoldChanged?.Invoke(playerGold);
        }
        
        // AI Economy Management (for enemy)
        public void UpdateEnemyEconomy()
        {
            // Simple AI: randomly purchase upgrades
            if (Random.Range(0f, 1f) < 0.1f) // 10% chance per frame
            {
                PurchaseRandomEnemyUpgrade();
            }
        }
        
        private void PurchaseRandomEnemyUpgrade()
        {
            // Simple AI upgrade logic
            if (enemyGold >= 50)
            {
                enemyGold -= 50;
                enemyIncome += 2;
            }
        }
        
        public void ResetEconomy()
        {
            playerGold = 100;
            playerIncome = 10;
            enemyGold = 100;
            enemyIncome = 10;
            
            // Reset upgrade levels
            incomeUpgrade.level = 0;
            meleeUpgrade.level = 0;
            rangedUpgrade.level = 0;
            siegeUpgrade.level = 0;
            baseDefenseUpgrade.level = 0;
            
            UpdateUpgradeCosts();
            
            OnPlayerGoldChanged?.Invoke(playerGold);
            OnPlayerIncomeChanged?.Invoke(playerIncome);
        }
    }
} 