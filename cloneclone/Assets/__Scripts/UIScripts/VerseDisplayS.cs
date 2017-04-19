﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VerseDisplayS : MonoBehaviour {

	public Image verseBorder;
	public Image borderBG;
	public Image verseIcon;
	public Image iconBG;
	public Text verseTitle;
	public Text verseTitleBg;
	private string currentVerse;

	private float savedFade = 0f;

	public float fadeRate = 0.5f;
	private Color currentCol;

	private bool fadingIn = false;
	private bool fadingOut = false;

	public static VerseDisplayS V;
	private bool _isShowing = true;

	void Awake(){

		if (V != null){
			Destroy(gameObject);
		}else{
			V = this;
		}

	}

	// Use this for initialization
	void Start () {

		currentCol = verseBorder.color;
		currentCol.a = 0f;
		verseBorder.color = verseIcon.color = currentCol;

		currentCol = verseTitle.color;
		currentCol.a = 0f;
		verseTitle.color = currentCol;
		currentCol = borderBG.color;
		currentCol.a = 0f;
		borderBG.color = currentCol;
		currentCol = verseTitleBg.color;
		currentCol.a = 0f;
		verseTitleBg.color = currentCol;
		currentCol = iconBG.color;
		currentCol.a = 0f;
		iconBG.color = currentCol;

		verseIcon.enabled = iconBG.enabled = false;
		verseBorder.enabled = borderBG.enabled = false;
		verseTitle.text = verseTitleBg.text = "";

		if (!PlayerController.equippedUpgrades.Contains(3)){
			_isShowing  = false;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (_isShowing){
		if (verseBorder.enabled){
			if (fadingOut){
				currentCol = verseBorder.color;
				currentCol.a -= Time.deltaTime*fadeRate;
				if (currentCol.a <= 0){
					fadingOut = false;
					currentCol.a = 0;
						verseIcon.enabled = iconBG.enabled = false;
						verseBorder.enabled = borderBG.enabled = false;
						verseTitle.text = verseTitleBg.text = "";
				}
					verseBorder.color =  verseIcon.color = currentCol;
					savedFade = currentCol.a;

					currentCol = verseTitle.color;
					currentCol.a = savedFade;
					verseTitle.color = currentCol;
					currentCol = borderBG.color;
					currentCol.a = savedFade;
					borderBG.color = currentCol;
					currentCol = verseTitleBg.color;
					currentCol.a = savedFade;
					verseTitleBg.color = currentCol;
					currentCol = iconBG.color;
					currentCol.a = savedFade;
					iconBG.color = currentCol;
			}
			if (fadingIn){
				currentCol = verseBorder.color;
				currentCol.a += Time.deltaTime*fadeRate;
				if (currentCol.a >= 1f){
					fadingIn = false;
					currentCol.a = 1f;
				}
					verseBorder.color =  verseIcon.color = currentCol;
					savedFade = currentCol.a;

					currentCol = verseTitle.color;
					currentCol.a = savedFade;
					verseTitle.color = currentCol;
					currentCol = borderBG.color;
					currentCol.a = savedFade;
					borderBG.color = currentCol;
					currentCol = verseTitleBg.color;
					currentCol.a = savedFade;
					verseTitleBg.color = currentCol;
					currentCol = iconBG.color;
					currentCol.a = savedFade;
					iconBG.color = currentCol;
			}
		}
		}
	
	}

	public void EndVerse(){
		StartCoroutine(EndVerseCoroutine());
	}
	private IEnumerator EndVerseCoroutine(){

		currentVerse = "";
		yield return new WaitForSeconds(1.6f);
		if (!fadingIn){
			fadingIn = false;
			fadingOut = true;
		}
	}

	public void NewVerse(string verseString){
		verseTitle.text = verseTitleBg.text = currentVerse = verseString;
		fadingIn = true;
		fadingOut = false;
		currentCol = verseBorder.color;
		currentCol.a = 0f;
		verseBorder.color = verseIcon.color = currentCol;

		currentCol = verseTitle.color;
		currentCol.a = 0f;
		verseTitle.color = currentCol;
		currentCol = borderBG.color;
		currentCol.a = 0f;
		borderBG.color = currentCol;
		currentCol = verseTitleBg.color;
		currentCol.a = 0f;
		verseTitleBg.color = currentCol;
		currentCol = iconBG.color;
		currentCol.a = 0f;
		iconBG.color = currentCol;

		if (_isShowing){
			verseIcon.enabled = iconBG.enabled = true;
			verseBorder.enabled = borderBG.enabled = true;
		}else{
			verseTitle.text = "";
		}
	}

	public void Show(){
		_isShowing = true;
		if (currentVerse != ""){
			verseIcon.enabled = iconBG.enabled = true;
			verseBorder.enabled = borderBG.enabled = true;
			verseTitle.text = currentVerse;
		}
	}
	public void Hide(){
		_isShowing = false;
		if (currentVerse != ""){
			verseIcon.enabled = iconBG.enabled = false;
			verseBorder.enabled = borderBG.enabled = false;
			verseTitle.text = "";
		}
	}
}
