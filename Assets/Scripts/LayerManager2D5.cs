using UnityEngine;
using System.Collections.Generic;

namespace Game2D5
{
    /// <summary>
    /// 2.5D Layer Manager
    /// Manages depth layers for 2.5D games and provides easy layer switching
    /// </summary>
    public class LayerManager2D5 : MonoBehaviour
    {
        [Header("2.5D Layer Configuration")]
        [SerializeField] private LayerData[] layers;
        
        [Header("Runtime Settings")]
        public bool autoSortOnStart = true;
        public bool debugMode = false;
        
        private Dictionary<string, float> layerDepths = new Dictionary<string, float>();
        
        [System.Serializable]
        public class LayerData
        {
            public string layerName;
            public float zDepth;
            public Color debugColor = Color.white;
            public List<GameObject> objectsInLayer = new List<GameObject>();
        }
        
        void Start()
        {
            InitializeLayers();
            
            if (autoSortOnStart)
            {
                SortAllLayers();
            }
        }
        
        void InitializeLayers()
        {
            layerDepths.Clear();
            
            foreach (var layer in layers)
            {
                layerDepths[layer.layerName] = layer.zDepth;
            }
        }
        
        /// <summary>
        /// Move an object to a specific layer
        /// </summary>
        public void MoveToLayer(GameObject obj, string layerName)
        {
            if (layerDepths.ContainsKey(layerName))
            {
                Vector3 pos = obj.transform.position;
                pos.z = layerDepths[layerName];
                obj.transform.position = pos;
                
                // Update layer data
                foreach (var layer in layers)
                {
                    layer.objectsInLayer.Remove(obj);
                    if (layer.layerName == layerName)
                    {
                        layer.objectsInLayer.Add(obj);
                    }
                }
                
                if (debugMode)
                {
                    Debug.Log($"Moved {obj.name} to layer '{layerName}' at Z depth {layerDepths[layerName]}");
                }
            }
            else
            {
                Debug.LogWarning($"Layer '{layerName}' not found!");
            }
        }
        
        /// <summary>
        /// Get the Z depth of a specific layer
        /// </summary>
        public float GetLayerDepth(string layerName)
        {
            return layerDepths.ContainsKey(layerName) ? layerDepths[layerName] : 0f;
        }
        
        /// <summary>
        /// Sort all objects in all layers
        /// </summary>
        public void SortAllLayers()
        {
            foreach (var layer in layers)
            {
                foreach (var obj in layer.objectsInLayer)
                {
                    if (obj != null)
                    {
                        Vector3 pos = obj.transform.position;
                        pos.z = layer.zDepth;
                        obj.transform.position = pos;
                    }
                }
            }
        }
        
        /// <summary>
        /// Create a new layer at runtime
        /// </summary>
        public void CreateLayer(string layerName, float zDepth)
        {
            if (!layerDepths.ContainsKey(layerName))
            {
                layerDepths[layerName] = zDepth;
                
                // Add to layers array
                System.Array.Resize(ref layers, layers.Length + 1);
                layers[layers.Length - 1] = new LayerData
                {
                    layerName = layerName,
                    zDepth = zDepth,
                    objectsInLayer = new List<GameObject>()
                };
            }
        }
        
        /// <summary>
        /// Get all layer names
        /// </summary>
        public string[] GetLayerNames()
        {
            string[] names = new string[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                names[i] = layers[i].layerName;
            }
            return names;
        }
        
        void OnDrawGizmos()
        {
            if (debugMode && layers != null)
            {
                foreach (var layer in layers)
                {
                    Gizmos.color = layer.debugColor;
                    
                    // Draw layer plane
                    Vector3 center = new Vector3(0, 0, layer.zDepth);
                    Vector3 size = new Vector3(20, 0.1f, 20);
                    Gizmos.DrawWireCube(center, size);
                    
                    // Draw objects in layer
                    foreach (var obj in layer.objectsInLayer)
                    {
                        if (obj != null)
                        {
                            Gizmos.color = layer.debugColor;
                            Gizmos.DrawWireSphere(obj.transform.position, 0.5f);
                        }
                    }
                }
            }
        }
    }
}