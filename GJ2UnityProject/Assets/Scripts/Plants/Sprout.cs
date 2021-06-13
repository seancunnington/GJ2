using UnityEngine;

public class Sprout : MonoBehaviour
{
     public GameObject linkedPlant;
     public GameObject originalSeed;
     public GameObject reticle;

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
