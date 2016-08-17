﻿using UnityEngine;
using System.Collections;

public class ShootBuddyS : BuddyS {

	public GameObject myProjectile;

	private EnemyDetectS myEnemyDetect;

	public float shootRate;
	private float shootCountdown;

	public float shotDelay = 0.08f;
	private float shotDelayCountdown = 0f;
	private bool shotTriggered = false;

	private bool chargeButtonUp = true;


	public string chargeAnimatorTrigger;
	public string fireAnimatorTrigger;

	// Use this for initialization
	void Start () {

		Initialize();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		ShootControl();
		FollowPlayer();
		FaceDirection();
	
	}

	public override void Initialize(){

		base.Initialize();

		myEnemyDetect = playerRef.myDetect;
	}

	public override void FaceDirection(){

		if (!myEnemyDetect){
			
			myEnemyDetect = playerRef.myDetect;
		}

		if (myEnemyDetect.closestEnemy != null && charging){
			Vector3 fScale = transform.localScale;
			if (myEnemyDetect.closestEnemy.transform.position.x > transform.position.x){
				fScale.x = sScale;
			}
			
			if (myEnemyDetect.closestEnemy.transform.position.x < transform.position.x){
				fScale.x = -sScale;
			}
			transform.localScale = fScale;

		}else if (shootCountdown > 0){
			Vector3 fScale = transform.localScale;
			if (myRigid.velocity.x > 0){
				fScale.x = -sScale;
			}
			
			if (myRigid.velocity.x < 0){
				fScale.x = sScale;
			}
			transform.localScale = fScale;
		}else{
			base.FaceDirection();
		}

	}

	private void ShootControl(){

		if (shotTriggered){
			shotDelayCountdown -= Time.deltaTime;
			if (shotDelayCountdown <= 0){
				FireProjectile();
				shotTriggered = false;
			}
		}
		else{
			shootCountdown -= Time.deltaTime;
	
			if (!playerRef.talking && !playerRef.myStats.PlayerIsDead()){
	
				if (!playerRef.myControl.FamiliarControl()){
					chargeButtonUp = true;
				}else{
					if (chargeButtonUp){
						if (shootCountdown <= 0 && playerRef.myStats.ChargeCheck(costPerUse)){
							myAnimator.SetTrigger(fireAnimatorTrigger);
							if (shotDelay <= 0){
								FireProjectile();
							}else{
								shotDelayCountdown = shotDelay;
								shotTriggered = true;
							}
						}
						chargeButtonUp = false;
					}
				}
			}
		}

	}

	private void FireProjectile(){

		myAnimator.SetTrigger(fireAnimatorTrigger);

		Vector3 aimDir = Vector3.zero;

		if (myEnemyDetect.closestEnemy != null){
			aimDir.x = myEnemyDetect.closestEnemy.transform.position.x - transform.position.x;
			aimDir.y = myEnemyDetect.closestEnemy.transform.position.y - transform.position.y;
		}else if (playerRef.myRigidbody.velocity.x != 0 || playerRef.myRigidbody.velocity.y != 0){
			aimDir.x = playerRef.myRigidbody.velocity.x;
			aimDir.y = playerRef.myRigidbody.velocity.y;
		}else{
			aimDir.x = myRigid.velocity.x;
			aimDir.y = myRigid.velocity.y;
		}

		GameObject myProj = Instantiate(myProjectile, transform.position, Quaternion.identity)
			as GameObject;
		myProj.transform.position += aimDir.normalized*myProj.GetComponent<BuddyProjectileS>().attackSpawnDistance;

		myProj.GetComponent<BuddyProjectileS>().Fire(aimDir.normalized, this);

		charging = false;
		shootCountdown = shootRate;

	}
}
