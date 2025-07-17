using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaEarthStrike.Core
{
    [System.Serializable]
    public class Lane
    {
        public int laneIndex;
        public Transform playerSpawnPoint;
        public Transform enemySpawnPoint;
        public Transform playerBase;
        public Transform enemyBase;
        public List<Transform> waypoints = new List<Transform>();
        public List<Unit> activeUnits = new List<Unit>();
        public float spawnInterval = 5f;
        public float lastSpawnTime;
    }

    public class LaneManager : MonoBehaviour
    {
        public static LaneManager Instance { get; private set; }
        [Header("Lane Settings")]
        public GameObject lanePrefab;
        public float laneSpacing = 10f;
        public Vector3 laneDirection = Vector3.right;
        
        [Header("Unit Spawning")]
        public GameObject[] playerUnitPrefabs;
        public GameObject[] enemyUnitPrefabs;
        public float spawnInterval = 5f;
        
        [Header("Lanes")]
        public List<Lane> lanes = new List<Lane>();
        
        [Header("References")]
        [SerializeField] private ObjectPool unitPool;
        private int maxLanes = 1; // Single lane 1v1
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }
        
        public void InitializeLanes(int laneCount)
        {
            maxLanes = laneCount;
            CreateLanes();
        }
        
        private void CreateLanes()
        {
            // Clear existing lanes
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
            lanes.Clear();
            
            // Create new lanes
            for (int i = 0; i < maxLanes; i++)
            {
                CreateLane(i);
            }
        }
        
        private void CreateLane(int laneIndex)
        {
            GameObject laneObject = new GameObject($"Lane_{laneIndex}");
            laneObject.transform.SetParent(transform);
            
            // Position lane (single lane centered)
            Vector3 lanePosition = Vector3.zero;
            laneObject.transform.position = lanePosition;
            
            // Create spawn points
            GameObject playerSpawn = new GameObject("PlayerSpawn");
            GameObject enemySpawn = new GameObject("EnemySpawn");
            GameObject playerBase = new GameObject("PlayerBase");
            GameObject enemyBase = new GameObject("EnemyBase");
            
            playerSpawn.transform.SetParent(laneObject.transform);
            enemySpawn.transform.SetParent(laneObject.transform);
            playerBase.transform.SetParent(laneObject.transform);
            enemyBase.transform.SetParent(laneObject.transform);
            
            // Position spawn points and bases
            playerSpawn.transform.position = lanePosition + Vector3.left * 20f;
            enemySpawn.transform.position = lanePosition + Vector3.right * 20f;
            playerBase.transform.position = lanePosition + Vector3.left * 25f;
            enemyBase.transform.position = lanePosition + Vector3.right * 25f;
            
            // Create waypoints
            List<Transform> waypoints = new List<Transform>();
            int waypointCount = 5;
            for (int j = 0; j < waypointCount; j++)
            {
                GameObject waypoint = new GameObject($"Waypoint_{j}");
                waypoint.transform.SetParent(laneObject.transform);
                
                float t = (float)j / (waypointCount - 1);
                Vector3 waypointPos = Vector3.Lerp(
                    playerSpawn.transform.position,
                    enemySpawn.transform.position,
                    t
                );
                waypoint.transform.position = waypointPos;
                waypoints.Add(waypoint.transform);
            }
            
            // Create lane data
            Lane lane = new Lane
            {
                laneIndex = laneIndex,
                playerSpawnPoint = playerSpawn.transform,
                enemySpawnPoint = enemySpawn.transform,
                playerBase = playerBase.transform,
                enemyBase = enemyBase.transform,
                waypoints = waypoints,
                spawnInterval = spawnInterval,
                lastSpawnTime = 0f
            };
            
            lanes.Add(lane);
        }
        
        private void Update()
        {
            if (GameManager.Instance.currentGameState == GameState.Gameplay)
            {
                UpdateLaneSpawning();
            }
        }
        
        private void UpdateLaneSpawning()
        {
            foreach (Lane lane in lanes)
            {
                if (Time.time - lane.lastSpawnTime >= lane.spawnInterval)
                {
                    SpawnUnits(lane);
                    lane.lastSpawnTime = Time.time;
                }
            }
        }
        
        private void SpawnUnits(Lane lane)
        {
            // Spawn player unit
            if (playerUnitPrefabs.Length > 0)
            {
                GameObject playerUnitPrefab = playerUnitPrefabs[Random.Range(0, playerUnitPrefabs.Length)];
                SpawnUnit(playerUnitPrefab, lane.playerSpawnPoint.position, lane, true);
            }
            
            // Spawn enemy unit
            if (enemyUnitPrefabs.Length > 0)
            {
                GameObject enemyUnitPrefab = enemyUnitPrefabs[Random.Range(0, enemyUnitPrefabs.Length)];
                SpawnUnit(enemyUnitPrefab, lane.enemySpawnPoint.position, lane, false);
            }
        }
        
        private void SpawnUnit(GameObject unitPrefab, Vector3 position, Lane lane, bool isPlayerUnit)
        {
            GameObject unitObject = unitPool.GetObject(unitPrefab);
            if (unitObject == null)
            {
                unitObject = Instantiate(unitPrefab, position, Quaternion.identity);
            }
            else
            {
                unitObject.transform.position = position;
                unitObject.SetActive(true);
            }
            
            Unit unit = unitObject.GetComponent<Unit>();
            if (unit == null)
            {
                unit = unitObject.AddComponent<Unit>();
            }
            
            unit.Initialize(lane, isPlayerUnit, lane.waypoints);
            lane.activeUnits.Add(unit);
        }
        
        public void RemoveUnitFromLane(Unit unit, Lane lane)
        {
            if (lane.activeUnits.Contains(unit))
            {
                lane.activeUnits.Remove(unit);
            }
        }
        
        public Lane GetLaneByIndex(int index)
        {
            if (index >= 0 && index < lanes.Count)
                return lanes[index];
            return null;
        }
        
        public List<Unit> GetUnitsInLane(int laneIndex)
        {
            Lane lane = GetLaneByIndex(laneIndex);
            return lane?.activeUnits ?? new List<Unit>();
        }
        
        public Transform GetPlayerBase(int laneIndex)
        {
            Lane lane = GetLaneByIndex(laneIndex);
            return lane?.playerBase;
        }
        
        public Transform GetEnemyBase(int laneIndex)
        {
            Lane lane = GetLaneByIndex(laneIndex);
            return lane?.enemyBase;
        }
    }
} 