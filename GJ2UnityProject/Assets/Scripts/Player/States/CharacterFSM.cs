using UnityEngine;

[RequireComponent(typeof(CharacterPhysics))]
public class CharacterFSM : MonoBehaviour
{
     #region Public Variables

     public string currentStateName = "";

     //states

     //input
     [HideInInspector] public CharacterPhysics _characterPhysics;

     #endregion

     #region Private Variables

     //current state

     #endregion



     private void Awake()
     {
          _characterPhysics = GetComponent<CharacterPhysics>();
     }


     private void OnEnable()
     {
          
     }


     private void Update()
     {
          
     }
}
