using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MetaEarthStrike.Core; // Required for UpgradeData, EconomyManager, etc.

namespace MetaEarthStrike.UI
{
    public class UpgradeButton : MonoBehaviour
    {
        [Header("UI Components")]
        public Button button;
        public TextMeshProUGUI upgradeNameText;
        public TextMeshProUGUI costText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI descriptionText;
        public Image upgradeIcon;
        public Image costIcon;
        
        [Header("Visual Feedback")]
        public Image buttonBackground;
        public Color affordableColor = Color.green;
        public Color unaffordableColor = Color.red;
        public Color maxLevelColor = Color.gray;
        
        [Header("Animation")]
        public float pulseSpeed = 1f;
        public float pulseAmount = 0.1f;
        public bool pulseWhenAffordable = true;
        
        private UpgradeData upgradeData;
        private EconomyManager economyManager;
        private bool isAffordable = false;
        private bool isMaxLevel = false;
        
        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
            
            economyManager = EconomyManager.Instance;
        }
        
        private void Start()
        {
            SetupButton();
        }
        
        private void Update()
        {
            UpdateButtonState();
            
            if (pulseWhenAffordable && isAffordable && !isMaxLevel)
            {
                PulseAnimation();
            }
        }
        
        public void Initialize(UpgradeData upgrade)
        {
            upgradeData = upgrade;
            UpdateDisplay();
        }
        
        private void SetupButton()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnUpgradeClicked);
            }
        }
        
        private void UpdateDisplay()
        {
            if (upgradeData == null) return;
            
            // Update text displays
            if (upgradeNameText != null)
                upgradeNameText.text = upgradeData.upgradeName;
                
            if (costText != null)
                costText.text = upgradeData.cost.ToString();
                
            if (levelText != null)
                levelText.text = $"Lv.{upgradeData.level}";
                
            if (descriptionText != null)
                descriptionText.text = upgradeData.description;
        }
        
        private void UpdateButtonState()
        {
            if (upgradeData == null || economyManager == null) return;
            
            isAffordable = economyManager.CanAffordUpgrade(upgradeData);
            isMaxLevel = upgradeData.level >= upgradeData.maxLevel;
            
            // Update button interactability
            if (button != null)
            {
                button.interactable = isAffordable && !isMaxLevel;
            }
            
            // Update visual feedback
            UpdateVisualFeedback();
        }
        
        private void UpdateVisualFeedback()
        {
            if (buttonBackground == null) return;
            
            if (isMaxLevel)
            {
                buttonBackground.color = maxLevelColor;
            }
            else if (isAffordable)
            {
                buttonBackground.color = affordableColor;
            }
            else
            {
                buttonBackground.color = unaffordableColor;
            }
        }
        
        private void PulseAnimation()
        {
            if (buttonBackground == null) return;
            
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount + 1f;
            Color currentColor = buttonBackground.color;
            currentColor.a = pulse;
            buttonBackground.color = currentColor;
        }
        
        private void OnUpgradeClicked()
        {
            if (economyManager != null && upgradeData != null)
            {
                bool success = economyManager.PurchaseUpgrade(upgradeData);
                
                if (success)
                {
                    // Play success sound/effect
                    PlayUpgradeEffect();
                    
                    // Update display
                    UpdateDisplay();
                }
            }
        }
        
        private void PlayUpgradeEffect()
        {
            // Could add particle effects, sounds, etc.
            StartCoroutine(UpgradeFlashEffect());
        }
        
        private IEnumerator UpgradeFlashEffect()
        {
            if (buttonBackground == null) yield break;
            
            Color originalColor = buttonBackground.color;
            buttonBackground.color = Color.white;
            
            yield return new WaitForSeconds(0.1f);
            
            buttonBackground.color = originalColor;
        }
        
        public void SetUpgradeData(UpgradeData upgrade)
        {
            upgradeData = upgrade;
            UpdateDisplay();
        }
        
        public void SetIcon(Sprite icon)
        {
            if (upgradeIcon != null)
            {
                upgradeIcon.sprite = icon;
            }
        }
        
        public void SetCostIcon(Sprite icon)
        {
            if (costIcon != null)
            {
                costIcon.sprite = icon;
            }
        }
    }
} 