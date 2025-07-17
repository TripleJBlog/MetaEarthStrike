using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MetaEarthStrike.Core;

namespace MetaEarthStrike.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Gold Display")]
        public TextMeshProUGUI goldText;

        [Header("Unit Buttons")]
        public Button unit1Button;
        public TextMeshProUGUI unit1ButtonText;
        public int unit1Cost = 10;

        private void Start()
        {
            // Subscribe to gold changes
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.OnPlayerGoldChanged.AddListener(UpdateGoldDisplay);
                UpdateGoldDisplay(EconomyManager.Instance.playerGold);
            }
            else
            {
                Debug.LogWarning("EconomyManager.Instance is null in GameUI.Start(). Make sure EconomyManager exists in the scene and initializes before GameUI.");
            }

            if (unit1ButtonText != null)
                unit1ButtonText.text = $"Unit1 ({unit1Cost} Gold)";
            if (unit1Button != null)
                unit1Button.onClick.AddListener(OnUnit1ButtonClicked);
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (EconomyManager.Instance != null)
            {
                EconomyManager.Instance.OnPlayerGoldChanged.RemoveListener(UpdateGoldDisplay);
            }
        }

        void UpdateGoldDisplay(int gold)
        {
            if (goldText != null)
            {
                goldText.text = $"{gold} Gold";
            }
        }

        public void OnUnit1ButtonClicked()
        {
            if (EconomyManager.Instance != null && EconomyManager.Instance.playerGold >= unit1Cost)
            {
                EconomyManager.Instance.SpendGold(unit1Cost);
                // Spawn swordman at PlayerSpawnPoint and set to attack EnemyBase
                LaneManager laneManager = LaneManager.Instance;
                if (laneManager != null && laneManager.lanes.Count > 0)
                {
                    var lane = laneManager.lanes[0];
                    var pool = laneManager.GetType().GetField("unitPool", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(laneManager) as ObjectPool;
                    if (pool != null && laneManager.playerUnitPrefabs.Length > 0)
                    {
                        GameObject swordmanPrefab = laneManager.playerUnitPrefabs[0];
                        GameObject swordman = pool.GetObject(swordmanPrefab);
                        swordman.transform.position = lane.playerSpawnPoint.position;
                        swordman.SetActive(true);
                        Unit unitScript = swordman.GetComponent<Unit>();
                        if (unitScript != null)
                        {
                            unitScript.Initialize(lane, true, lane.waypoints);
                            Debug.Log("unit1 spawned");
                        }
                        else
                        {
                            Debug.LogWarning("Spawned swordman does not have a Unit script attached.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("ObjectPool or playerUnitPrefabs not set up correctly in LaneManager.");
                    }
                }
                else
                {
                    Debug.LogWarning("LaneManager or lanes not set up correctly.");
                }
            }
            else
            {
                Debug.Log("Not enough gold!");
            }
        }
    }
}
