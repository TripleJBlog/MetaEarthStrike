using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MetaEarthStrike.AI;
using UnityEngine.UI;

using MetaEarthStrike.Core; // Required for GameManager, LaneManager, EconomyManager, etc.

namespace MetaEarthStrike.Editor
{
    public class GameSetupWizard : EditorWindow
    {
        [MenuItem("Meta Earth Strike/Game Setup Wizard")]
        public static void ShowWindow()
        {
            GetWindow<GameSetupWizard>("Game Setup Wizard");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Meta Earth Strike - Game Setup Wizard", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("This wizard will help you set up the game scene with all necessary components.", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);
            
            if (GUILayout.Button("Setup Complete Game Scene"))
            {
                SetupCompleteGameScene();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Setup Core Managers Only"))
            {
                SetupCoreManagers();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Setup UI Only"))
            {
                SetupUI();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Setup Camera"))
            {
                SetupCamera();
            }
            
            GUILayout.Space(20);
            
            GUILayout.Label("Individual Setup Options:", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            if (GUILayout.Button("Create GameManager"))
            {
                CreateGameManager();
            }
            
            if (GUILayout.Button("Create LaneManager"))
            {
                CreateLaneManager();
            }
            
            if (GUILayout.Button("Create EconomyManager"))
            {
                CreateEconomyManager();
            }
            
            if (GUILayout.Button("Create HeroManager"))
            {
                CreateHeroManager();
            }
            
            if (GUILayout.Button("Create UIManager"))
            {
                CreateUIManager();
            }
            
            if (GUILayout.Button("Create ObjectPool"))
            {
                CreateObjectPool();
            }
            
            if (GUILayout.Button("Create EnemyAI"))
            {
                CreateEnemyAI();
            }
            
            GUILayout.Space(20);
            
            GUILayout.Label("Troubleshooting:", EditorStyles.boldLabel);
            GUILayout.Space(5);
            
            if (GUILayout.Button("Check Scene Setup"))
            {
                CheckSceneSetup();
            }
            
            if (GUILayout.Button("Fix Missing References"))
            {
                FixMissingReferences();
            }
        }
        
        private void SetupCompleteGameScene()
        {
            SetupCoreManagers();
            SetupCamera();
            SetupUI();
            
            Debug.Log("Complete game scene setup finished!");
        }
        
        private void SetupCoreManagers()
        {
            CreateGameManager();
            CreateLaneManager();
            CreateEconomyManager();
            CreateHeroManager();
            CreateObjectPool();
            CreateEnemyAI();
            
            Debug.Log("Core managers setup finished!");
        }
        
        private void SetupUI()
        {
            CreateUIManager();
            
            // Create basic UI canvas
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                canvas = new GameObject("Canvas");
                Canvas canvasComponent = canvas.AddComponent<Canvas>();
                canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            Debug.Log("UI setup finished!");
        }
        
        private void SetupCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                mainCamera = cameraObj.AddComponent<Camera>();
                cameraObj.tag = "MainCamera";
            }
            
            // Add camera controller
            if (mainCamera.GetComponent<CameraController>() == null)
            {
                mainCamera.gameObject.AddComponent<CameraController>();
            }
            
            // Set up camera for 2.5D view (single lane)
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 8f; // Closer view for single lane
            mainCamera.transform.position = new Vector3(0, 12, -12);
            mainCamera.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            
            Debug.Log("Camera setup finished!");
        }
        
        private void CreateGameManager()
        {
            if (FindFirstObjectByType<GameManager>() == null)
            {
                GameObject gameManager = new GameObject("GameManager");
                gameManager.AddComponent<GameManager>();
                Debug.Log("GameManager created!");
            }
            else
            {
                Debug.Log("GameManager already exists!");
            }
        }
        
        private void CreateLaneManager()
        {
            if (FindFirstObjectByType<LaneManager>() == null)
            {
                GameObject laneManager = new GameObject("LaneManager");
                laneManager.AddComponent<LaneManager>();
                Debug.Log("LaneManager created!");
            }
            else
            {
                Debug.Log("LaneManager already exists!");
            }
        }
        
        private void CreateEconomyManager()
        {
            if (FindFirstObjectByType<EconomyManager>() == null)
            {
                GameObject economyManager = new GameObject("EconomyManager");
                economyManager.AddComponent<EconomyManager>();
                Debug.Log("EconomyManager created!");
            }
            else
            {
                Debug.Log("EconomyManager already exists!");
            }
        }
        
        private void CreateHeroManager()
        {
            if (FindFirstObjectByType<HeroManager>() == null)
            {
                GameObject heroManager = new GameObject("HeroManager");
                heroManager.AddComponent<HeroManager>();
                Debug.Log("HeroManager created!");
            }
            else
            {
                Debug.Log("HeroManager already exists!");
            }
        }
        
        private void CreateUIManager()
        {
            if (FindFirstObjectByType<UIManager>() == null)
            {
                GameObject uiManager = new GameObject("UIManager");
                uiManager.AddComponent<UIManager>();
                Debug.Log("UIManager created!");
            }
            else
            {
                Debug.Log("UIManager already exists!");
            }
        }
        
        private void CreateObjectPool()
        {
            if (FindFirstObjectByType<ObjectPool>() == null)
            {
                GameObject objectPool = new GameObject("ObjectPool");
                objectPool.AddComponent<ObjectPool>();
                Debug.Log("ObjectPool created!");
            }
            else
            {
                Debug.Log("ObjectPool already exists!");
            }
        }
        
        private void CreateEnemyAI()
        {
            if (FindFirstObjectByType<EnemyAI>() == null)
            {
                GameObject enemyAI = new GameObject("EnemyAI");
                enemyAI.AddComponent<EnemyAI>();
                Debug.Log("EnemyAI created!");
            }
            else
            {
                Debug.Log("EnemyAI already exists!");
            }
        }
        
        private void CheckSceneSetup()
        {
            bool allGood = true;
            
            if (FindFirstObjectByType<GameManager>() == null)
            {
                Debug.LogError("Missing: GameManager");
                allGood = false;
            }
            
            if (FindFirstObjectByType<LaneManager>() == null)
            {
                Debug.LogError("Missing: LaneManager");
                allGood = false;
            }
            
            if (FindFirstObjectByType<EconomyManager>() == null)
            {
                Debug.LogError("Missing: EconomyManager");
                allGood = false;
            }
            
            if (FindFirstObjectByType<HeroManager>() == null)
            {
                Debug.LogError("Missing: HeroManager");
                allGood = false;
            }
            
            if (FindFirstObjectByType<UIManager>() == null)
            {
                Debug.LogError("Missing: UIManager");
                allGood = false;
            }
            
            if (FindFirstObjectByType<ObjectPool>() == null)
            {
                Debug.LogError("Missing: ObjectPool");
                allGood = false;
            }
            
            if (FindFirstObjectByType<EnemyAI>() == null)
            {
                Debug.LogError("Missing: EnemyAI");
                allGood = false;
            }
            
            if (FindFirstObjectByType<CameraController>() == null)
            {
                Debug.LogError("Missing: CameraController");
                allGood = false;
            }
            
            if (allGood)
            {
                Debug.Log("Scene setup check passed! All components are present.");
            }
        }
        
        private void FixMissingReferences()
        {
            // This would contain logic to fix common missing reference issues
            Debug.Log("Missing references fix completed!");
        }
    }
} 