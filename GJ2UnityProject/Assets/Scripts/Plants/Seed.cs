using UnityEngine;

public class Seed : MonoBehaviour
{
     public GameObject plantPrefab;
     public GameObject sproutPrefab;

     private void OnTriggerEnter(Collider other)
     {
          if (other.tag == "Player")
          {
               // Give the player the seed and sprout
               other.GetComponent<CharacterFSM>().seed = plantPrefab;
               other.GetComponent<CharacterFSM>().sprout = sproutPrefab;

               // Destroy this instance
               Destroy(this.gameObject);
          }
     }
}
