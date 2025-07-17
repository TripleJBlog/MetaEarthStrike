using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MetaEarthStrike.Core;
using UnityEngine.UI;
using MetaEarthStrike.UI;

namespace MetaEarthStrike.Editor
{
    public class UnitPrefabCreator : EditorWindow
    {
        [MenuItem("Meta Earth Strike/Create Unit Prefab")]
        public static void ShowWindow()
        {
            GetWindow<UnitPrefabCreator>("Unit Prefab Creator");
        }
        
        private string unitName = "New Unit";
        private UnitType unitType = UnitType.Melee;
        private FactionType faction = FactionType.Alliance;
        private int maxHealth = 100;
        private int damage = 20;
        private float attackRange = 1f;
        private float attackSpeed = 1f;
        private float moveSpeed = 3f;
        private Color unitColor = Color.white;
        private bool createHealthBar = true;
        private bool createAnimator = true;
        
        public static readonly Color lime = new Color(0.0f, 1.0f, 0.0f);
        public static readonly Color lightblue = new Color(0.678f, 0.847f, 0.902f);
        public static readonly Color darkgreen = new Color(0.0f, 0.392f, 0.0f);
        
        private void OnGUI()
        {
            GUILayout.Label("Unit Prefab Creator", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            // Unit basic info
            GUILayout.Label("Unit Information:", EditorStyles.boldLabel);
            unitName = EditorGUILayout.TextField("Unit Name:", unitName);
            unitType = (UnitType)EditorGUILayout.EnumPopup("Unit Type:", unitType);
            faction = (FactionType)EditorGUILayout.EnumPopup("Faction:", faction);
            
            GUILayout.Space(10);
            
            // Unit stats
            GUILayout.Label("Unit Stats:", EditorStyles.boldLabel);
            maxHealth = EditorGUILayout.IntField("Max Health:", maxHealth);
            damage = EditorGUILayout.IntField("Damage:", damage);
            attackRange = EditorGUILayout.FloatField("Attack Range:", attackRange);
            attackSpeed = EditorGUILayout.FloatField("Attack Speed:", attackSpeed);
            moveSpeed = EditorGUILayout.FloatField("Move Speed:", moveSpeed);
            
            GUILayout.Space(10);
            
            // Visual settings
            GUILayout.Label("Visual Settings:", EditorStyles.boldLabel);
            unitColor = EditorGUILayout.ColorField("Unit Color:", unitColor);
            createHealthBar = EditorGUILayout.Toggle("Create Health Bar:", createHealthBar);
            createAnimator = EditorGUILayout.Toggle("Create Animator:", createAnimator);
            
            GUILayout.Space(20);
            
            if (GUILayout.Button("Create Unit Prefab"))
            {
                CreateUnitPrefab();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Create All Basic Units"))
            {
                CreateAllBasicUnits();
            }
        }
        
        private void CreateUnitPrefab()
        {
            // Create the unit GameObject
            GameObject unitObject = new GameObject(unitName);
            
            // Add SpriteRenderer
            SpriteRenderer spriteRenderer = unitObject.AddComponent<SpriteRenderer>();
            spriteRenderer.color = unitColor;
            
            // Create a simple sprite (white square)
            Texture2D texture = CreateSimpleTexture(64, 64, unitColor);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = sprite;
            
            // Add Unit component
            Unit unit = unitObject.AddComponent<Unit>();
            unit.unitName = unitName;
            unit.unitType = unitType;
            unit.maxHealth = maxHealth;
            unit.damage = damage;
            unit.attackRange = attackRange;
            unit.attackSpeed = attackSpeed;
            unit.moveSpeed = moveSpeed;
            
            // Add Rigidbody2D for physics
            Rigidbody2D rb = unitObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            // Add Collider2D
            BoxCollider2D collider = unitObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 1f);
            
            // Create health bar if requested
            if (createHealthBar)
            {
                CreateHealthBar(unitObject);
            }
            
            // Create animator if requested
            if (createAnimator)
            {
                CreateAnimator(unitObject);
            }
            
            // Create prefab
            string prefabPath = $"Assets/Prefabs/Units/{unitName}.prefab";
            
            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(prefabPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            
            // Create the prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(unitObject, prefabPath);
            
            // Destroy the scene object
            DestroyImmediate(unitObject);
            
            // Select the created prefab
            Selection.activeObject = prefab;
            
            Debug.Log($"Unit prefab created: {prefabPath}");
        }
        
        private void CreateAllBasicUnits()
        {
            // Create basic units for each faction and type (single lane focused)
            CreateBasicUnit("Alliance Melee", UnitType.Melee, FactionType.Alliance, Color.blue);
            CreateBasicUnit("Alliance Ranged", UnitType.Ranged, FactionType.Alliance, Color.cyan);
            CreateBasicUnit("Alliance Siege", UnitType.Siege, FactionType.Alliance, lightblue);
            
            CreateBasicUnit("Horde Melee", UnitType.Melee, FactionType.Horde, Color.red);
            CreateBasicUnit("Horde Ranged", UnitType.Ranged, FactionType.Horde, lime);
            CreateBasicUnit("Horde Siege", UnitType.Siege, FactionType.Horde, Color.yellow);
            
            CreateBasicUnit("Scourge Melee", UnitType.Melee, FactionType.Scourge, Color.green);
            CreateBasicUnit("Scourge Ranged", UnitType.Ranged, FactionType.Scourge, lime);
            CreateBasicUnit("Scourge Siege", UnitType.Siege, FactionType.Scourge, darkgreen);
            
            Debug.Log("All basic units created for single lane 1v1!");
        }
        
        private void CreateBasicUnit(string name, UnitType type, FactionType faction, Color color)
        {
            // Set stats based on unit type
            int health = 100;
            int damage = 20;
            float range = 1f;
            float speed = 3f;
            
            switch (type)
            {
                case UnitType.Melee:
                    health = 150;
                    damage = 25;
                    range = 1f;
                    speed = 2.5f;
                    break;
                case UnitType.Ranged:
                    health = 80;
                    damage = 30;
                    range = 3f;
                    speed = 3f;
                    break;
                case UnitType.Siege:
                    health = 60;
                    damage = 50;
                    range = 4f;
                    speed = 2f;
                    break;
            }
            
            // Create the unit
            GameObject unitObject = new GameObject(name);
            
            // Add SpriteRenderer
            SpriteRenderer spriteRenderer = unitObject.AddComponent<SpriteRenderer>();
            spriteRenderer.color = color;
            
            // Create sprite
            Texture2D texture = CreateSimpleTexture(64, 64, color);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
            spriteRenderer.sprite = sprite;
            
            // Add Unit component
            Unit unit = unitObject.AddComponent<Unit>();
            unit.unitName = name;
            unit.unitType = type;
            unit.maxHealth = health;
            unit.damage = damage;
            unit.attackRange = range;
            unit.attackSpeed = 1f;
            unit.moveSpeed = speed;
            
            // Add physics components
            Rigidbody2D rb = unitObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            BoxCollider2D collider = unitObject.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 1f);
            
            // Create health bar
            CreateHealthBar(unitObject);
            
            // Create animator
            CreateAnimator(unitObject);
            
            // Save as prefab
            string prefabPath = $"Assets/Prefabs/Units/{name}.prefab";
            
            // Ensure directory exists
            string directory = System.IO.Path.GetDirectoryName(prefabPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(unitObject, prefabPath);
            DestroyImmediate(unitObject);
        }
        
        private void CreateHealthBar(GameObject parent)
        {
            // Create health bar GameObject
            GameObject healthBarObj = new GameObject("HealthBar");
            healthBarObj.transform.SetParent(parent.transform);
            healthBarObj.transform.localPosition = new Vector3(0, 1.5f, 0);
            
            // Add Canvas
            Canvas canvas = healthBarObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            // Add CanvasScaler
            CanvasScaler scaler = healthBarObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add GraphicRaycaster
            healthBarObj.AddComponent<GraphicRaycaster>();
            
            // Create background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(healthBarObj.transform);
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = new Vector3(1.2f, 0.3f, 1f);
            
            UnityEngine.UI.Image bgImage = background.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = Color.black;
            
            // Create fill
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(background.transform);
            fill.transform.localPosition = Vector3.zero;
            fill.transform.localScale = Vector3.one;
            
            UnityEngine.UI.Image fillImage = fill.AddComponent<UnityEngine.UI.Image>();
            fillImage.color = Color.green;
            
            // Create Slider
            GameObject sliderObj = new GameObject("Slider");
            sliderObj.transform.SetParent(healthBarObj.transform);
            sliderObj.transform.localPosition = Vector3.zero;
            
            Slider slider = sliderObj.AddComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.value = 1f;
            
            // Add HealthBar component
            HealthBar healthBar = healthBarObj.AddComponent<HealthBar>();
            healthBar.healthSlider = slider;
            healthBar.fillImage = fillImage;
        }
        
        private void CreateAnimator(GameObject parent)
        {
            // Create Animator
            Animator animator = parent.AddComponent<Animator>();
            
            // Create a simple AnimatorController
            UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"Assets/Animations/{parent.name}Controller.controller");
            
            // Add basic states
            UnityEditor.Animations.AnimatorState idleState = controller.layers[0].stateMachine.AddState("Idle");
            UnityEditor.Animations.AnimatorState walkState = controller.layers[0].stateMachine.AddState("Walk");
            UnityEditor.Animations.AnimatorState attackState = controller.layers[0].stateMachine.AddState("Attack");
            UnityEditor.Animations.AnimatorState hitState = controller.layers[0].stateMachine.AddState("Hit");
            
            // Set idle as default
            controller.layers[0].stateMachine.defaultState = idleState;
            
            // Assign controller to animator
            animator.runtimeAnimatorController = controller;
        }
        
        private Texture2D CreateSimpleTexture(int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return texture;
        }
    }
} 