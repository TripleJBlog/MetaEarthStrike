using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using MetaEarthStrike.Core;

namespace MetaEarthStrike.UI
{
    public class HealthBar : MonoBehaviour
    {
        [Header("Health Bar Components")]
        public Slider healthSlider;
        public Image fillImage;
        public TextMeshProUGUI healthText;
        
        [Header("Colors")]
        public Color highHealthColor = Color.green;
        public Color mediumHealthColor = Color.yellow;
        public Color lowHealthColor = Color.red;
        
        [Header("Settings")]
        public bool showHealthText = true;
        public bool updateColor = true;
        public float mediumHealthThreshold = 0.6f;
        public float lowHealthThreshold = 0.3f;
        
        private Unit targetUnit;
        private Camera mainCamera;
        
        private void Awake()
        {
            mainCamera = Camera.main;
            
            if (healthSlider == null)
                healthSlider = GetComponent<Slider>();
                
            if (fillImage == null)
                fillImage = healthSlider.fillRect.GetComponent<Image>();
        }
        
        private void Start()
        {
            // Find the unit this health bar belongs to
            targetUnit = GetComponentInParent<Unit>();
            
            if (targetUnit != null)
            {
                UpdateHealthBar(targetUnit.currentHealth, targetUnit.maxHealth);
            }
        }
        
        private void Update()
        {
            if (targetUnit != null)
            {
                UpdateHealthBar(targetUnit.currentHealth, targetUnit.maxHealth);
                
                // Make health bar face camera
                if (mainCamera != null)
                {
                    transform.LookAt(mainCamera.transform);
                    transform.Rotate(0, 180, 0);
                }
            }
        }
        
        public void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            if (healthSlider == null) return;
            
            float healthPercentage = (float)currentHealth / maxHealth;
            healthSlider.value = healthPercentage;
            
            // Update health text
            if (healthText != null && showHealthText)
            {
                healthText.text = $"{currentHealth}/{maxHealth}";
            }
            
            // Update color based on health percentage
            if (fillImage != null && updateColor)
            {
                if (healthPercentage > mediumHealthThreshold)
                {
                    fillImage.color = highHealthColor;
                }
                else if (healthPercentage > lowHealthThreshold)
                {
                    fillImage.color = mediumHealthColor;
                }
                else
                {
                    fillImage.color = lowHealthColor;
                }
            }
        }
        
        public void SetTargetUnit(Unit unit)
        {
            targetUnit = unit;
        }
        
        public void ShowHealthBar(bool show)
        {
            gameObject.SetActive(show);
        }
    }
} 