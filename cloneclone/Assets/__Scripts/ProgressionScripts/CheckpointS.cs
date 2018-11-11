﻿using UnityEngine;
using System.Collections;

public class CheckpointS : MonoBehaviour {

    public static string lastSavePointName = "Abandoned Faith";
    public static int totalPlayTimeSeconds;
    public static int totalPlayTimeMinutes;
    public static int totalPlayTimeHours;

    public static float lastSavedTimeDotTime = 0f;

	public bool fullCheckpoint = true;
    public bool dontAddToSceneList = false;
    public string savePointName = "Abandoned Faith";
    public string overrideSaveScene = "";
	private PlayerDetectS _playerDetect;
	private bool _examining = false;
	public Vector3 examinePos = new Vector3(0, 1f, 0);

	private InGameMenuManagerS _menuManager;

	private bool _exitButtonUp = false;
	private bool _talkButtonUp = false;

	private InstructionTextS instructionText;

	public int[] addToCompletedFights;

	private string infiniteMessage = "Health Restored.";

	private string infiniteMessageWithHeal = "Health Restored.\nHEALTH ESSENCES and REWINDs restored.";
	private string healMessageWithHeal =  "Health restored. Progress saved.\nHEALTH ESSENCE and REWINDs restored.";

	private string infiniteMessageWithItem = "Health Restored.\nREWINDs restored.";
	private string healMessage = "Health restored. Progress saved.";
	private string healMessageWithItem =  "Health restored. Progress saved.\nREWINDs restored.";
	public int spawnNum = 0;

	public GameObject checkpointSound;

	
	public BGMLayerS[] musicAtPoint;
    public int addToProgress = -1;
    public bool preMariaCheckpoint;
    public bool midDescentCheckpoint;

	void Awake(){
		if (fullCheckpoint){
			PlayerController.familiarUnlocked = true;
		}
	}

	// Use this for initialization
	void Start () {

		_playerDetect = GetComponentInChildren<PlayerDetectS>();

		DeathCountdownS.DC.TurnOffCountdown(true);

		if (!fullCheckpoint){
			instructionText = GameObject.Find("InstructionText").GetComponent<InstructionTextS>();
		}else{	
			_menuManager = GameObject.Find("Menus").GetComponent<InGameMenuManagerS>();
			if (!dontAddToSceneList){
			PlayerInventoryS.I.AddCheckpoint(Application.loadedLevel, spawnNum);
			}
		}
        if (preMariaCheckpoint || midDescentCheckpoint){
            _menuManager.levelMenu.LockTravel();
        }
		if (addToCompletedFights.Length > 0){
			for (int i = 0; i<addToCompletedFights.Length;i++){
				PlayerInventoryS.I.dManager.AddClearedCombat(addToCompletedFights[i], -1, "C");
			}
		}

        lastSavePointName = savePointName;
        AddToSaveTime();

		CombatGiverS.chosenSpecialCombat = -1;

		if (!SceneManagerS.inInfiniteScene){
            if (overrideSaveScene != "")
            {
                GameOverS.reviveScene = overrideSaveScene;
                GameOverS.revivePosition = 0;
            }
            else
            {
                GameOverS.reviveScene = Application.loadedLevelName;
                GameOverS.revivePosition = spawnNum;
            }
            PlayerStatsS.PlayerCantDie = false;
            if (addToProgress > -1)
            {
                StoryProgressionS.SetStory(addToProgress);
            }
			StoryProgressionS.SaveProgress();
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_playerDetect.PlayerInRange() && !_examining){

			
			if (_playerDetect.player.myControl.ControllerAttached() || _playerDetect.examineStringNoController == ""){
				_playerDetect.player.SetExamining(true, examinePos, _playerDetect.examineString);
			}else{
				_playerDetect.player.SetExamining(true, examinePos, _playerDetect.examineStringNoController);
			}

			if (_playerDetect.player.myControl.GetCustomInput(3) && _talkButtonUp
			    && !_playerDetect.player.talking && !CameraEffectsS.E.isFading){
				
                AddToSaveTime();

				if (fullCheckpoint){
					_examining = true;
						_menuManager.TurnOnLevelUpMenu();
					_playerDetect.player.SetTalking(true);
					_playerDetect.player.SetExamining(true, examinePos, "");
					if (checkpointSound){
						Instantiate(checkpointSound);
					}

				}
				else{
					_playerDetect.player.TriggerResting(3f);
					if (checkpointSound){
						Instantiate(checkpointSound);
					}
					if (!PlayerStatDisplayS.RECORD_MODE){
						if (PlayerInventoryS.I.CheckForItem(1)){
							if (SceneManagerS.inInfiniteScene){
								instructionText.SetTimedMessage(infiniteMessageWithHeal, 1.4f);
							}else{
								instructionText.SetTimedMessage(healMessageWithHeal, 1.4f);
							}
						}

					else if (PlayerInventoryS.I.CheckForItem(0)){
						if (SceneManagerS.inInfiniteScene){
							instructionText.SetTimedMessage(infiniteMessageWithItem, 1.4f);
						}else{
						instructionText.SetTimedMessage(healMessageWithItem, 1.4f);
						}
					}else{
						if (SceneManagerS.inInfiniteScene){
							instructionText.SetTimedMessage(infiniteMessage, 1.4f);
						}else{
							instructionText.SetTimedMessage(healMessage, 1.4f);
						}
					}
					}
					_playerDetect.player.SetExamining(true, examinePos, "");
					//Debug.Log("YEAH");
			}
				// set revive pos
				if (!SceneManagerS.inInfiniteScene){
                    if (overrideSaveScene != "")
                    {
                        GameOverS.reviveScene = overrideSaveScene;
                        GameOverS.revivePosition = 0;
                    }
                    else
                    {
                        GameOverS.reviveScene = Application.loadedLevelName;
                        GameOverS.revivePosition = spawnNum;
                    }
					StoryProgressionS.SaveProgress();
				}
			// heal player
			_playerDetect.player.myStats.FullRecover();

				PlayerInventoryS.I.dManager.ClearAll();
				if (addToCompletedFights.Length > 0){
					for (int i = 0; i<addToCompletedFights.Length;i++){
						PlayerInventoryS.I.dManager.AddClearedCombat(addToCompletedFights[i], -1, "C");
					}
				}
				PlayerInventoryS.I.RefreshRechargeables();
			_talkButtonUp = false;
			}
		}

		if (_examining){
			if ((_playerDetect.player.myControl.GetCustomInput(1) && _exitButtonUp && _menuManager.levelMenu.canBeExited)
			    || _menuManager.levelMenu.sendExitMessage){
				// exit menu
				_examining = false;
				_menuManager.TurnOffLevelUpMenu();
				_playerDetect.player.SetTalking(false);
				_exitButtonUp = false;
				_menuManager.levelMenu.sendExitMessage = false;
			}
		}

		if (_playerDetect.player != null){
            if (!_playerDetect.player.myControl.GetCustomInput(1)){
			_exitButtonUp = true;
		}else{
			_exitButtonUp = false;
		}
		
		if (!_playerDetect.player.myControl.GetCustomInput(3)){
			_talkButtonUp = true;
		}else{
			_talkButtonUp = false;
		}
		}else{
			_talkButtonUp = false;
			_exitButtonUp = false;
		}

	
	}

