using UnityEngine;

public class Seed : MonoBehaviour
{
     public GameObject plantPrefab;
     public GameObject sproutPrefab;
     public Sprite seedSprite;

     private void OnTriggerEnter(Collider other)
     {
          if (other.tag == "Player")
          {
               // Give the player the seed and sprout
               other.GetComponent<CharacterFSM>().seed = plantPrefab;
               other.GetComponent<CharacterFSM>().sprout = sproutPrefab;

               // Give the ui image
               other.GetComponent<CharacterFSM>().inventory.sprite = seedSprite;
               other.GetComponent<CharacterFSM>().inventory.gameObject.SetActive(true);

               // Destroy this instance
               Destroy(this.gameObject);
          }
     }
}
