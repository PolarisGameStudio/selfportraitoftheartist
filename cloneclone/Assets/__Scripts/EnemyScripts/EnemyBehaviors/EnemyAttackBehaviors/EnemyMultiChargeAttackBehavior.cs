﻿using UnityEngine;
using System.Collections;

public class EnemyMultiChargeAttackBehavior : EnemyBehaviorS {

	public PlayerDetectS rangeCheck;
	public EnemyDodgeBehaviorS dodgeCheck;
	private bool doDodge = false;

	[Header("Behavior Properties")]
	public float attackDuration = 3f;
	public float[] attackWarmups;
	public EnemyChargeAttackS[] attackColliders;
	public bool killOnCast = false;


	private Vector3 attackDirection;

	private float attackTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {
		
		BehaviorUpdate();

		if (BehaviorActing()){

			attackTimeCountdown -= Time.deltaTime;


			if (attackTimeCountdown <= 0){
				EndAction();
			}
		}
	
	}

	private void InitializeAction(){

		if (AttackInRange()){
			myEnemyReference.myRigidbody.velocity = Vector3.zero;
			attackTimeCountdown = attackDuration;
			for (int i = 0; i < attackColliders.Length; i++){
				attackColliders[i].TurnOn(attackWarmups[i]/currentDifficultyMult, killOnCast);
			}
			myEnemyReference.AttackFlashEffect();
			if (animationKey != ""){
				myEnemyReference.myAnimator.SetTrigger(animationKey);
				//Debug.Log("Behavior " + behaviorName + " is attempting to send trigger " + animationKey + " through base.StartAction()");

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
		else if (doDodge){

			EndAction(false);
			dodgeCheck.SetEnemy(myEnemyReference);
			dodgeCheck.StartAction();
		}
		else{
			EndAction();
		}

	}

	private bool AttackInRange(){

		bool canContinue = true;
		doDodge = false;

		// check for dodge in beginning
		if (dodgeCheck != null){
			if (myEnemyReference.GetPlayerReference().InAttack()){
				canContinue = false;
				doDodge = true;
			}
		}

		if (rangeCheck != null){
			rangeCheck.FindTarget();
			if (!rangeCheck.PlayerInRange()){
				canContinue = false;
			}
		}

		return canContinue;

	}

	public override void StartAction (bool setAnimTrigger = true)
	{
		base.StartAction (false);
		InitializeAction();

	}

	public override void EndAction (bool doNextAction = true)
	{
		
		base.EndAction (doNextAction);
		
	}
}
