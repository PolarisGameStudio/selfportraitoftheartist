﻿using UnityEngine;
using System.Collections;

public class EnemyShooterS : MonoBehaviour {

	private const float rotateAnimRate = 0.083f;

	private float rotateAnimCountdown;

	[Header("Attack Properties")]
	public GameObject projectileToSpawn;
	public float spawnTime;
	private float startSpawnTime;
	public float trackingTime = -1f;
	private bool useTracking = false;
	private bool foundTarget = false;
	private Vector3 aimDirection;

	[Header("Effect Properties")]
	public int shakeAmt = 0;
	public int startFlashFrames = 3;
	public int endFlashFrames = 3;
	public float rotateRate;
	public Vector3 rotateBase;
	public float growRate = 1.3f;
	public float fadeRate = 2f;
	private int flashFrames;
	public Texture flashTexture;

	private Renderer myRenderer;
	private Color myColor;
	private Texture startTexture;
	private SpriteRenderer timingIndicator;
	private Color timingIndicatorColor;
	private Vector3 timingIndicatorStartSize;
	private Transform poi;

	private bool firedProjectile = false;

	// Use this for initialization
	void Start () {

		poi = GameObject.Find("Player").transform;
		myRenderer = GetComponentInChildren<Renderer>();
		startTexture = myRenderer.material.GetTexture("_MainTex");
		myColor = myRenderer.material.color;

		rotateAnimCountdown = rotateAnimRate;

		myRenderer.material.SetTexture("_MainTex", flashTexture);
		myRenderer.material.color = Color.white;
		flashFrames = startFlashFrames;

		startSpawnTime = spawnTime;

		timingIndicator = GetComponentInChildren<SpriteRenderer>();
		timingIndicator.enabled = false;
		timingIndicatorStartSize = timingIndicator.transform.localScale;

		DoShake();

		if (trackingTime >= 0){
			useTracking = true;
		}
	
	}
	
	// Update is called once per frame
	void Update () {

		if (useTracking && !foundTarget){
			trackingTime -= Time.deltaTime;
			if (trackingTime <= 0){
				foundTarget = true;
				aimDirection = poi.transform.position-transform.position;
				aimDirection = aimDirection.normalized;
				aimDirection.z = 1f;
			}
		}

		if (flashFrames > 0){
			if (myRenderer.material.GetTexture("_MainTex") != flashTexture){
				myRenderer.material.SetTexture("_MainTex", flashTexture);
				myRenderer.material.color = Color.white;
			}
			flashFrames--;
		}
		else{
			if (myRenderer.material.GetTexture("_MainTex") != startTexture){
				myRenderer.material.SetTexture("_MainTex", startTexture);
				myRenderer.material.color = myColor;
			}

			if (!firedProjectile){

				spawnTime -= Time.deltaTime;

				if (spawnTime <= 0){
					flashFrames = endFlashFrames;

					timingIndicator.enabled = false;
	
					if (!foundTarget){
						aimDirection = poi.transform.position-transform.position;
						aimDirection = aimDirection.normalized;
						aimDirection.z = 1f;
					}

	
					GameObject newProjectile = Instantiate(projectileToSpawn, transform.position, Quaternion.identity)
						as GameObject;
					newProjectile.GetComponent<EnemyProjectileS>().Fire(aimDirection,null);
					firedProjectile = true;
				}else{

					if (!timingIndicator.enabled){
						timingIndicator.enabled = true;
					}

					timingIndicator.transform.localScale = timingIndicatorStartSize*(spawnTime/startSpawnTime);

					timingIndicatorColor = timingIndicator.color;
					timingIndicatorColor.a = 1f-spawnTime/startSpawnTime;
					timingIndicator.color = timingIndicatorColor;


					rotateAnimCountdown -= Time.deltaTime;
					if (rotateAnimCountdown <= 0){
						rotateAnimCountdown = rotateAnimRate;
						myRenderer.transform.RotateAround(myRenderer.transform.position, myRenderer.transform.up,
						                                  rotateRate*Time.deltaTime);
					}
				}
			}
			else{
				myColor = myRenderer.material.color;
				myColor.a -= fadeRate*Time.deltaTime;
				if (myColor.a <= 0){
					Destroy(gameObject);
				}
				else{
					myRenderer.material.color = myColor;
					transform.localScale += Vector3.one*growRate*Time.deltaTime;
				}
			}
		}
	
	}

	private void DoShake(){
		
		switch (shakeAmt){
			
		default:
			CameraShakeS.C.MicroShake();
			break;
		case(0):
			break;
		case(1):
			CameraShakeS.C.SmallShake();
			break;
		case(2):
			CameraShakeS.C.LargeShake();
			break;
			
		}
		
	}
}
