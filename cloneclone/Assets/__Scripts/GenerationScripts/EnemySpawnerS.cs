﻿using UnityEngine;
using System.Collections;

public class EnemySpawnerS : MonoBehaviour {

	[HideInInspector]
	public CombatManagerS myManager;

	private bool _enemySpawned = false;
	public bool enemySpawned { get { return _enemySpawned; } }
	private EnemyS currentEnemyReference;
	public EnemyS currentSpawnedEnemy { get { return currentEnemyReference; } }

	public float enemySpawnDelay = 0f;
	public float chanceToSpawn = 1f;
	private bool didNotSpawnEnemy = false;

	public GameObject[] enemyPool;
	public int enemySpawnID = -1;

	//private RoomClearCheck parentClear;
	private InfinitySpawnS parentClear;
	private bool sentClearMessage = false;
	public bool sentMessage { get { return sentClearMessage; } }

	[Header("Item Properties")]
	public GameObject dropOnDefeat;

	private Vector3 savedSpawnPt;

	// Use this for initialization
	void Start () {

		//parentClear = GetComponentInParent<RoomClearCheck>();
		parentClear = GetComponentInParent<InfinitySpawnS>();

		if (enemySpawnDelay <= 0){
			SpawnEnemy();
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (!_enemySpawned){
			enemySpawnDelay -= Time.deltaTime;
			if (enemySpawnDelay <= 0){
				SpawnEnemy();
			}
		}

		if (EnemiesDefeated() && !sentClearMessage){
			sentClearMessage = true;
			if (parentClear){
				parentClear.AddClear();
			}
			/*if (enemySpawnID > -1){
				PlayerInventoryS.I.dManager.AddEnemyDefeated(enemySpawnID, currentEnemyReference.transform.position);
			}**/
		}
	
	}

	public void SetWitchTime(bool witchOn){
		if (_enemySpawned && currentEnemyReference != null){
			if (witchOn){
				currentEnemyReference.StartWitchTime();
			}else{
				currentEnemyReference.EndWitchTime();
			}
		}
	}

	public void SaveEnemyDefeated(){
		if (enemySpawnID > -1){
			PlayerInventoryS.I.dManager.AddEnemyDefeated(enemySpawnID, currentEnemyReference.transform.position);
		}
	}

	private void SpawnEnemy(){

		float chanceEnemySpawns = Random.Range(0f, 1f);

		if (chanceEnemySpawns <= chanceToSpawn){
	
			int enemyToSpawn = Mathf.RoundToInt(Random.Range(0, enemyPool.Length));
	
			GameObject newEnemy = Instantiate(enemyPool[enemyToSpawn], transform.position, Quaternion.identity)
				as GameObject;

			savedSpawnPt = transform.position;
	
			currentEnemyReference = newEnemy.GetComponent<EnemyS>();
			currentEnemyReference.mySpawner = this;
	
			//newEnemy.transform.parent = transform;

			//Debug.Log("Enemy defeated?" + (enemySpawnID > -1 && PlayerInventoryS.I.dManager.enemiesDefeated.Contains(enemySpawnID)));
			if (enemySpawnID > -1 && PlayerInventoryS.I.dManager.enemiesDefeated.Contains(enemySpawnID)){
				currentEnemyReference.transform.position = PlayerInventoryS.I.dManager.enemiesDefeatedPos
					[PlayerInventoryS.I.dManager.enemiesDefeated.IndexOf(enemySpawnID)];
				currentEnemyReference.KillWithoutXP();
			}

		}else{
			didNotSpawnEnemy = true;
		}
		
		
		_enemySpawned = true;

	}

	public void RespawnEnemies(bool usedSaveSpawn = true){

		if (!didNotSpawnEnemy && currentEnemyReference != null){
			currentEnemyReference.gameObject.SetActive(true);
			currentEnemyReference.Reinitialize();
			if (usedSaveSpawn){
				currentEnemyReference.transform.position = savedSpawnPt;
			}else{
				currentEnemyReference.transform.position = transform.position;
			}
			//currentEnemyReference.transform.parent = transform;
		}

	}

	public void Unspawn(){

		if (!didNotSpawnEnemy && currentEnemyReference != null){
			currentEnemyReference.gameObject.SetActive(false);
			currentEnemyReference.myAnimator.SetBool("Death", false);
		}
	}

	public void KillWithoutXP(){
		if (!didNotSpawnEnemy && currentEnemyReference != null){
			currentEnemyReference.KillWithoutXP();
		}
	}

	public void ChangeFeatherColor(Color newC){
		if (!didNotSpawnEnemy && currentEnemyReference != null){
			currentEnemyReference.ChangeFeatherColor(newC);
		}
	}

	public bool EnemiesDefeated(){

		if (didNotSpawnEnemy){
			return true;
		}
		else{
			if (currentEnemyReference != null){
				return (currentEnemyReference.isDead);
			}
			else{
				return false;
			}
		}

	}

	public void DropOnDefeat(){
		if (dropOnDefeat){
			Vector3 spawnPos = currentSpawnedEnemy.transform.position;
			spawnPos.z = dropOnDefeat.transform.position.z;
			Instantiate(dropOnDefeat, spawnPos, Quaternion.identity);
		}
	}

	public bool SpawnedEnemyIsActive(){
		bool active = false;
		if (!didNotSpawnEnemy && currentEnemyReference != null){
			if (!currentEnemyReference.isDead && currentEnemyReference.gameObject.activeSelf){
				active = true;
			}
		}
		return active;
	}
}
