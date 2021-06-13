using UnityEngine;

public class SpringBush : MonoBehaviour
{
     [Range(0f,200f)] public float springStrength;

     private void OnTriggerEnter(Collider other)
     {
          if (other.tag == "Player")
          {
               other.GetComponent<CharacterPhysics>().allowLevitate = false;
               other.GetComponent<Rigidbody>().AddForce(Vector3.up * springStrength, ForceMode.Impulse);

               other.GetComponent<CharacterPhysics>().DisableLevitate();
          }
     }


     private void OnTriggerExit(Collider other)
     {
          if (other.tag == "Player")
          {
               
          }
     }
}