	public void ActivateMusic(){

		if (musicAtPoint.Length > 0){

			for (int i = 0; i < musicAtPoint.Length; i++){
				if (BGMHolderS.BG.ContainsChild(musicAtPoint[i].sourceRef.clip)){
					BGMHolderS.BG.GetLayerWithClip(musicAtPoint[i].sourceRef.clip).FadeIn();
				}else{
					musicAtPoint[i].transform.parent = BGMHolderS.BG.transform;
					if (musicAtPoint[i].matchTimeStamp && musicAtPoint[i].sourceRef.clip.samples >= BGMHolderS.BG.GetCurrentTimeSample()){
						musicAtPoint[i].sourceRef.timeSamples = BGMHolderS.BG.GetCurrentTimeSample();
					}
					musicAtPoint[i].FadeIn();
				}
			}

			BGMHolderS.BG.EndAllExcept(musicAtPoint, false, true);
		}else{
			// clear all currently playing
			BGMHolderS.BG.EndAllLayers(false, true);
		}

	}

    void AddToSaveTime(){
        
            float currentTime = Time.time;
            int additionalTime = Mathf.RoundToInt(currentTime - lastSavedTimeDotTime);
            lastSavedTimeDotTime = currentTime;
        if (totalPlayTimeHours < 9999)
        {

            while (additionalTime >= 3600)
            {
                additionalTime -= 3600;
                totalPlayTimeHours++;
            }
            while (additionalTime >= 60)
            {
                additionalTime -= 60;
                totalPlayTimeMinutes++;
                if (totalPlayTimeMinutes > 59)
                {
                    totalPlayTimeMinutes = 0;
                    totalPlayTimeHours++;
                }
            }
            totalPlayTimeSeconds += additionalTime;
            if (totalPlayTimeSeconds >= 60)
            {
                totalPlayTimeSeconds -= 60;
                totalPlayTimeMinutes++;
                if (totalPlayTimeMinutes >= 60)
                {
                    totalPlayTimeMinutes -= 60;
                    totalPlayTimeHours++;
                }
            }

            // max time check
            if (totalPlayTimeHours > 9999)
            {
                totalPlayTimeHours = 9999;
                totalPlayTimeMinutes = 59;
                totalPlayTimeSeconds = 59;
            }
        }
    }
}
