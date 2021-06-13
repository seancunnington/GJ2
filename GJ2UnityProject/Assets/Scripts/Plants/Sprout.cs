using UnityEngine;

public class Sprout : MonoBehaviour
{
     public GameObject linkedPlant;
     public GameObject originalSeed;
     public GameObject reticle;
     public Sprite seedSprite;

     private void Awake()
     {
          reticle.SetActive(false);
     }

     private void OnTriggerStay(Collider other)
     {
          if (other.tag == "Player")
          {
               if (Input.GetButtonDown("Plant"))
               {
                    // If player already has seed, do nothing.
                    if (other.GetComponent<CharacterFSM>().seed != null)
                         return;

                    // Give the ui image
                    other.GetComponent<CharacterFSM>().inventory.sprite = seedSprite;
                    other.GetComponent<CharacterFSM>().inventory.gameObject.SetActive(true);

                    // Play destroy sound
                    other.GetComponent<CharacterFSM>().PlaySound("destroy");

                    // Destroy plants
                    other.GetComponent<CharacterFSM>().seed = originalSeed;
                    Destroy(linkedPlant.gameObject);
                    Destroy(this.gameObject);
               }
          }
     }

     private void OnTriggerEnter(Collider other)
     {
          if (other.tag == "Player")
          {
               // If player already has seed, do nothing.
               if (other.GetComponent<CharacterFSM>().seed != null)
                    return;

               reticle.SetActive(true);
          }
     }

     private void OnTriggerExit(Collider other)
     {
          if (other.tag == "Player")
          {
               reticle.SetActive(false);
          }
     }
}
