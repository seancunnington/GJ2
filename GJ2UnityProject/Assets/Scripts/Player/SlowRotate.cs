using UnityEngine;

public class SlowRotate : MonoBehaviour
{
     [Range(0f, 100f)] public float rotateSpeed;

    
     void Update()
     {
          transform.RotateAround(transform.position, Vector3.up, rotateSpeed * Time.deltaTime);
     }
}
