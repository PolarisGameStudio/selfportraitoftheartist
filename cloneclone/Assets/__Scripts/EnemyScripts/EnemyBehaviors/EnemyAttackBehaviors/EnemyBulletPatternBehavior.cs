﻿using UnityEngine;
using System.Collections;

public class EnemyBulletPatternBehavior: EnemyBehaviorS {

	public PlayerDetectS rangeCheck;

	[Header("Behavior Duration")]
	public float attackDuration = 3f;
	public float attackWarmup = 1f;
	private bool launchedAttack = false;

	[Header ("Behavior Physics")]
	public GameObject attackPrefab;
	public Transform[] attackTargets;
	public float attackDragAmt = -1;
	public bool setVelocityToZeroOnStart = false;


	private Vector3 attackDirection;

	private float attackTimeCountdown;
	
	// Update is called once per frame
	void FixedUpdate () {

		if (BehaviorActing()){
			
			
			BehaviorUpdate();
			
			if (attackTimeCountdown <= 0 || myEnemyReference.behaviorBroken){
				EndAction();
			}

			attackTimeCountdown -= Time.deltaTime;
		

			if (!launchedAttack && attackTimeCountdown <= (attackDuration-attackWarmup)){
				GameObject attackObj;
				foreach (Transform t in attackTargets){
					SetAttackDirection(t);
				attackObj = Instantiate(attackPrefab, transform.position, attackPrefab.transform.rotation)
					as GameObject;
				EnemyProjectileS projectileRef = attackObj.GetComponent<EnemyProjectileS>();
				projectileRef.Fire(attackDirection, myEnemyReference);
				}
				launchedAttack = true;

				myEnemyReference.SetBreakState(9999f,0f);
			}
		}
	
	}

	private void InitializeAction(){

		if (AttackInRange()){

			launchedAttack = false;
			attackTimeCountdown = attackDuration;
			
			myEnemyReference.myAnimator.SetTrigger(animationKey);
			
	
			if (attackDragAmt > 0){
				myEnemyReference.myRigidbody.drag = attackDragAmt;
			}
	
			if (setVelocityToZeroOnStart){
				myEnemyReference.myRigidbody.velocity = Vector3.zero;
			}
		}
		else{
			myEnemyReference.myAnimator.SetTrigger("Idle");
			EndAction();
		}

	}

	private bool AttackInRange(){

		bool canContinue = true;

		if (rangeCheck != null){
			rangeCheck.FindTarget();
			if (!rangeCheck.PlayerInRange()){
				canContinue = false;
			}
		}

		return canContinue;

	}

	private void SetAttackDirection(Transform currentTarget){

		attackDirection = (currentTarget.position - transform.position).normalized;
		attackDirection.z = 0;
		myEnemyReference.SetTargetReference(attackDirection);

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
