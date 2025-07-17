using UnityEngine;
using MetaEarthStrike.Data; // Use the canonical UnitData ScriptableObject

namespace MetaEarthStrike.Core
{
    public class UnitSpawner : MonoBehaviour
    {
        [Header("Available Units (ScriptableObjects)")]
        [SerializeField] private UnitData[] availableUnits; // Drag UnitData assets here in the inspector
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform enemyBase;

        // Spawns a unit by index (for UI buttons)
        public void SpawnUnit(int unitIndex, int upgradeLevel = 0)
        {
            if (unitIndex < 0 || unitIndex >= availableUnits.Length)
                return;
            SpawnUnit(availableUnits[unitIndex], upgradeLevel);
        }

        // Spawns a unit by UnitData reference
        public void SpawnUnit(UnitData unitData, int upgradeLevel = 0)
        {
            if (unitData == null || unitData.unitPrefab == null || spawnPoint == null)
                return;

            GameObject unit = Instantiate(unitData.unitPrefab, spawnPoint.position, Quaternion.identity);

            // Apply stats from UnitData (with upgrade scaling)
            var unitComponent = unit.GetComponent<Unit>();
            if (unitComponent != null)
            {
                unitComponent.unitName = unitData.unitName;
                unitComponent.unitType = unitData.unitType;
                unitComponent.maxHealth = unitData.GetUpgradedHealth(upgradeLevel);
                unitComponent.currentHealth = unitComponent.maxHealth;
                unitComponent.damage = unitData.GetUpgradedDamage(upgradeLevel);
                unitComponent.attackRange = unitData.attackRange;
                unitComponent.attackSpeed = unitData.attackSpeed;
                unitComponent.moveSpeed = unitData.moveSpeed;
                // Optionally set visuals, animator, etc.
            }

            // Start movement coroutine (simple example)
            StartCoroutine(MoveUnitToTarget(unit, unitData.moveSpeed));
        }

        private System.Collections.IEnumerator MoveUnitToTarget(GameObject unit, float speed)
        {
            while (unit != null && enemyBase != null)
            {
                if (Vector3.Distance(unit.transform.position, enemyBase.position) > 0.1f)
                {
                    // Move towards enemy base
                    Vector3 direction = (enemyBase.position - unit.transform.position).normalized;
                    unit.transform.position += direction * speed * Time.deltaTime;

                    // Rotate to face movement direction
                    if (direction != Vector3.zero)
                    {
                        unit.transform.rotation = Quaternion.LookRotation(direction);
                    }
                }
                yield return null;
            }
        }
    }
}