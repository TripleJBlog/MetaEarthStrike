using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetaEarthStrike.Core;

namespace MetaEarthStrike.Data
{
    [CreateAssetMenu(fileName = "New Unit Data", menuName = "Meta Earth Strike/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [Header("Unit Info")]
        public string unitName = "Unit";
        public UnitType unitType = UnitType.Melee;
        public FactionType faction = FactionType.Alliance;
        public int tier = 1;
        
        [Header("Stats")]
        public int maxHealth = 100;
        public int damage = 20;
        public float attackRange = 1f;
        public float attackSpeed = 1f;
        public float moveSpeed = 3f;
        
        [Header("Visual")]
        public Sprite unitSprite;
        public GameObject unitPrefab;
        public RuntimeAnimatorController animatorController;
        
        [Header("Upgrade Scaling")]
        public float healthScaling = 1.2f;
        public float damageScaling = 1.15f;
        public float costScaling = 1.5f;
        
        [Header("Cost")]
        public int baseCost = 50;
        
        public int GetUpgradedHealth(int upgradeLevel)
        {
            return Mathf.RoundToInt(maxHealth * Mathf.Pow(healthScaling, upgradeLevel));
        }
        
        public int GetUpgradedDamage(int upgradeLevel)
        {
            return Mathf.RoundToInt(damage * Mathf.Pow(damageScaling, upgradeLevel));
        }
        
        public int GetUpgradeCost(int upgradeLevel)
        {
            return Mathf.RoundToInt(baseCost * Mathf.Pow(costScaling, upgradeLevel));
        }
    }
} 