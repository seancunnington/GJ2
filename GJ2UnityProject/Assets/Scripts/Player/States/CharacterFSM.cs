using UnityEngine;

[RequireComponent(typeof(CharacterPhysics))]
public class CharacterFSM : MonoBehaviour
{

     public enum Character
     {
          Left,
          Right
     }
     public Character character = Character.Left;

     public enum State
     {
          Moving,
          Planting
     }
     public State state = State.Moving;

     public GameObject leftArchPoint;
     public GameObject rightArchPoint;

     [Header("Planting Components")]
     public GameObject seed;
     public GameObject PlantingReticle;
     [Range(0f, 10f)] public float reticleOffset;
     [Range(0f, 5f)] public float reticleHeight;
     [Range(0f, 5f)] public float reticleCancelWeight;
     [Range(0f, 500f)] public float plantingHeightCheck;
     public LayerMask surfaceDetectionMask;

     CharacterPhysics _characterPhysics;





     private void Awake()
     {
          _characterPhysics = GetComponent<CharacterPhysics>();
          PlantingReticle.SetActive(false);
     }


     private void Update()
     {
          if (state == State.Moving)
          {
               // Allow movement
               _characterPhysics.allowMove = true;


               // Transitions
               if (Input.GetButtonDown("Plant"))
               {
                    // Maybe don't allow movement?
                    _characterPhysics.allowMove = false;

                    state = State.Planting;
               }
          }

          if (state == State.Planting)
          {
               if (character == Character.Left)
               {
                    // Set planting reticle
                    PlantingReticle.SetActive(true);
                    SetPlantingReticle();
               }
               
               // Transitions
               if (Input.GetButtonUp("Plant"))
               {
                    if (character == Character.Left)
                    {
                         // See if we planted something
                         if (CheckForAllowedPlanting())
                         {
                              PlantSeed();
                         }
                    }

                    // Hide planting reticle
                    PlantingReticle.SetActive(false);

                    // Transition
                    state = State.Moving;
               }
          }
     }


     void PlantSeed()
     {
          // Get the difference of position from the Left Char and Left Arch
          Vector3 distanceFromLeftArch = PlantingReticle.transform.position - leftArchPoint.transform.position;

          // Apply that difference to the Right Arch position
          Vector3 positionFromRightArch = distanceFromLeftArch + rightArchPoint.transform.position;
          Vector3 seedPosition = FindPlantingHeight(positionFromRightArch);

          // Create a seed at the new position
          GameObject newSeed = Instantiate(seed, seedPosition, seed.transform.rotation);
     }


     Vector3 FindPlantingHeight(Vector3 plantPosition)
     {
          RaycastHit rayHit;
          Vector3 startPosition = plantPosition + new Vector3(0, plantingHeightCheck, 0);

          // Find the first ground from the top down.
          bool rayDown = Physics.Raycast(startPosition, Vector3.down, out rayHit, plantingHeightCheck*2, surfaceDetectionMask);

          if (rayDown)
               return rayHit.point;

          // default
          return Vector3.zero;
     }


     bool CheckForAllowedPlanting()
     {
          if (Mathf.Abs(PlantingReticle.transform.localPosition.x) > reticleCancelWeight)
               return true;

          else if (Mathf.Abs(PlantingReticle.transform.localPosition.z) > reticleCancelWeight)
               return true;

          else
               return false;
     }


     void SetPlantingReticle()
     {
          Vector3 newReticlePosition = transform.position + (GetDirection() * reticleOffset);
          newReticlePosition += new Vector3(0, -reticleHeight, 0);

          PlantingReticle.transform.position = newReticlePosition;    
     }


     Vector3 GetDirection()
     {
          Vector2 playerInput = new Vector2();

          playerInput.x = Input.GetAxis("Horizontal");
          playerInput.y = Input.GetAxis("Vertical");
          playerInput = Vector2.ClampMagnitude(playerInput, 1f);

          return new Vector3(playerInput.x, 0f, playerInput.y);
     }
}
