using UnityEngine;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody))]
public class CharacterPhysics : MonoBehaviour
{
     #region Public Variables

     [Header("Character Stats")]
     [Range(0f, 100f)] public float maxSpeed = 6f;
     [Range(0f, 100f)] public float acceleration = 2f;

     public LayerMask surfaceDetectionMask;

     public bool allowMove = true;
     public bool allowLevitate = true;
     public float coolDownTimeAmount = 10f;
     public float coolDownTimer = 0f;

     #endregion

     #region Private Variables

     private Rigidbody _rigidbody;
     private CapsuleCollider _collider;

     Vector3 playerInput;
     Vector3 velocity, desiredVelocity;

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

          if (allowMove)
          {
               playerInput.x = Input.GetAxis("Horizontal");
               playerInput.y = Input.GetAxis("Vertical");
               playerInput = Vector2.ClampMagnitude(playerInput, 1f);
          }
          else
               playerInput = Vector2.zero;

          // Set the desired velocity
          desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

          // Reduce the cooldown timer
          if (coolDownTimer > 0f)
               coolDownTimer -= Time.deltaTime;

          if (coolDownTimer < 0.2)
               allowLevitate = true;
     }


     private void FixedUpdate()
     {
          // Keep track of PhsyX velocity
          velocity = _rigidbody.velocity;

          // Apply movement
          AdjustVelocity();

          // Update PhysX velocity
          _rigidbody.velocity = velocity;

          if (allowLevitate)
               LevitateObject();
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


     public void DisableLevitate()
     {
          allowLevitate = false;
          coolDownTimer = coolDownTimeAmount;
     }

}
