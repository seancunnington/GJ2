using UnityEngine;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class CharacterPhysics : MonoBehaviour
{
     #region Public Variables

     [Header("Character Stats")]
     [Range(0f, 100f)] public float maxSpeed = 6f;
     [Range(0f, 100f)] public float acceleration = 2f;

     public LayerMask surfaceDetectionMask;

     #endregion

     #region Private Variables

     private Rigidbody _rigidbody;
     private CapsuleCollider _collider;

     //private Vector3 direction = new Vector3();
     Vector3 velocity, desiredVelocity;
     //private Vector3 speed = new Vector3();

     private Vector3 surfaceDetectionRay = new Vector3();
     [SerializeField] private float rayDistance = 1.5f;
     [SerializeField] private float rideHeight = 1;
     [SerializeField] private float rideSpringStrength = 1f;
     [SerializeField] private float rideSpringDamper = 1f;

     #endregion



     private void Awake()
     {
          _rigidbody = GetComponent<Rigidbody>();
          _collider = GetComponent<CapsuleCollider>();
     }


     private void Update()
     {
          surfaceDetectionRay = transform.position - new Vector3(0, rayDistance, 0);

          // Input
          Vector2 playerInput = new Vector2();
          playerInput.x = Input.GetAxis("Horizontal");
          playerInput.y = Input.GetAxis("Vertical");
          playerInput = Vector2.ClampMagnitude(playerInput, 1f);

          // Set the desired velocity
          desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
     }


     private void FixedUpdate()
     {
          // Keep track of PhsyX velocity
          velocity = _rigidbody.velocity;

          // Apply movement
          AdjustVelocity();

          // Update PhysX velocity
          _rigidbody.velocity = velocity;

          LevitateObject();
          //MoveObject();
     }


     void AdjustVelocity()
     {
          Vector3 xAxis = Vector3.right;
          Vector3 zAxis = Vector3.forward;

          float currentX = Vector3.Dot(velocity, xAxis);
          float currentZ = Vector3.Dot(velocity, zAxis);

          float maxSpeedChange = acceleration * Time.deltaTime;

          float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
          float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

          velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
     }


     private void LevitateObject()
     {
          RaycastHit rayHit;
          bool rayDidHit = Physics.Raycast(transform.position, Vector3.down, out rayHit, rayDistance, surfaceDetectionMask);
          
          if (rayDidHit)
          {
               Vector3 velocity = _rigidbody.velocity;
               Vector3 rayDirection = Vector3.down;

               float rayDirectVelocity = Vector3.Dot(rayDirection, velocity);
               float x = rayHit.distance - rideHeight;

               // Set the spring force
               float springForce = (x * rideSpringStrength) - (rayDirectVelocity * rideSpringDamper);

               // Apply the force
               _rigidbody.AddForce(rayDirection * springForce);
          }
     }


     private void MoveObject()
     {



          //_rigidbody.AddForce(direction * maxSpeed, ForceMode.VelocityChange);
     }


     public void SetDirection(Vector2 input)
     {
          //direction = new Vector3(input.x, 0f, input.y);
     }
}
