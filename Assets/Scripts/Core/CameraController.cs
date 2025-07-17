using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetaEarthStrike.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        public float moveSpeed = 10f;
        public float zoomSpeed = 5f;
        public float minZoom = 5f;
        public float maxZoom = 20f;
        public float panSpeed = 15f;
        
        [Header("Boundaries")]
        public float minX = -50f;
        public float maxX = 50f;
        public float minZ = -50f;
        public float maxZ = 50f;
        
        [Header("Target Following")]
        public Transform target;
        public float followSpeed = 5f;
        public Vector3 offset = new Vector3(0, 10, -10);
        
        [Header("Input")]
        public KeyCode resetCameraKey = KeyCode.Space;
        public bool enableTouchPan = true;
        public float touchPanSensitivity = 1f;
        
        private Camera cam;
        private Vector3 targetPosition;
        private bool isFollowingTarget = false;
        
        private void Awake()
        {
            cam = GetComponent<Camera>();
            targetPosition = transform.position;
        }
        
        private void Start()
        {
            // Set initial camera position for 2.5D view
            SetupIsometricView();
        }
        
        private void Update()
        {
            if (GameManager.Instance.currentGameState == GameState.Gameplay)
            {
                HandleInput();
                UpdateCamera();
            }
        }
        
        private void SetupIsometricView()
        {
            // Set up isometric camera angle
            transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            
            // Position camera to see the battlefield
            Vector3 centerPosition = Vector3.zero;
            centerPosition.y = 15f;
            centerPosition.z = -15f;
            
            transform.position = centerPosition;
            targetPosition = centerPosition;
        }
        
        private void HandleInput()
        {
            // Touch pan for mobile
            if (enableTouchPan && Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                
                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 panDirection = new Vector3(-touch.deltaPosition.x, 0, -touch.deltaPosition.y);
                    targetPosition += panDirection * touchPanSensitivity * Time.deltaTime;
                    isFollowingTarget = false;
                }
            }
            
            // Mouse pan for testing in editor
            if (Input.GetMouseButton(0) && Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                Vector3 panDirection = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y"));
                targetPosition += panDirection * panSpeed * Time.deltaTime;
                isFollowingTarget = false;
            }
            
            // Zoom with mouse wheel or pinch
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                float newSize = cam.orthographicSize - scroll * zoomSpeed;
                cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
            
            // Pinch zoom for mobile
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);
                
                Vector2 touch0Prev = touch0.position - touch0.deltaPosition;
                Vector2 touch1Prev = touch1.position - touch1.deltaPosition;
                
                float prevTouchDeltaMag = (touch0Prev - touch1Prev).magnitude;
                float touchDeltaMag = (touch0.position - touch1.position).magnitude;
                
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                
                float newSize = cam.orthographicSize + deltaMagnitudeDiff * 0.01f;
                cam.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
            
            // Reset camera
            if (Input.GetKeyDown(resetCameraKey))
            {
                ResetCamera();
            }
            
            // Follow target (hero) when right-clicking or double tap
            if (Input.GetMouseButtonDown(1) || (Input.touchCount == 1 && Input.GetTouch(0).tapCount == 2))
            {
                ToggleTargetFollowing();
            }
        }
        
        private void UpdateCamera()
        {
            if (isFollowingTarget && target != null)
            {
                // Follow target with offset
                Vector3 desiredPosition = target.position + offset;
                targetPosition = desiredPosition;
            }
            
            // Clamp position to boundaries
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);
            
            // Smoothly move camera to target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                isFollowingTarget = true;
            }
        }
        
        public void ClearTarget()
        {
            target = null;
            isFollowingTarget = false;
        }
        
        public void ToggleTargetFollowing()
        {
            if (target != null)
            {
                isFollowingTarget = !isFollowingTarget;
            }
        }
        
        public void ResetCamera()
        {
            SetupIsometricView();
            isFollowingTarget = false;
        }
        
        public void FocusOnPosition(Vector3 position)
        {
            targetPosition = position + offset;
            isFollowingTarget = false;
        }
        
        public void SetCameraBounds(float minX, float maxX, float minZ, float maxZ)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minZ = minZ;
            this.maxZ = maxZ;
        }
        
        public void ShakeCamera(float intensity, float duration)
        {
            StartCoroutine(CameraShake(intensity, duration));
        }
        
        private IEnumerator CameraShake(float intensity, float duration)
        {
            Vector3 originalPosition = transform.position;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * intensity;
                float z = Random.Range(-1f, 1f) * intensity;
                
                transform.position = originalPosition + new Vector3(x, 0, z);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.position = originalPosition;
        }
        
        // Screen to world position conversion for mouse input
        public Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -transform.position.z;
            return cam.ScreenToWorldPoint(mousePos);
        }
        
        // Check if a world position is visible on screen
        public bool IsPositionVisible(Vector3 worldPosition)
        {
            Vector3 screenPoint = cam.WorldToScreenPoint(worldPosition);
            return screenPoint.x >= 0 && screenPoint.x <= Screen.width &&
                   screenPoint.y >= 0 && screenPoint.y <= Screen.height &&
                   screenPoint.z > 0;
        }
    }
} 