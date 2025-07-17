using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetaEarthStrike.Core;

namespace MetaEarthStrike.Data
{
    [CreateAssetMenu(fileName = "New Faction Data", menuName = "Meta Earth Strike/Faction Data")]
    public class FactionData : ScriptableObject
    {
        [Header("Faction Info")]
        public FactionType factionType = FactionType.Alliance;
        public string factionName = "Alliance";
        public string description = "Noble defenders of justice";
        
        [Header("Visual Theme")]
        public Color primaryColor = Color.blue;
        public Color secondaryColor = Color.gold;
        public Sprite factionIcon;
        public Material factionMaterial;
        
        [Header("Units")]
        public UnitData[] meleeUnits = new UnitData[3];
        public UnitData[] rangedUnits = new UnitData[3];
        public UnitData[] siegeUnits = new UnitData[3];
        
        [Header("Hero")]
        public GameObject heroPrefab;
        public string heroName = "Hero";
        public int heroBaseHealth = 200;
        public int heroBaseMana = 100;
        public int heroBaseDamage = 30;
        
        [Header("Hero Abilities")]
        public HeroAbilityData[] heroAbilities = new HeroAbilityData[4];
        
        [Header("Base")]
        public GameObject basePrefab;
        public int baseHealth = 1000;
        public GameObject[] baseUpgrades = new GameObject[3];
        
        [Header("Economy")]
        public int startingGold = 100;
        public int startingIncome = 10;
        public float incomeScaling = 1.1f;
        
        public UnitData GetUnitByTypeAndTier(UnitType unitType, int tier)
        {
            if (tier < 1 || tier > 3) return null;
            
            switch (unitType)
            {
                case UnitType.Melee:
                    return tier <= meleeUnits.Length ? meleeUnits[tier - 1] : null;
                case UnitType.Ranged:
                    return tier <= rangedUnits.Length ? rangedUnits[tier - 1] : null;
                case UnitType.Siege:
                    return tier <= siegeUnits.Length ? siegeUnits[tier - 1] : null;
                default:
                    return null;
            }
        }
        
        public int GetIncomeAtLevel(int level)
        {
            return Mathf.RoundToInt(startingIncome * Mathf.Pow(incomeScaling, level));
        }
    }
    
    [System.Serializable]
    public class HeroAbilityData
    {
        public string abilityName;
        public string description;
        public float cooldown;
        public int manaCost;
        public float range;
        public int damage;
        public HeroAbility.AbilityType abilityType;
        public GameObject effectPrefab;
        public Sprite abilityIcon;
    }
} 