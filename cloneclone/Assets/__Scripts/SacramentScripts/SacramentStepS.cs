﻿using UnityEngine;
using System.Collections;

public class SacramentStepS : MonoBehaviour {

	private SacramentHandlerS _myHandler;
	public SacramentHandlerS myHandler { get { return _myHandler; } }
	private bool _initialized = false;

	private bool _stepActive = false;
	public bool stepActive { get { return _stepActive; } }

	private bool waitOnOption = false;
	public bool waitOnOptions { get { return waitOnOption; } }

	[Header("Item Properties")]
	public SacramentTextS[] sacramentTexts;
	private int currentText = 0;
	public SacramentOptionS[] sacramentOptions;
	public SacramentImageS[] sacramentImages;
	private bool delayOptionSetup = false;

	[Header("Navigation Properties")]
	public SacramentStepS nextStep;

	[Header("Sound Properties")]
	public GameObject onSound;
	public GameObject offSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void AdvanceText(){
		sacramentTexts[currentText].DeactivateText();
		currentText++;
		if (currentText < sacramentTexts.Length){
			sacramentTexts[currentText].ActivateText(this);
		}else if (!waitOnOption){
			_myHandler.AdvanceStep();
		}else if (delayOptionSetup){
			delayOptionSetup = false;
			DelayedOptionSetup();
		}

	}

	public void ActivateStep(){
		_stepActive = true;
	delayOptionSetup = false;
		_myHandler.DeactivateWait();
		SetUpOptions();
		SetUpTexts();
		gameObject.SetActive(true);
		if (onSound){
			Instantiate(onSound);
		}
	}

	public void DeactivateStep(){
		_stepActive = false;
		gameObject.SetActive(false);
		if (offSound){
			Instantiate(offSound);
		}
	}

	public void Initialize(SacramentHandlerS handler){
		if (!_initialized){
			_myHandler = handler;
			_initialized = true;
		}
	}

	void SetUpTexts(){
		currentText = 0;
		if (sacramentTexts.Length > 0){
			for (int i = 0; i < sacramentTexts.Length; i++){
				if (i == 0){
			sacramentTexts[i].ActivateText(this);
				}else{
					sacramentTexts[i].gameObject.SetActive(false);
				}
			}
		}
	}

	void SetUpOptions(){
		if (sacramentTexts.Length > 0){
			if (sacramentOptions.Length > 0){
				for (int i = 0; i < sacramentOptions.Length; i++){
					sacramentOptions[i].Hide();
				}
				delayOptionSetup = true;
				waitOnOption = true;
			}
		}else{
		if (sacramentOptions.Length > 0){
			for (int i = 0; i < sacramentOptions.Length; i++){
				sacramentOptions[i].Initialize(_myHandler);
			}
			waitOnOption = true;
		}
		}
	}

	void DelayedOptionSetup(){
		for (int i = 0; i < sacramentOptions.Length; i++){
			sacramentOptions[i].Initialize(_myHandler);
		}
	}

	void SetUpImages(){
		
	}
}
