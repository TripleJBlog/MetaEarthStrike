using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MetaEarthStrike.Core
{
    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        GameOver
    }

    public enum FactionType
    {
        Alliance,
        Horde,
        Scourge
    }

    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        public int maxLanes = 1; // Single lane 1v1
        public float matchDuration = 900f; // 15 minutes
        public float incomeInterval = 1f; // Gold per second
        
        [Header("Game State")]
        public GameState currentGameState = GameState.MainMenu;
        public FactionType playerFaction = FactionType.Alliance;
        public FactionType enemyFaction = FactionType.Horde;
        
        [Header("Economy")]
        public int playerGold = 100;
        public int playerIncome = 10;
        public int enemyGold = 100;
        public int enemyIncome = 10;
        
        [Header("Base Health")]
        public int playerBaseHealth = 1000;
        public int enemyBaseHealth = 1000;
        
        [Header("Events")]
        public UnityEvent<GameState> OnGameStateChanged;
        public UnityEvent<int> OnPlayerGoldChanged;
        public UnityEvent<int> OnPlayerBaseHealthChanged;
        public UnityEvent<int> OnEnemyBaseHealthChanged;
        
        // Singleton
        public static GameManager Instance { get; private set; }
        
        // Components
        private LaneManager laneManager;
        private EconomyManager economyManager;
        private HeroManager heroManager;
        private UIManager uiManager;
        
        // Timers
        private float matchTimer;
        private float incomeTimer;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializeManagers();
            SetupGame();
        }
        
        private void Update()
        {
            if (currentGameState == GameState.Gameplay)
            {
                UpdateGameplay();
            }
        }
        
        private void InitializeManagers()
        {
            laneManager = LaneManager.Instance != null ? LaneManager.Instance : FindFirstObjectByType<LaneManager>();
            economyManager = EconomyManager.Instance != null ? EconomyManager.Instance : FindFirstObjectByType<EconomyManager>();
            heroManager = HeroManager.Instance != null ? HeroManager.Instance : FindFirstObjectByType<HeroManager>();
            uiManager = UIManager.Instance != null ? UIManager.Instance : FindFirstObjectByType<UIManager>();

            if (laneManager == null)
                laneManager = gameObject.AddComponent<LaneManager>();
            if (economyManager == null)
                economyManager = gameObject.AddComponent<EconomyManager>();
            if (heroManager == null)
                heroManager = gameObject.AddComponent<HeroManager>();
            if (uiManager == null)
                uiManager = gameObject.AddComponent<UIManager>();
        }
        
        private void SetupGame()
        {
            laneManager.InitializeLanes(maxLanes);
            economyManager.Initialize(playerGold, playerIncome, enemyGold, enemyIncome);
            heroManager.Initialize(playerFaction);
            
            ChangeGameState(GameState.Gameplay);
        }
        
        private void UpdateGameplay()
        {
            // Update match timer
            matchTimer += Time.deltaTime;
            if (matchTimer >= matchDuration)
            {
                EndMatch();
                return;
            }
            
            // Update income timer
            incomeTimer += Time.deltaTime;
            if (incomeTimer >= incomeInterval)
            {
                economyManager.AddIncome();
                incomeTimer = 0f;
            }
            
            // Check win conditions
            if (playerBaseHealth <= 0)
            {
                EndMatch(false);
            }
            else if (enemyBaseHealth <= 0)
            {
                EndMatch(true);
            }
        }
        
        public void ChangeGameState(GameState newState)
        {
            currentGameState = newState;
            OnGameStateChanged?.Invoke(newState);
            
            switch (newState)
            {
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.Gameplay:
                    Time.timeScale = 1f;
                    break;
            }
        }
        
        public void AddPlayerGold(int amount)
        {
            playerGold += amount;
            OnPlayerGoldChanged?.Invoke(playerGold);
        }
        
        public void SpendPlayerGold(int amount)
        {
            if (playerGold >= amount)
            {
                playerGold -= amount;
                OnPlayerGoldChanged?.Invoke(playerGold);
            }
        }
        
        public void DamagePlayerBase(int damage)
        {
            playerBaseHealth -= damage;
            OnPlayerBaseHealthChanged?.Invoke(playerBaseHealth);
        }
        
        public void DamageEnemyBase(int damage)
        {
            enemyBaseHealth -= damage;
            OnEnemyBaseHealthChanged?.Invoke(enemyBaseHealth);
        }
        
        private void EndMatch(bool playerWon = false)
        {
            ChangeGameState(GameState.GameOver);
            Debug.Log($"Match ended! Player {(playerWon ? "won" : "lost")}");
        }
        
        public void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        
        public void PauseGame()
        {
            if (currentGameState == GameState.Gameplay)
                ChangeGameState(GameState.Paused);
            else if (currentGameState == GameState.Paused)
                ChangeGameState(GameState.Gameplay);
        }
    }
} 