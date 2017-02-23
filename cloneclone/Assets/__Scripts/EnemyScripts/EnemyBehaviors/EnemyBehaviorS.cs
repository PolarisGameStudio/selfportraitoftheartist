﻿using UnityEngine;
using System.Collections;

public class EnemyBehaviorS : MonoBehaviour {

	private EnemyS myEnemy;

	public EnemyS myEnemyReference { get {return myEnemy;}}

	private EnemyBehaviorStateS myBehaviorState;

	private float behaviorActTime = 0f;

	private bool _behaviorActing = false;

	[Header("Behavior Properties")]
	public string behaviorName; // mostly for editor legibility
	public string animationKey = "";
	public GameObject soundObj;

	[Header("Status Properties")]
	public bool allowStun = false;
	public bool facePlayer = false;
	public bool dontAllowStateChange = false;
	
	[Header ("Break Properties")]
	public float breakAmt = 9999f;
	public float breakRecoverTime = 1f;
	public float allowParryStartTime = -1f;
	public float allowParryEndTime = -1f;
	
	[Header ("Effect Properties")]
	public GameObject signalObj;


	public virtual void StartAction(bool setAnimTrigger = true){

		_behaviorActing = true;
		myEnemy.SetActing(true);
		myEnemy.SetBehavior(this);
		myEnemy.SetStunStatus(allowStun);
		myEnemy.SetBreakState(breakAmt, breakRecoverTime);
		myEnemy.SetFaceStatus(facePlayer);

		behaviorActTime = 0f;

		if (animationKey != "" && setAnimTrigger){
			myEnemy.myAnimator.SetTrigger(animationKey);
			
			if (soundObj){
				Instantiate(soundObj);
			}

			if (signalObj != null){
				Vector3 signalPos =  transform.position;
				signalPos.z = transform.position.z+1f;
				GameObject signal = Instantiate(signalObj, signalPos, Quaternion.identity)
					as GameObject;
				signal.transform.parent = myEnemyReference.transform;
			}

		}


	}

	public virtual void BehaviorUpdate(){
		if (_behaviorActing){
			behaviorActTime += Time.deltaTime;
			if (behaviorActTime >= allowParryStartTime && behaviorActTime <= allowParryEndTime && !myEnemy.isCritical){
				myEnemy.canBeParried = true;
			}else{
				myEnemy.canBeParried = false;
			}
		}
	}

	public virtual void EndAction(bool doNextAction = true){

		_behaviorActing = false;
		myEnemy.SetActing(false);

		myEnemy.canBeParried = false;

		if (doNextAction){

			myEnemy.CheckBehaviorStateSwitch(dontAllowStateChange);

		}

	}

	public virtual void SetEnemy(EnemyS newEnemy){
		myEnemy = newEnemy;
	}

	public virtual void SetState(EnemyBehaviorStateS myState){
		myBehaviorState = myState;
	}

	public virtual bool BehaviorActing(){
		return (_behaviorActing && !myEnemy.isCritical && !myEnemy.hitStunned);
	}
}
