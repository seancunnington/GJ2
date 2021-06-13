﻿using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Level
{
     public GameObject levelPrefab;
     public AudioClip backgroundMusic;
}

public class LevelManager : MonoBehaviour
{
     public bool titleScreenLevel = true;

     public GameObject leftCharacter;
     public GameObject rightCharacter;

     public bool leftGoalPointActivated = false;
     public bool rightGoalPointActivated = false;

     public GameObject leftStartingPoint;
     public GameObject rightStartingPoint;

     public Level[] levelList;
     [SerializeField] int currentLevelIndex = -1;
     public GameObject currentLoadedLevel;

     public AudioSource _audio_1;
     public AudioSource _audio_2;
     public bool fadeOut_1 = false;
     public float fadeSpeed = 1f;

     public GameObject uiCanvas;
     public Image uiImage;

     public Animation screenTransition;
     public Animation winCard;
     bool lookForLoad = false;

     private void Awake()
     {
          winCard.transform.parent.gameObject.SetActive(false);
          titleScreenLevel = true;
          uiCanvas.SetActive(false);
          currentLevelIndex = -1;
          LoadLevelDetails();
     }


     private void Update()
     {
          AudioTransition();

          // If on the title screen, be able to progress
          if (titleScreenLevel)
          {
               if (Input.GetButtonDown("Plant"))
               {
                    titleScreenLevel = false;
                    uiCanvas.SetActive(true);
                    PlayTransition();
               }
          }

          if (lookForLoad)
          {
               Debug.Log("looking for load");

               // Once animation stops playing, load level
               if (screenTransition.isPlaying == false)
               {
                    LoadLevelDetails();
                    lookForLoad = false;
                    screenTransition.Play("anim_DotShrink");
                    Debug.Log("should play shrink anim");
               }           
          }
     }

     public void LoadLevelDetails(bool restart = false)
     {
          // Clear goal points
          leftGoalPointActivated = false;
          rightGoalPointActivated = false;

          // Deactivate characters
          leftCharacter.SetActive(false);
          rightCharacter.SetActive(false);

          if (!restart)
               currentLevelIndex++;

          // Only if NOT FIRST LEVEL
          if (currentLevelIndex != 0)
          {
               // Destroy current level
               Destroy(currentLoadedLevel);

               // Create next level
               currentLoadedLevel = Instantiate(levelList[currentLevelIndex].levelPrefab);
          }

          // Play this level's audio
          StartAudioTransition();

          if (currentLevelIndex == (levelList.Length - 1))
          {
               Debug.Log("This is a win condition.");
               PlayWinCard();
               _audio_1.loop = false;
               _audio_2.loop = false;
               return;
          }

          if (!titleScreenLevel)
          {
               // Set new character positions
               FindStartingPoints();
               leftCharacter.transform.position = leftStartingPoint.transform.position + new Vector3(0, 2, 0);
               rightCharacter.transform.position = rightStartingPoint.transform.position + new Vector3(0, 2, 0);

               // Set left character's arch positions
               leftCharacter.GetComponent<CharacterFSM>().leftArchPoint = leftStartingPoint;
               leftCharacter.GetComponent<CharacterFSM>().rightArchPoint = rightStartingPoint;

               // Get rid of all seeds and sprouts
               leftCharacter.GetComponent<CharacterFSM>().seed = null;
               leftCharacter.GetComponent<CharacterFSM>().sprout = null;
               uiImage.gameObject.SetActive(false);

               // Reactivate characters
               leftCharacter.SetActive(true);
               rightCharacter.SetActive(true);
          }
     }

     void PlayWinCard()
     {
          uiCanvas.SetActive(false);
          winCard.transform.parent.gameObject.SetActive(true);
          winCard.Play();
     }


     void PlayTransition()
     {
          screenTransition.transform.parent.gameObject.SetActive(true);
          screenTransition.Play("anim_DotGrow");
          lookForLoad = true;
     }


     void StartAudioTransition()
     {
          // if fadeOut_1 is set FALSE, then set _audio_1 clip.
          if (fadeOut_1 == true)
          {
               // assign new clip
               _audio_1.clip = levelList[currentLevelIndex].backgroundMusic;

               // if clip is same as old, remove and and return.
               if (_audio_1.clip == _audio_2.clip)
               {
                    _audio_1.clip = null;
                    return;
               }

               _audio_1.Play();
               fadeOut_1 = false;
               return;
          }

          // if fadeOut_1 is set TRUE, then set _audio_2 clip.
          if (fadeOut_1 == false)
          {               
               // assign new clip
               _audio_2.clip = levelList[currentLevelIndex].backgroundMusic;

               // if clip is same as old, remove and and return.
               if (_audio_1.clip == _audio_2.clip)
               {
                    _audio_2.clip = null;
                    return;
               }
               _audio_2.Play();
               fadeOut_1 = true;
               return;
          }
     }


     void AudioTransition()
     {
          // if fadeOut_1 is currently FALSE, then increase _audio_1 volume.
          if (fadeOut_1 == false)
          {
               if (_audio_1.volume < 0.3f)
                    _audio_1.volume += Time.deltaTime * fadeSpeed;

               if (_audio_2.volume > 0f)
                    _audio_2.volume -= Time.deltaTime * fadeSpeed;
          }

          // if fadeOut_1 is currently TRUE, then decrease _audio_1 volume.
          if (fadeOut_1 == true)
          {
               if (_audio_1.volume > 0f)
                    _audio_1.volume -= Time.deltaTime * fadeSpeed;

               if (_audio_2.volume < 0.3f)
                    _audio_2.volume += Time.deltaTime * fadeSpeed;
          }

          // If either audio volume is 0, set null.
          if (_audio_1.volume <= 0)
               _audio_1.clip = null;

          if (_audio_2.volume <= 0)
               _audio_2.clip = null;
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
          PlayTransition();
     }
}
