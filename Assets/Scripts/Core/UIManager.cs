using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MetaEarthStrike.Core
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        [Header("Game Status")]
        public TextMeshProUGUI goldText;
        public TextMeshProUGUI incomeText;
        public TextMeshProUGUI playerBaseHealthText;
        public TextMeshProUGUI enemyBaseHealthText;
        public TextMeshProUGUI timerText;
        public Slider playerBaseHealthSlider;
        public Slider enemyBaseHealthSlider;
        
        [Header("Hero UI")]
        public GameObject heroPanel;
        public TextMeshProUGUI heroNameText;
        public TextMeshProUGUI heroLevelText;
        public TextMeshProUGUI heroHealthText;
        public TextMeshProUGUI heroManaText;
        public Slider heroHealthSlider;
        public Slider heroManaSlider;
        public Button[] abilityButtons = new Button[4];
        public Image[] abilityCooldownImages = new Image[4];
        public TextMeshProUGUI[] abilityCostTexts = new TextMeshProUGUI[4];
        
        // [Header("Upgrade UI")]
        // public GameObject upgradePanel;
        // public Button[] upgradeButtons = new Button[5];
        // public TextMeshProUGUI[] upgradeCostTexts = new TextMeshProUGUI[5];
        // public TextMeshProUGUI[] upgradeLevelTexts = new TextMeshProUGUI[5];
        
        [Header("Game Controls")]
        public Button pauseButton;
        public Button speedButton;
        public GameObject pausePanel;
        public GameObject gameOverPanel;
        public TextMeshProUGUI gameOverText;
        public Button restartButton;
        public Button mainMenuButton;
        
        [Header("Lane UI")]
        public GameObject[] laneButtons = new GameObject[1]; // Single lane
        public TextMeshProUGUI[] laneUnitCountTexts = new TextMeshProUGUI[1]; // Single lane
        
        private GameManager gameManager;
        private EconomyManager economyManager;
        private HeroManager heroManager;
        private LaneManager laneManager;
        
        private float matchTimer;
        private bool isPaused = false;
        private bool isSpeedUp = false;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
            gameManager = FindFirstObjectByType<GameManager>();
            economyManager = FindFirstObjectByType<EconomyManager>();
            heroManager = FindFirstObjectByType<HeroManager>();
            laneManager = FindFirstObjectByType<LaneManager>();
        }
        
        private void Start()
        {
            // SetupUI();
            // SubscribeToEvents();
        }
        
        private void Update()
        {
            if (GameManager.Instance.currentGameState == GameState.Gameplay)
            {
                UpdateGameUI();
                UpdateHeroUI();
                UpdateAbilityUI();
                UpdateLaneUI();
            }
        }
        
        private void SetupUI()
        {
            // Setup upgrade buttons
            // SetupUpgradeButtons();
            
            // Setup ability buttons
            SetupAbilityButtons();
            
            // Setup control buttons
            SetupControlButtons();
            
            // Setup game over panel
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
                
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }
        
        // private void SetupUpgradeButtons()
        // {
        //     string[] upgradeNames = { "Income Boost", "Melee Units", "Ranged Units", "Siege Units", "Base Defense" };
            
        //     for (int i = 0; i < upgradeButtons.Length && i < upgradeNames.Length; i++)
        //     {
        //         int index = i; // Capture for lambda
        //         upgradeButtons[i].onClick.AddListener(() => PurchaseUpgrade(upgradeNames[index]));
        //     }
        // }
        
        private void SetupAbilityButtons()
        {
            for (int i = 0; i < abilityButtons.Length; i++)
            {
                int index = i; // Capture for lambda
                abilityButtons[i].onClick.AddListener(() => UseHeroAbility(index));
            }
        }
        
        private void SetupControlButtons()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(TogglePause);
                
            if (speedButton != null)
                speedButton.onClick.AddListener(ToggleSpeed);
                
            if (restartButton != null)
                restartButton.onClick.AddListener(RestartGame);
                
            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        
        private void SubscribeToEvents()
        {
            if (gameManager != null)
            {
                gameManager.OnPlayerGoldChanged.AddListener(UpdateGoldDisplay);
                gameManager.OnPlayerBaseHealthChanged.AddListener(UpdatePlayerBaseHealth);
                gameManager.OnEnemyBaseHealthChanged.AddListener(UpdateEnemyBaseHealth);
                gameManager.OnGameStateChanged.AddListener(OnGameStateChanged);
            }
            
            if (economyManager != null)
            {
                economyManager.OnPlayerIncomeChanged.AddListener(UpdateIncomeDisplay);
                economyManager.OnUpgradePurchased.AddListener(OnUpgradePurchased);
            }
        }
        
        private void UpdateGameUI()
        {
            // Update timer
            matchTimer += Time.deltaTime;
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(matchTimer / 60f);
                int seconds = Mathf.FloorToInt(matchTimer % 60f);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            
            // Update upgrade costs
            // UpdateUpgradeCosts();
        }
        
        private void UpdateHeroUI()
        {
            if (heroManager == null || heroManager.heroInstance == null)
                return;
                
            // Update hero stats
            if (heroNameText != null)
                heroNameText.text = heroManager.heroName;
                
            if (heroLevelText != null)
                heroLevelText.text = $"Level {heroManager.level}";
                
            if (heroHealthText != null)
                heroHealthText.text = $"{heroManager.currentHealth}/{heroManager.maxHealth}";
                
            if (heroManaText != null)
                heroManaText.text = $"{heroManager.currentMana}/{heroManager.maxMana}";
                
            // Update sliders
            if (heroHealthSlider != null)
                heroHealthSlider.value = (float)heroManager.currentHealth / heroManager.maxHealth;
                
            if (heroManaSlider != null)
                heroManaSlider.value = (float)heroManager.currentMana / heroManager.maxMana;
        }
        
        private void UpdateAbilityUI()
        {
            if (heroManager == null)
                return;
                
            for (int i = 0; i < abilityButtons.Length; i++)
            {
                if (i >= heroManager.abilities.Length)
                    continue;
                    
                HeroAbility ability = heroManager.abilities[i];
                
                // Update cooldown
                if (abilityCooldownImages[i] != null)
                {
                    float cooldown = heroManager.GetAbilityCooldown(i);
                    float cooldownPercent = cooldown / ability.cooldown;
                    abilityCooldownImages[i].fillAmount = cooldownPercent;
                }
                
                // Update cost text
                if (abilityCostTexts[i] != null)
                {
                    abilityCostTexts[i].text = ability.manaCost.ToString();
                }
                
                // Update button interactability
                if (abilityButtons[i] != null)
                {
                    abilityButtons[i].interactable = heroManager.CanUseAbility(i);
                }
            }
        }
        
        private void UpdateLaneUI()
        {
            if (laneManager == null)
                return;
                
            for (int i = 0; i < laneButtons.Length; i++)
            {
                if (i < laneManager.lanes.Count)
                {
                    Lane lane = laneManager.lanes[i];
                    int playerUnits = 0;
                    int enemyUnits = 0;
                    
                    foreach (Unit unit in lane.activeUnits)
                    {
                        if (unit.isPlayerUnit)
                            playerUnits++;
                        else
                            enemyUnits++;
                    }
                    
                    if (laneUnitCountTexts[i] != null)
                    {
                        laneUnitCountTexts[i].text = $"{playerUnits} vs {enemyUnits}";
                    }
                }
            }
        }
        
        private void UpdateGoldDisplay(int gold)
        {
            if (goldText != null)
                goldText.text = $"Gold: {gold}";
        }
        
        private void UpdateIncomeDisplay(int income)
        {
            if (incomeText != null)
                incomeText.text = $"Income: {income}/s";
        }
        
        private void UpdatePlayerBaseHealth(int health)
        {
            if (playerBaseHealthText != null)
                playerBaseHealthText.text = $"Base: {health}";
                
            if (playerBaseHealthSlider != null)
                playerBaseHealthSlider.value = (float)health / gameManager.playerBaseHealth;
        }
        
        private void UpdateEnemyBaseHealth(int health)
        {
            if (enemyBaseHealthText != null)
                enemyBaseHealthText.text = $"Enemy: {health}";
                
            if (enemyBaseHealthSlider != null)
                enemyBaseHealthSlider.value = (float)health / gameManager.enemyBaseHealth;
        }
        
        // private void UpdateUpgradeCosts()
        // {
        //     if (economyManager == null)
        //         return;
                
        //     string[] upgradeNames = { "Income Boost", "Melee Units", "Ranged Units", "Siege Units", "Base Defense" };
            
        //     for (int i = 0; i < upgradeCostTexts.Length && i < upgradeNames.Length; i++)
        //     {
        //         UpgradeData upgrade = economyManager.GetUpgradeData(upgradeNames[i]);
        //         if (upgrade != null && upgradeCostTexts[i] != null)
        //         {
        //             upgradeCostTexts[i].text = upgrade.cost.ToString();
                    
        //             // Update button interactability
        //             if (upgradeButtons[i] != null)
        //             {
        //                 upgradeButtons[i].interactable = economyManager.CanAffordUpgrade(upgrade) && upgrade.level < upgrade.maxLevel;
        //             }
                    
        //             // Update level text
        //             if (upgradeLevelTexts[i] != null)
        //             {
        //                 upgradeLevelTexts[i].text = $"Lv.{upgrade.level}";
        //             }
        //         }
        //     }
        // }
        
        private void PurchaseUpgrade(string upgradeName)
        {
            if (economyManager != null)
            {
                UpgradeData upgrade = economyManager.GetUpgradeData(upgradeName);
                if (upgrade != null)
                {
                    economyManager.PurchaseUpgrade(upgrade);
                }
            }
        }
        
        private void UseHeroAbility(int abilityIndex)
        {
            if (heroManager != null)
            {
                heroManager.UseAbility(abilityIndex);
            }
        }
        
        private void TogglePause()
        {
            isPaused = !isPaused;
            
            if (isPaused)
            {
                GameManager.Instance.ChangeGameState(GameState.Paused);
                if (pausePanel != null)
                    pausePanel.SetActive(true);
            }
            else
            {
                GameManager.Instance.ChangeGameState(GameState.Gameplay);
                if (pausePanel != null)
                    pausePanel.SetActive(false);
            }
        }
        
        private void ToggleSpeed()
        {
            isSpeedUp = !isSpeedUp;
            
            if (isSpeedUp)
            {
                Time.timeScale = 2f;
                if (speedButton != null)
                {
                    TextMeshProUGUI speedText = speedButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (speedText != null)
                        speedText.text = "1x";
                }
            }
            else
            {
                Time.timeScale = 1f;
                if (speedButton != null)
                {
                    TextMeshProUGUI speedText = speedButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (speedText != null)
                        speedText.text = "2x";
                }
            }
        }
        
        private void RestartGame()
        {
            GameManager.Instance.RestartGame();
        }
        
        private void GoToMainMenu()
        {
            // Load main menu scene
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
        
        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.GameOver:
                    ShowGameOverPanel();
                    break;
                case GameState.Gameplay:
                    if (gameOverPanel != null)
                        gameOverPanel.SetActive(false);
                    if (pausePanel != null)
                        pausePanel.SetActive(false);
                    break;
            }
        }
        
        private void ShowGameOverPanel()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                
                if (gameOverText != null)
                {
                    bool playerWon = gameManager.playerBaseHealth > 0 && gameManager.enemyBaseHealth <= 0;
                    gameOverText.text = playerWon ? "Victory!" : "Defeat!";
                }
            }
        }
        
        private void OnUpgradePurchased(string upgradeName)
        {
            // Could add visual feedback here
            Debug.Log($"Upgrade purchased: {upgradeName}");
        }
        
        public void SelectLane(int laneIndex)
        {
            // Handle lane selection (could be used for hero movement or targeting)
            Debug.Log($"Selected lane: {laneIndex}");
        }
    }
} 