using UnityEngine;

namespace Game2D5
{
    /// <summary>
    /// 2.5D Camera Controller
    /// Handles camera following and bounds in 2.5D games
    /// </summary>
    public class CameraController2D5 : MonoBehaviour
    {
        [Header("Follow Settings")]
        public Transform target;
        public float followSpeed = 5f;
        public Vector3 offset = new Vector3(0, 5, -10);
        
        [Header("2.5D Camera Settings")]
        public bool maintainZPosition = true;
        public float fixedZPosition = -10f;
        public bool orthographic = true;
        public float orthographicSize = 8f;
        
        [Header("Bounds")]
        public bool useBounds = false;
        public Vector2 minBounds = new Vector2(-10, -10);
        public Vector2 maxBounds = new Vector2(10, 10);
        
        [Header("Smoothing")]
        public bool useSmoothing = true;
        public float smoothTime = 0.3f;
        
        private Camera cam;
        private Vector3 velocity = Vector3.zero;
        private Vector3 targetPosition;
        
        void Start()
        {
            cam = GetComponent<Camera>();
            
            if (cam != null)
            {
                cam.orthographic = orthographic;
                if (orthographic)
                {
                    cam.orthographicSize = orthographicSize;
                }
            }
            
            // Find target if not assigned
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    player = GameObject.Find("Player_Layer");
                }
                if (player != null)
                {
                    target = player.transform;
                }
            }
        }
        
        void LateUpdate()
        {
            if (target == null) return;
            
            UpdateCameraPosition();
        }
        
        void UpdateCameraPosition()
        {
            // Calculate target position
            targetPosition = target.position + offset;
            
            // Apply bounds if enabled
            if (useBounds)
            {
                targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
            }
            
            // Maintain fixed Z position for 2.5D
            if (maintainZPosition)
            {
                targetPosition.z = fixedZPosition;
            }
            
            // Apply movement
            if (useSmoothing)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Set a new target to follow
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        /// <summary>
        /// Set camera bounds
        /// </summary>
        public void SetBounds(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
            useBounds = true;
        }
        
        /// <summary>
        /// Disable bounds
        /// </summary>
        public void DisableBounds()
        {
            useBounds = false;
        }
        
        /// <summary>
        /// Get screen position from world position (useful for UI placement)
        /// </summary>
        public Vector3 WorldToScreenPoint(Vector3 worldPosition)
        {
            if (cam != null)
            {
                return cam.WorldToScreenPoint(worldPosition);
            }
            return Vector3.zero;
        }
        
        void OnDrawGizmos()
        {
            if (useBounds)
            {
                Gizmos.color = Color.green;
                Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2f, (minBounds.y + maxBounds.y) / 2f, 0);
                Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0.1f);
                Gizmos.DrawWireCube(center, size);
            }
            
            if (target != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(target.position, 0.5f);
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
}