﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InstructionTextS : MonoBehaviour {

	public float fadeRate = 1f;
	private Text myText;
	private Color textColor;

	private bool showing = false;

	// Use this for initialization
	void Start () {

		myText = GetComponent<Text>();
		textColor = myText.color;
		textColor.a = 0;
		myText.color = textColor;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (showing){
			if (myText.color.a < 1){
				textColor = myText.color;
				textColor.a += Time.deltaTime*fadeRate;
				myText.color = textColor;
			}
		}else{
			if (myText.color.a > 0){
				textColor = myText.color;
				textColor.a -= Time.deltaTime*fadeRate/2f;
				myText.color = textColor;
			}
		}
	
	}

	public void SetShowing(bool newShow, string newText = ""){
		showing = newShow; 

		if (newText != ""){
			myText.text = newText;
		}
	}
}
