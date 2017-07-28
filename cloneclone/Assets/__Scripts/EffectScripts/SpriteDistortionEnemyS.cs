﻿using UnityEngine;
using System.Collections;

public class SpriteDistortionEnemyS : MonoBehaviour {

	private SpriteRenderer mySprite;
	private SpriteRenderer parentSprite;
	private Color critColor = Color.gray;
	private Color startColor;

	public float changeRate = 0.12f;
	private float changeCountdown = 0f;
	public float changeSizeAmt = 0.2f;
	public float changeSizeCritMult = 2f;
	private float changeRateCritMult = 3f;

	private float deadChangeRate;
	private bool inCritState = false;

	[Header("Special Cases")]
	public float shadowAlphaMult = 1f;

	private EnemyS enemyReference;
	private bool matchingFlash = false;

	// Use this for initialization
	void Start () {

		enemyReference = transform.GetComponentInParent<EnemyS>();

		mySprite = GetComponent<SpriteRenderer>();
		parentSprite = transform.parent.GetComponent<SpriteRenderer>();
		startColor = enemyReference.bloodColor;
		startColor.a *= shadowAlphaMult;
		mySprite.material.SetColor("_FlashColor", startColor);
		mySprite.sprite = parentSprite.sprite;
		ChangeSize();

		deadChangeRate = changeRate*2f;
	}
	
	// Update is called once per frame
	void Update () {

		if (enemyReference.isDead){
			changeRate = deadChangeRate;
			if (matchingFlash){
				matchingFlash = false;
				mySprite.material.SetColor("_FlashColor", startColor);
			}
		}

		if (mySprite.enabled){
			mySprite.sprite = parentSprite.sprite;
	
			if (enemyReference.isCritical && !enemyReference.isDead){
				changeCountdown -= Time.deltaTime*changeRateCritMult;
			}else{
				changeCountdown -= Time.deltaTime;
			}
			if (changeCountdown <= 0){
				ChangeSize();
			}

			if (!enemyReference.isDead && !enemyReference.isCritical){
				if (matchingFlash){
					if (enemyReference.flashReference <= 0){
						matchingFlash = false;
						mySprite.material.SetColor("_FlashColor", startColor);
					}
					
				}else{
					if (enemyReference.flashReference > 0){
						matchingFlash = true;
						mySprite.material.SetColor("_FlashColor", enemyReference.myRenderer.material.GetColor("_FlashColor"));
					}
				}
			}
		}
	
	}

	private void ChangeSize(){
		if (enemyReference.isCritical && !enemyReference.isDead){
			if (!inCritState){
				mySprite.material.SetColor("_FlashColor", critColor);
				inCritState = true;
			}
			transform.localScale = Vector3.one+Random.insideUnitSphere*changeSizeCritMult;
		}else{
			if (inCritState){
				inCritState = false;
				mySprite.material.SetColor("_FlashColor", startColor);
			}
			transform.localScale = Vector3.one+Random.insideUnitSphere*changeSizeAmt;
		}
		changeCountdown = changeRate;
	}
}
