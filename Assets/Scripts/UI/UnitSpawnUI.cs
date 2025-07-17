using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MetaEarthStrike.Core;

namespace MetaEarthStrike.UI
{
    public class UnitSpawnUI : MonoBehaviour
    {
        [System.Serializable]
        public class UnitButton
        {
            public Button button;
            public int cost;
            public int unitIndex;
            public TextMeshProUGUI costText;
        }

        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private UnitButton[] unitButtons;
        [SerializeField] private UnitSpawner unitSpawner;

        private void Start()
        {
            // Subscribe to gold changes
            GoldManager.Instance.OnGoldChanged += UpdateGoldDisplay;
            
            // Initialize buttons
            for (int i = 0; i < unitButtons.Length; i++)
            {
                var button = unitButtons[i];
                button.costText.text = button.cost.ToString() + " gold";
                
                int index = i; // Capture the index for the lambda
                button.button.onClick.AddListener(() => TrySpawnUnit(index));
            }

            // Initial gold display
            UpdateGoldDisplay(GoldManager.Instance.GetCurrentGold());
        }

        private void UpdateGoldDisplay(int currentGold)
        {
            goldText.text = "Gold: " + currentGold;
            
            // Update button interactability based on gold
            foreach (var button in unitButtons)
            {
                button.button.interactable = currentGold >= button.cost;
            }
        }

        private void TrySpawnUnit(int buttonIndex)
        {
            var button = unitButtons[buttonIndex];
            
            if (GoldManager.Instance.SpendGold(button.cost))
            {
                unitSpawner.SpawnUnit(button.unitIndex);
            }
        }

        private void OnDestroy()
        {
            if (GoldManager.Instance != null)
            {
                GoldManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
            }
        }
    }
}