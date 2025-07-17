using UnityEngine;

namespace Game2D5
{
    /// <summary>
    /// 2.5D Movement Controller
    /// Handles movement in X and Y axes while maintaining fixed Z position for layering
    /// </summary>
    public class Movement2D5 : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        
        [Header("2.5D Layer Settings")]
        public float zPosition = 0f; // Fixed Z position for this object's layer
        public bool constrainToLayer = true; // Whether to lock Z position
        
        [Header("Input Settings")]
        public KeyCode jumpKey = KeyCode.Space;
        public bool useRigidbody = true;
        
        private Rigidbody rb;
        private bool isGrounded;
        private Vector3 targetPosition;
        
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            targetPosition = transform.position;
            
            // Set initial Z position
            if (constrainToLayer)
            {
                Vector3 pos = transform.position;
                pos.z = zPosition;
                transform.position = pos;
            }
            
            // Configure Rigidbody for 2.5D
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                if (constrainToLayer)
                {
                    rb.constraints |= RigidbodyConstraints.FreezePositionZ;
                }
            }
        }
        
        void Update()
        {
            HandleInput();
            
            if (!useRigidbody)
            {
                MoveWithTransform();
            }
            
            // Always maintain Z position if constrained
            if (constrainToLayer && transform.position.z != zPosition)
            {
                Vector3 pos = transform.position;
                pos.z = zPosition;
                transform.position = pos;
            }
        }
        
        void HandleInput()
        {
            // Get input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            if (useRigidbody && rb != null)
            {
                // Rigidbody movement
                Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed;
                rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
                
                // Jump
                if (Input.GetKeyDown(jumpKey) && isGrounded)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            }
            else
            {
                // Transform movement
                targetPosition += new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
                
                // Simple jump for transform movement
                if (Input.GetKeyDown(jumpKey) && transform.position.y <= 0.6f)
                {
                    targetPosition.y += jumpForce * Time.deltaTime;
                }
            }
        }
        
        void MoveWithTransform()
        {
            // Smooth movement towards target
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
            
            // Apply gravity for transform movement
            if (transform.position.y > 0.5f)
            {
                targetPosition.y -= 9.81f * Time.deltaTime;
            }
            else
            {
                targetPosition.y = 0.5f;
            }
        }
        
        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.name.Contains("Ground"))
            {
                isGrounded = true;
            }
        }
        
        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.name.Contains("Ground"))
            {
                isGrounded = false;
            }
        }
        
        void OnDrawGizmos()
        {
            // Draw the layer constraint
            if (constrainToLayer)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, new Vector3(0.1f, 0.1f, 0.1f));
                
                // Draw Z layer line
                Gizmos.color = Color.red;
                Vector3 start = transform.position + Vector3.left * 2f;
                Vector3 end = transform.position + Vector3.right * 2f;
                start.z = zPosition;
                end.z = zPosition;
                Gizmos.DrawLine(start, end);
            }
        }
    }
}