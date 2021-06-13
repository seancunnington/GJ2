using UnityEngine;
using UnityEngine.UI;

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

     public Texture leftPose;
     public Texture rightPose;
     public Texture backPose;
     public Texture forwardPose;
     public Renderer _renderer;

     public GameObject leftArchPoint;
     public GameObject rightArchPoint;

     [Header("Planting Components")]
     public GameObject seed;
     public GameObject sprout;
     public GameObject PlantingReticle_Left;
     public GameObject PlantingReticle_Right;
     [Range(0f, 10f)] public float reticleOffset;
     public float reticleHeight;
     [Range(0f, 5f)] public float reticleCancelWeight;
     [Range(0f, 500f)] public float plantingHeightCheck;

     public LayerMask surfaceDetectionMask;
     public LayerMask cancelMask;

     public Image inventory;

     public AudioClip creationSound;
     public AudioClip destroySound;
     AudioSource _audioSource;

     CharacterPhysics _characterPhysics;
     LevelManager _levelManager;




     private void Awake()
     {
          if (character == Character.Left)
          {
               seed = null;
               inventory.sprite = null;
               inventory.gameObject.SetActive(false);
          }

          _levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
          _characterPhysics = GetComponent<CharacterPhysics>();
          _audioSource = GetComponent<AudioSource>();

          if (character == Character.Left)
          {
               PlantingReticle_Left.SetActive(false);
               PlantingReticle_Right.SetActive(false);
          }
     }


     private void Update()
     {
          if (state == State.Moving)
          {
               // Allow movement
               _characterPhysics.allowMove = true;

               // Animate
               SimpleAnimate();

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
                    // If not holding a seed, don't allow planting.
                    if (seed != null)
                    {
                         PlantingReticle_Left.SetActive(true);
                         PlantingReticle_Right.SetActive(true);
                         SetPlantingReticle();
                    } 
               }
               
               // Transitions
               if (Input.GetButtonUp("Plant"))
               {
                    if (character == Character.Left)
                    {
                         // See if we planted something
                         if (seed != null && CheckForAllowedPlanting(PlantingReticle_Left.transform.position))
                         {
                              PlantSeed();
                         }
                    }

                    // Hide planting reticle
                    if (character == Character.Left)
                    {
                         PlantingReticle_Left.SetActive(false);
                         PlantingReticle_Right.SetActive(false);
                    }

                    // Transition
                    state = State.Moving;
               }
          }
     }


     public void PlaySound(string sound)
     {
          if (sound == "create")
          {
               _audioSource.clip = creationSound;
               _audioSource.Play();
          }

          if (sound == "destroy")
          {
               _audioSource.clip = destroySound;
               _audioSource.Play();
          }
     }


     void SimpleAnimate()
     {
          Vector3 direction = GetDirection();

          // Left
          if (direction.x < 0)
          {
               //Debug.Log("Anim Left");
               _renderer.material.SetTexture("_MainTex", leftPose);
          }

          // Right
          if (direction.x > 0)
          {
               //Debug.Log("Anim Right");
               _renderer.material.SetTexture("_MainTex", rightPose);
          }

          // Up
          if (direction.z > 0)
          {
               //Debug.Log("Anim Forward");
               _renderer.material.SetTexture("_MainTex", forwardPose);
          }

          // Down
          if (direction.z < 0)
          {
               //Debug.Log("Anim Back");
               _renderer.material.SetTexture("_MainTex", backPose);
          }
     }


     void PlantSeed()
     {
          // RIGHT SIDE //
          // Get the difference of position from the Left Char and Left Arch
          Vector3 distanceFromLeftArch = PlantingReticle_Left.transform.position - leftArchPoint.transform.position;

          // Apply that difference to the Right Arch position
          Vector3 positionFromRightArch = distanceFromLeftArch + rightArchPoint.transform.position;
          Vector3 seedPosition = FindPlantingHeight(positionFromRightArch);

          // if invalid position, don't plant
          if (seedPosition == new Vector3(-11, -11, -11))
               return;

          // Create a seed at the new position
          Transform seedParent = _levelManager.currentLoadedLevel.transform;
          GameObject newPlant = Instantiate(seed, seedPosition, seed.transform.rotation, seedParent);

          // LEFT SIDE //
          // Create the sprout
          GameObject newSprout = Instantiate(sprout, PlantingReticle_Left.transform.position, seed.transform.rotation, seedParent);

          // Link the sprout to the bush
          newSprout.GetComponent<Sprout>().linkedPlant = newPlant;

          // Hide the ui inventory element
          inventory.gameObject.SetActive(false);

          // Play sound
          PlaySound("create");

          // Remove the seed from inventory -- it was used.
          seed = null;
     }


     Vector3 FindPlantingHeight(Vector3 plantPosition)
     {
          RaycastHit rayHit;
          Vector3 startPosition = plantPosition + new Vector3(0, plantingHeightCheck, 0);

          // Find the first ground from the top down.
          bool cancelRayDown = Physics.Raycast(startPosition, Vector3.down, out rayHit, plantingHeightCheck * 2, cancelMask);
          if (cancelRayDown)
               return rayHit.point;

          // Find the first ground from the top down.
          bool rayDown = Physics.Raycast(startPosition, Vector3.down, out rayHit, plantingHeightCheck*2, surfaceDetectionMask);       
          if (rayDown)
               return rayHit.point;

          // default
          return Vector3.zero;
     }


     bool CheckForAllowedPlanting(Vector3 plantPosition)
     {
          RaycastHit rayHit;
          Vector3 startPosition = plantPosition + new Vector3(0, plantingHeightCheck, 0);

          // Find the first ground from the top down.
          bool cancelRayDown = Physics.Raycast(startPosition, Vector3.down, out rayHit, plantingHeightCheck * 2, cancelMask);
          if (cancelRayDown)
               return false;

          // Check for Cancel Range
          if (Mathf.Abs(PlantingReticle_Left.transform.localPosition.x) > reticleCancelWeight)
               return true;

          else if (Mathf.Abs(PlantingReticle_Left.transform.localPosition.z) > reticleCancelWeight)
               return true;

          else
               return false;
     }


     void SetPlantingReticle()
     {
          // Left Reticle
          Vector3 leftReticlePosition = transform.position + (GetDirection() * reticleOffset);
          leftReticlePosition = new Vector3(leftReticlePosition.x, 0f, leftReticlePosition.z);

          Vector3 groundHeight = FindPlantingHeight(leftReticlePosition) + new Vector3(0, -reticleHeight, 0);

          PlantingReticle_Left.transform.position = leftReticlePosition + new Vector3(0, groundHeight.y, 0);

          // Right Reticle
          // Get the difference of position from the Left Char and Left Arch
          Vector3 distanceFromLeftArch = PlantingReticle_Left.transform.position - leftArchPoint.transform.position;

          // Apply that difference to the Right Arch position
          Vector3 positionFromRightArch = distanceFromLeftArch + rightArchPoint.transform.position;

          PlantingReticle_Right.transform.position = FindPlantingHeight(positionFromRightArch) + new Vector3(0, -reticleHeight, 0);
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
