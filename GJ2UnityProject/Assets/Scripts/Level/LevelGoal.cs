using UnityEngine;

public class LevelGoal : MonoBehaviour
{
     public enum LevelSide
     {
          Left,
          Right
     }
     public LevelSide levelSide;

     LevelManager levelManager;


     private void Awake()
     {
          levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>(); ;
     }


     private void OnTriggerEnter(Collider other)
     {
          if (other.tag == "Player")
          {
               if (levelSide == LevelSide.Left)
                    levelManager.leftGoalPointActivated = true;

               if (levelSide == LevelSide.Right)
                    levelManager.rightGoalPointActivated = true;

               CheckWinCondition();
          }
     }


     private void OnTriggerExit(Collider other)
     {
          if (other.tag == "Player")
          {
               if (levelSide == LevelSide.Left)
                    levelManager.leftGoalPointActivated = false;

               if (levelSide == LevelSide.Right)
                    levelManager.rightGoalPointActivated = false;
          }
     }


     void CheckWinCondition()
     {
          // If both the level manager's goals are activated, then set the win condition.
          if ( levelManager.leftGoalPointActivated == true &&
               levelManager.rightGoalPointActivated == true)
          {
               levelManager.StartLevelTransition();
          }
     }
}
