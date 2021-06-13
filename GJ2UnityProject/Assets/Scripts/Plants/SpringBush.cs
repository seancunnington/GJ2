using UnityEngine;

public class SpringBush : MonoBehaviour
{
     [Range(0f,200f)] public float springStrength;
     bool allowForce = true;


     private void OnTriggerEnter(Collider other)
     {
          if (other.tag == "Player")
          {
               other.GetComponent<CharacterPhysics>().allowLevitate = false;

               if (allowForce)
               {
                    other.GetComponent<Rigidbody>().AddForce(Vector3.up * springStrength, ForceMode.Impulse);
                    allowForce = false;
               }

               other.GetComponent<CharacterPhysics>().DisableLevitate();
          }
     }


     private void OnTriggerExit(Collider other)
     {
          if (other.tag == "Player")
          {
               allowForce = true;
          }
     }
}
