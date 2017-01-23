﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerLvDisplayS : MonoBehaviour {

	private PlayerStatsS playerStatRef;

	public Text hpStat;
	public Text stStat;
	public Text mnStat;
	public Text vtStat;
	public Text enStat;
	public Text abStat;

	private Color textStartColor;
	private Color highlightColor = Color.white;

	private bool _initialized = false;


	// Use this for initialization
	void Start () {

		Initialize();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void Initialize(bool doUpdate = true){
		if (!_initialized){
			_initialized = true;
			playerStatRef = GameObject.Find("Player").GetComponent<PlayerStatsS>();
			textStartColor = hpStat.color;
		}
		if (doUpdate){
			UpdateDisplays();
		}
	}

	public void UpdateDisplays(){

		if (!_initialized){
			Initialize(false);
		}

		hpStat.color = stStat.color = mnStat.color = vtStat.color = enStat.color = abStat.color = textStartColor;
		hpStat.text = (playerStatRef.maxHealth*10f).ToString();
		stStat.text = (playerStatRef.maxMana*10f).ToString();
		mnStat.text = (playerStatRef.maxCharge).ToString();
		vtStat.text = (playerStatRef.virtueAmt).ToString();
		//enStat.text = playerStatRef.currentRecoverRateLv.ToString();
		//abStat.text = playerStatRef.currentChargeRecoverLv.ToString();

	}

	public void HighlightStat(int statToEdit){
		UpdateDisplays();
		switch (statToEdit){
		default: 
			break;
		case (0):
			hpStat.text = (playerStatRef.maxHealth*10f+10f).ToString();
			hpStat.color = highlightColor;
			break;
		case (1):
			stStat.text = (playerStatRef.maxMana*10f+10f).ToString();
			stStat.color = highlightColor;
			break;
		case (2):
			mnStat.text = (playerStatRef.maxCharge+1f).ToString();
			mnStat.color = highlightColor;
			break;
		case (3):
			vtStat.text = (playerStatRef.virtueAmt+2f).ToString();
			vtStat.color = highlightColor;
			break;
		case (4):
			abStat.text = (playerStatRef.currentChargeRecoverLv+1).ToString();
			abStat.color = highlightColor;
			break;
		case (5):
			enStat.text = (playerStatRef.currentRecoverRateLv+1).ToString();
			enStat.color = highlightColor;
			break;
		
		}
	}
}
