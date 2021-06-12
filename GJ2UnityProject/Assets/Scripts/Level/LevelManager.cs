using UnityEngine;

[System.Serializable]
public class Level
{
     public GameObject levelPrefab;
     public AudioClip backgroundMusic;
}


public class LevelManager : MonoBehaviour
{
     public GameObject leftCharacter;
     public GameObject rightCharacter;

     public bool leftGoalPointActivated = false;
     public bool rightGoalPointActivated = false;

     public GameObject leftStartingPoint;
     public GameObject rightStartingPoint;

     public Level[] levelList;
     [SerializeField] int currentLevelIndex = -1;
     public GameObject currentLoadedLevel;


     private void Awake()
     {
          currentLevelIndex = -1;
          LoadLevel();
     }


     public void LoadLevel()
     {
          // Clear goal points
          leftGoalPointActivated = false;
          rightGoalPointActivated = false;

          // Deactivate characters
          leftCharacter.SetActive(false);
          rightCharacter.SetActive(false);

          currentLevelIndex++;

          if ( currentLevelIndex == (levelList.Length))
          {
               Debug.Log("This is a win condition.");
               return;
          }

          // Only if NOT FIRST LEVEL
          if (currentLevelIndex != 0)
          {
               // Destroy current level
               Destroy(currentLoadedLevel);

               // Create next level
               currentLoadedLevel = Instantiate(levelList[currentLevelIndex].levelPrefab);
          }

          // Set new character positions
          FindStartingPoints();
          leftCharacter.transform.position = leftStartingPoint.transform.position + new Vector3(0, 2, 0);
          rightCharacter.transform.position = rightStartingPoint.transform.position + new Vector3(0, 2, 0);

          // Set left character's arch positions
          leftCharacter.GetComponent<CharacterFSM>().leftArchPoint = leftStartingPoint;
          leftCharacter.GetComponent<CharacterFSM>().rightArchPoint = rightStartingPoint;

          // Reactivate characters
          leftCharacter.SetActive(true);
          rightCharacter.SetActive(true);
     }


     void FindStartingPoints()
     {
          GameObject leftStart = null;
          GameObject rightStart = null;

          for (int i = 0; i < currentLoadedLevel.transform.childCount; i++)
          {
               if (currentLoadedLevel.transform.GetChild(i).tag == "LeftStart")
                    leftStart = currentLoadedLevel.transform.GetChild(i).gameObject;

               if (currentLoadedLevel.transform.GetChild(i).tag == "RightStart")
                    rightStart = currentLoadedLevel.transform.GetChild(i).gameObject;
          }

          leftStartingPoint = leftStart;
          rightStartingPoint = rightStart;
     }


     public void StartLevelTransition()
     {
          Debug.Log("Starting Level Transition");

          LoadLevel();
     }
}
