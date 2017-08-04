using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealthFeathersS : MonoBehaviour {

	public Animator[] myFeathers;
	private List<EnemyHealthFeatherColorS> featherSprites;
	private List<Animator> falseFeathers;
	public float maxStartInterval = 0.08f;
	private float currentInterval;

	private Vector3 falseFeatherScale = new Vector3 (0.65f, 0.65f, 1f);

	private float destroyFeatherTime = 0.1f;
	private float deathFeatherTime = 0.04f;

	private int currentStartFeather = 0;
	private int currentHealthFeather = 0;

	private bool allFeathersStarted = false;

	private float currentMaxHealthInterval;
	private float currentHealthInterval;

	private float startFloatPos;
	private float endFloatPos;
	private float endFloatDif = -0.5f;

	private Vector3 currentFloatPos;

	private float onTransparency = 0.8f;

	private float currentFloatTime;
	private float floatTimeMax = 6f;
	private float currentFloatT;
	private float floatDir = 1f;
	
	private bool isShowing = true;
	private bool hidingInvulnerable = false;

	private EnemyS myEnemy;
	public EnemyS enemyRef { get { return myEnemy; } }

	void Start(){
		
		currentFloatPos = transform.localPosition;
		endFloatPos = startFloatPos = currentFloatPos.y;
		startFloatPos -= endFloatDif/2f;
		endFloatPos += endFloatDif/2f;
		currentFloatTime = 0f;
		currentFloatPos.y = startFloatPos;
		transform.localPosition = currentFloatPos;
	}
	
	// Update is called once per frame
	void Update () {

		FeatherStart();
		Float ();
	
	}

	void FeatherStart(){
		if (!allFeathersStarted){
			currentInterval -= Time.deltaTime;
			if (currentInterval <= 0){
				myFeathers[currentStartFeather].SetTrigger("Start");
				falseFeathers[currentStartFeather].SetTrigger("Start");
				currentInterval = Random.Range(0, maxStartInterval);
				currentStartFeather++;
				if (currentStartFeather >= myFeathers.Length){
					currentStartFeather = 0;
					allFeathersStarted = true;
				}
			}
		}
	}

	void Initialize(){

		currentStartFeather = 0;

		currentInterval = Random.Range(0, maxStartInterval);
		allFeathersStarted = false;

	}

	void Float(){

		/*if (isShowing){
			if (myEnemy.invulnerable && !hidingInvulnerable){
				Hide(true);
				hidingInvulnerable = true;
			}
			if (!myEnemy.invulnerable && hidingInvulnerable){
				Show(true);
				hidingInvulnerable = false;
			}
		}**/

		if (allFeathersStarted){
			currentFloatTime += Time.deltaTime*floatDir;
			if (currentFloatTime >= floatTimeMax || currentFloatTime <= 0){
				floatDir *= -1f;
			}
			currentFloatT = currentFloatTime/floatTimeMax;
			currentFloatT = Mathf.Sin(currentFloatT * Mathf.PI * 0.5f);
			currentFloatPos.y = Mathf.Lerp(startFloatPos, endFloatPos, currentFloatT);
			transform.localPosition = currentFloatPos;
		}

	}

	public void SetUpEnemy(EnemyS newEnemy){
		newEnemy.SetHealthDisplay(this);
		myEnemy = newEnemy;
		currentHealthInterval = currentMaxHealthInterval = myEnemy.actingMaxHealth/(myFeathers.Length*1f);
		currentHealthFeather = myFeathers.Length-1;

		featherSprites = new List<EnemyHealthFeatherColorS>();
		falseFeathers = new List<Animator>();
		Color falseColor = myEnemy.bloodColor;
		falseColor.a = 0.25f;
		for (int i = 0; i < myFeathers.Length; i++){
			featherSprites.Add(myFeathers[i].GetComponent<EnemyHealthFeatherColorS>());
			featherSprites[i].SetUpFeather(myEnemy.bloodColor);

			falseFeathers.Add(featherSprites[i].transform.GetChild(0).GetComponent<Animator>());
			featherSprites[i].transform.GetChild(0).localScale = falseFeatherScale;
			falseFeathers[i].GetComponent<SpriteRenderer>().color = falseColor;
		}

		Initialize();
	}

	public void EnemyHit(float damageAmt){
		StartCoroutine(HitEffect(damageAmt));
	}

	IEnumerator HitEffect(float damageAmt){

		float dmgToProcess = damageAmt;
		/*if (dmgToProcess > currentHealthInterval + (currentMaxHealthInterval*currentHealthFeather)){
			dmgToProcess = currentHealthInterval + currentMaxHealthInterval*currentHealthFeather;
		}*/

		while (dmgToProcess > 0 && currentHealthFeather >= 0){

			if (dmgToProcess > currentHealthInterval){
				dmgToProcess-=currentHealthInterval;
				currentHealthInterval = currentMaxHealthInterval;
				myFeathers[currentHealthFeather].SetTrigger("Destroy");
				featherSprites[currentHealthFeather].FlashWhite(true);
				currentHealthFeather--;

				yield return new WaitForSeconds(destroyFeatherTime);
			}else{
				currentHealthInterval -= dmgToProcess;
				myFeathers[currentHealthFeather].SetTrigger("Hit");
				featherSprites[currentHealthFeather].FlashWhite();

				// make sure we don't infinite loop for some reason
				dmgToProcess = 0;
			}
			
			yield return null;
		}

		if (myEnemy.currentHealth <= 0){
			StartCoroutine(DestroyFalseFeathers());
		}
	}

	IEnumerator DestroyFalseFeathers(){

		for (int i = falseFeathers.Count-1; i >= 0; i--){
			falseFeathers[i].SetTrigger("Destroy");
			yield return new WaitForSeconds(deathFeatherTime);
		}
	}

	public void Show(bool showingOverride = false){
		if (!isShowing){
			if (!showingOverride){
				isShowing = true;
			}
			for (int i = 0; i < myFeathers.Length; i++){
				myFeathers[i].gameObject.SetActive(true);
			}
		}else if (showingOverride && isShowing){
			SetTransparency(true);
		}
	}
	public void Hide(bool showingOverride = false){
		if (isShowing){
			if (!showingOverride){
				isShowing = false;
				for (int i = 0; i < myFeathers.Length; i++){
					myFeathers[i].gameObject.SetActive(false);
				}
				}else{
				SetTransparency(false);
				}


		}
	}

	public void ChangeFeatherColor(Color newCol){
		for (int i = 0; i < featherSprites.Count; i++){
			featherSprites[i].ChangeCurrentColor(newCol);
		}
		onTransparency = newCol.a;
		for (int i = 0; i < falseFeathers.Count; i++){
			newCol.a = 0.3f;
			falseFeathers[i].GetComponent<SpriteRenderer>().color = newCol;
		}
	}

	void SetTransparency(bool showing){
		Color newCol = Color.grey;
		float newAlpha = onTransparency;
		if (!showing){
			newAlpha = 0f;
		}
			for (int i = 0; i < featherSprites.Count; i++){
			newCol = featherSprites[i].rendererRef.color;
			newCol.a = newAlpha;
				featherSprites[i].ChangeCurrentColor(newCol);
			}
		if (showing){
		newAlpha = 0.25f;
		}
			for (int i = 0; i < falseFeathers.Count; i++){
				newCol.a = newAlpha;
				falseFeathers[i].GetComponent<SpriteRenderer>().color = newCol;
			}


	}
}
