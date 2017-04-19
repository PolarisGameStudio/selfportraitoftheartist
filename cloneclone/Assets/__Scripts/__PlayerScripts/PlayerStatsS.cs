using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatsS : MonoBehaviour {

	private const float NO_MANA_STOP_TIME = 0.1f;
	private const float NEAR_DEATH_STOP_TIME = 0.1f;
	private const float DEATH_KNOCKBACK_MULT = 2f;
	private const float BIG_KNOCKBACK_TIME = 0.4f;
	private const float DEATH_DRAG = 3.4f;
	private const float CAN_USE_MANA = 0.25f;
	private const float BREAK_STAMINA_PENALTY = 1.5f;

	private const float anxiousChargeRate = 0.1f;

	private const float DARKNESS_ADD_RATE = 0.001f;
	private const float DARKNESS_ADD_DEATH = 2f/3f;
	public const float DARKNESS_MAX = 100f;
	
	private const float VIRTUE_ADD_AMT = 4f;

	public static bool healOnStart = false;

	private PlayerController myPlayerController;
	public PlayerController pRef { get { return myPlayerController; } }

	public static bool godMode = false;
	public static bool dontDoCountUp = false;

	public static bool PlayerCantDie = false;

	//________________________________LEVEL
	private int _startLevel = 1;
	private int _addedLevel = 0;
	public int currentLevel { get { return _startLevel+_addedLevel; } }

	//________________________________HEALTH
	private float _baseHealth = 8;
	private float _addedHealth = 0; // max 8 (for 12 total)
	public float addedHealth { get { return _addedHealth; } }
	private static float _currentHealth;

	public float currentHealth { get { return _currentHealth; } }
	public float maxHealth { get { return (_baseHealth+_addedHealth);}}
	private float _savedHealth = 8f;
	public float savedHealth { get { return _savedHealth; } }

	public static float _currentDarkness = 0f;
	public float currentDarkness {get { return _currentDarkness; } }
	
	//________________________________MANA
	private float _baseMana = 4;
	private float _addedMana = 0; // max 16 (for 20 total)
	public float addedMana { get { return _addedMana; } }
	private float _currentMana;
	private RefreshDisplayS myRefresh;

	private float _savedMana = 5;
	
	public float maxMana { get { return (_baseMana+_addedMana);}}
	public float currentMana { get { return (_currentMana);}}

	private float _overchargeMana;
	public float overchargeMana { get { return _overchargeMana; } }
	private float overchargePenalty = 0.4f;

	//________________________________CHARGE
	private float _baseCharge = 5f;
	private float _addedCharge = 0;
	public float addedCharge { get { return _addedCharge; }}
	private static float _currentCharge = 0f;

	public float addedChargeLv {get { return Mathf.Round(_addedCharge/10f); } }
	
	public float maxCharge { get { return _baseCharge+_addedCharge;}}
	public float currentCharge { get { return _currentCharge;}}
	private float _savedCharge = 50f;

	private bool _exhausted = false;

	
	//________________________________CHARGE RECOVERY

	private int _currentChargeRecoverLv = 1;
	private float _baseChargeRecover = 0.8f;
	private float _addedChargeRecoverPerLevel = .175f;
	public float currentChargeRecover  { get { return 
			_baseChargeRecover+_addedChargeRecoverPerLevel*(_currentChargeRecoverLv*1f-1f); } }

	public float currentChargeRecoverLv { get { return _currentChargeRecoverLv*1f; } }


	//________________________________ATTACK
	private float _baseStrength = 0.6f;
	private float _addedStrength = 0; // (upgradeable)
	public float strengthAmt { get { return (_baseStrength+_addedStrength*0.1f);}}
	public float addedStrength { get { return _addedStrength; } }

	
	private float _baseCrit = 0;
	private float _addedCrit = 0; // (upgradeable)
	public float critAmt { get { return (_baseCrit+_addedCrit);}}
	
	//________________________________VIRTUE
	private float _baseVirtue = 6f;
	private float _addedVirtue = 0; // (upgradeable)
	public float addedVirtue { get { return _addedVirtue; } }
	private float _usedVirtue = 0;
	public float usedVirtue {get {return _usedVirtue; } }
	public float usedVirtuePercent {get { return _usedVirtue/(_baseVirtue+_addedVirtue); } }
	public float virtueAmt { get { return (_baseVirtue+_addedVirtue);}}

	//________________________________DEFENSE
	private float _baseDefense = 9f;
	private float _addedDefense = 0;
	private float _currentDefense;

	public float addedDefense { get { return _addedDefense; } }

	public float maxDefense { get { return (_baseDefense+_addedDefense);}}
	public float currentDefense { get { return _currentDefense; } }

	//_______________________________SPEED
	private float _baseSpeed = 1f;
	private float _addedSpeed = 0f;
	public float speedAmt { get { return (_baseSpeed+_addedSpeed);}}


	//________________________________RECOVERY
	private float _baseRecovery = 1f;
	private float _addedRecovery = 0f;
	public float currentRecovery { get { return _baseRecovery+_addedRecovery; } }

	private float _recoveryCooldownBase = 0.2f;
	private float _recoveryCooldownMultiplier = 1f; // higher = slower cooldown (upgradeable)
	public float recoveryCooldownMax { get { return (_recoveryCooldownBase*(_recoveryCooldownMultiplier-
			                                                                        (0.5f*_recoveryCooldownMultiplier*(currentRecovery-1f))));}}
	private float _currentCooldownTimer;
	public float currentCooldownTimer { get { return _currentCooldownTimer; } }
	
	private float blockRecoverMult = 0.5f;

	private float _recoverRateMin = 2f;
	private float _recoverRateMultiplier = 1f; // higher = faster recovery (upgradeable)

	public float recoverRate { get { return (_recoverRateMin*_recoverRateMultiplier);}}

	private float recoverRateIncrease;
	private float recoverRateAccelBase = 4f;
	private float recoverRateAccelAddPerLevel = 0.25f;
	private float recoverRateAccel { get { return recoverRateAccelBase+recoverRateAccelAddPerLevel*(_recoverRateLv*1f-1f); } }
	private int _recoverRateLv = 1;
	private int _addedRateLv = 0;
	public int currentRecoverRateLv { get { return _recoverRateLv+_addedRateLv; } }

	public int addedRateLv { get { return _addedRateLv; } }
	
	private float recoverBurdenMin = 0.08f;
	private float recoverBurdenMax = 0.22f;
	private float currentRegenCountdown;
	public float currentRegenCount { get { return currentRegenCountdown; } }

	private float _currentManaUsed = 0;

	//________________________________________DEFENSE

	private float _defenseKnockbackMult = 0.5f;
	private float _extraKnockbackMult = 2f;
	private BlockDisplay3DS myBlocker;

	//________________________________________OTHER
	private PlayerStatDisplayS _uiReference;
	public PlayerStatDisplayS uiReference { get { return _uiReference; } }

	private ItemEffectS _itemEffect;
	public ItemEffectS itemEffect { get { return _itemEffect; } }

	private FlashEffectS _hurtFlash;
	private FlashEffectS _killFlash;

	//_____________________________________UNITY FUNCTIONS

	// Use this for initialization
	void Start () {
		Initialize();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		ManaRecovery();
		//FakeManaRecovery();
		ChargeRecovery();
		DarknessAdd();
	}

	//________________________________________PUBLIC FUNCTIONS

	public bool ManaCheck(float useAmount, bool reduce = true){

		if (godMode){
			return true;
		}else if (!ManaUnlocked()){
			return false;
		}
		else{
		if (_currentMana > 0){
			if (reduce){
			if (_currentMana >= useAmount){
				_currentMana -= useAmount;
				
				_currentManaUsed += useAmount;

			}else{
				_currentManaUsed += _currentMana;

						_overchargeMana = useAmount-_currentMana;
						if (_overchargeMana > maxMana){
							_overchargeMana = maxMana;
						}
						// taking out overcharge for a bit
						_overchargeMana = 0;
				_currentMana = 0;
						_exhausted = true;

			}

			_currentCooldownTimer = recoveryCooldownMax;
					if (_currentMana <= 0){
						_currentCooldownTimer*=BREAK_STAMINA_PENALTY;
					}

			currentRegenCountdown = GetRegenTime();

			}

			return true;
		}
		else{
			return false;
		}
		}
	}

	public float GetRegenTime(){
		float baseBurden = recoverBurdenMin+_currentManaUsed/(_baseMana+_addedMana)*recoverBurdenMax;

		return baseBurden - baseBurden*0.2f*(currentRecovery-1f);
	}

	public bool ChargeCheck(float reqCharge, bool useCharge = true){

		bool canUse =  (_currentCharge > 0);

		if (useCharge && canUse){
			if (reqCharge > _currentCharge){
				reqCharge = _currentCharge;
			}
			_currentCharge -= reqCharge;
			_uiReference.ChargeUseEffect(reqCharge);
		}

		if (_currentCharge < 0f){
			_currentCharge = 0f;
		}
		return canUse;
	}

	public void RecoverCharge(float addPercent, bool itemEffect = false){
		// add to charge (old)
		float amtAdded = addPercent*maxCharge*currentChargeRecover;
		if (_currentCharge + amtAdded > maxCharge){
			amtAdded = maxCharge-_currentCharge;
		}
		_uiReference.ChargeAddEffect(amtAdded);
		_currentCharge += amtAdded;
		if (_currentCharge > maxCharge){
			_currentCharge = maxCharge;
		}
		if (itemEffect){
			
			myPlayerController.FlashCharge();
			CameraShakeS.C.SmallShakeCustomDuration(0.6f);
			CameraShakeS.C.TimeSleep(0.08f);
			_itemEffect.Flash(myPlayerController.myRenderer.material.color);
		
		}

		// add to stamina (new)
		/*float amtAdded = addPercent*maxMana*currentChargeRecover;
		if (_overchargeMana > 0){
			if (_overchargeMana - amtAdded <= 0){
				amtAdded -= _overchargeMana;
				_overchargeMana = 0;
			}else{
				_overchargeMana -= amtAdded;
				amtAdded = 0;
			}
		}
		if (_currentMana + amtAdded > maxMana){
			amtAdded = maxMana-_currentMana;
		}
		//_uiReference.ChargeAddEffect(amtAdded);
		_currentMana += amtAdded;
		if (_currentMana > maxMana){
			_currentMana = maxMana;
		}
		if (itemEffect){

			myPlayerController.FlashCharge();
			CameraShakeS.C.SmallShakeCustomDuration(0.6f);
			CameraShakeS.C.TimeSleep(0.08f);
			_itemEffect.Flash(myPlayerController.myRenderer.material.color);

		}**/
	}

	public void AddUIReference(PlayerStatDisplayS sd){
		_uiReference = sd;
	}

	//________________________________________PRIVATE FUNCTIONS

	private void DarknessAdd(){
		if (!PlayerIsDead() && !myPlayerController.talking){
			_currentDarkness += Time.deltaTime*DARKNESS_ADD_RATE;
		}
	}

	private void ChargeRecovery(){
		if (myPlayerController.playerAug.anxiousAug){
			if (_currentCharge < maxCharge){
				RecoverCharge(anxiousChargeRate*Time.deltaTime);
			}
		}
	}

	private void FakeManaRecovery(){
		if (RecoveryCheck()){

			// first burn down cooldown, then recover
			if (_currentCooldownTimer > 0){
				_currentCooldownTimer -= Time.deltaTime;
				if (_currentCooldownTimer < 0){

					_currentCooldownTimer = 0;
				}
				recoverRateIncrease = 0f;
			}
		}
	}

	private void ManaRecovery(){

		if (RecoveryCheck()){

			// first burn down cooldown, then recover
			if (_currentCooldownTimer > 0){
				_currentCooldownTimer -= Time.deltaTime;
				if (_currentCooldownTimer < 0){
					
					_currentCooldownTimer = 0;
				}
				recoverRateIncrease = 0f;
			}
			else{

				float actingRecoverRate = recoverRate + recoverRateIncrease;

				// new way (simple)
				if (myPlayerController.isBlocking){
					actingRecoverRate*=blockRecoverMult;
				}else{
					recoverRateIncrease+=recoverRateAccel*Time.deltaTime;
				}
				if (_overchargeMana <= 0){
					_currentMana+=actingRecoverRate*Time.deltaTime;
				_currentManaUsed-=actingRecoverRate*Time.deltaTime;
				}else{
					_overchargeMana -= actingRecoverRate*Time.deltaTime*overchargePenalty;
				}

				if (_exhausted){
					if (_currentMana >= maxMana*CAN_USE_MANA){
						_exhausted = false;
					}
				}

				if (_currentMana > maxMana){
					_currentMana = maxMana;
				}


			}

		}

	}

	private bool RecoveryCheck(){

		bool canRecover = true;

		if (_currentMana < maxMana && !PlayerIsDead()){
			if (myPlayerController != null){
				if (myPlayerController.isDashing || myPlayerController.isSprinting || myPlayerController.InAttack()
				//if (myPlayerController.InSpecialAttack()
				    || myPlayerController.chargingAttack){
					canRecover = false;
				}
			}
			else{
				canRecover = false;
			}
		}
		else{
			canRecover = false;
		}

		return canRecover;

	}

	public bool ManaUnlocked(){
		bool canUseMana = true;
		if (RecoveryCheck() && _exhausted && _currentMana < maxMana*CAN_USE_MANA){
			canUseMana = false;
		}
		return canUseMana;
	}

	private void Initialize(){

		myPlayerController = GetComponent<PlayerController>();
		myPlayerController.SetStatReference(this);

		InitializeStats();

		_hurtFlash = CameraEffectsS.E.hurtFlash;
		_killFlash = CameraEffectsS.E.deathFlash;

		_itemEffect = GetComponentInChildren<ItemEffectS>();

		_currentMana = maxMana;
		if (PlayerController.doWakeUp || healOnStart){
			_currentHealth = maxHealth;
			_currentCharge = maxCharge;
			PlayerInventoryS.I.RefreshRechargeables();
			healOnStart = false;
		}
		_currentDefense = maxDefense;

	}

	private void InitializeStats(){
		if (PlayerInventoryS.I.earnedUpgrades.Count > 0){
			foreach(int i in PlayerInventoryS.I.earnedUpgrades){
				// count mana
				if (i == 0){
					_addedHealth++;
				}
				if (i == 1){
					_addedMana++;
				}
				if (i == 2){
					_addedCharge+=1f;
				}
				if (i == 3){
					//_addedStrength++;
					_addedVirtue += VIRTUE_ADD_AMT;
				}
				if (i == 4){
					_currentChargeRecoverLv++;
				}
				if (i == 5){
					_addedRateLv++;
				}
				if (i == 6){
					_addedStrength++;
				}
				_addedLevel++;
			}
		}
	}

	public void AddStat(int i){
		if (i == 0){
			_addedHealth++;
			_currentHealth++;
		}
		if (i == 1){
			_addedMana++;
			_currentMana++;
		}
		if (i == 2){
			_currentCharge+=1f;
			_addedCharge+=1f;
		}
		if (i == 3){
			//_addedStrength++;
			_addedVirtue += VIRTUE_ADD_AMT;
		}
		if (i == 4){
			_currentChargeRecoverLv++;
		}
		if (i == 5){
			_addedRateLv++;
		}
		if (i == 6){
			_addedStrength ++;
		}
		_addedLevel++;
	}

	public void ResetStamina(){

		_currentMana = maxMana;
		_overchargeMana = 0f;
		_currentCooldownTimer = 0f;
		_currentManaUsed = 0f;
		myPlayerController.FlashMana(true);
		CameraShakeS.C.SmallShakeCustomDuration(0.6f);
		CameraShakeS.C.TimeSleep(0.08f);
		_itemEffect.Flash(myPlayerController.myRenderer.material.color);

	}

	public void Heal(float healAmt){
		_currentHealth += healAmt;
		if (_currentHealth > maxHealth){
			_currentHealth = maxHealth;
		}
		myPlayerController.FlashHeal();
		_itemEffect.Flash(myPlayerController.myRenderer.material.color);
		CameraShakeS.C.SmallShakeCustomDuration(0.6f);
		CameraShakeS.C.TimeSleep(0.08f);
	}

	public void TakeDamage(EnemyS damageSource, float dmg, Vector3 knockbackForce, float knockbackTime){

		if (!PlayerIsDead() && !myPlayerController.allowCounterAttack && !myPlayerController.doingCounterAttack && !myPlayerController.usingitem
		    && !myPlayerController.delayWitchTime && (!myPlayerController.isDashing || (myPlayerController.isDashing && myPlayerController.IsSliding())) 
		    && !myPlayerController.talking){
			if (myPlayerController.isBlocking && _currentDefense > 0){
				if (!godMode){
				_currentDefense-=dmg;
				}
				if (_currentDefense <= 0){
					_currentDefense = 0;
					CameraShakeS.C.TimeSleep(NO_MANA_STOP_TIME);
					CameraShakeS.C.SmallShake();
					myPlayerController.myRigidbody.AddForce(knockbackForce*_extraKnockbackMult, ForceMode.Impulse);
					myPlayerController.Stun(BIG_KNOCKBACK_TIME);
					myPlayerController.myAnimator.SetTrigger("Hurt");
					myPlayerController.FlashMana();

				}
				else{
					myBlocker.DoFlash();
					myPlayerController.myRigidbody.AddForce(knockbackForce*_defenseKnockbackMult, ForceMode.Impulse);
					CameraShakeS.C.SmallShake();
				}
				if (damageSource != null){
					damageSource.Deflect();
				}
			}else{
				
				myPlayerController.Stun(knockbackTime);
				myPlayerController.myAnimator.SetTrigger("Hurt");
				myPlayerController.FlashDamage();

				EffectSpawnManagerS.E.SpawnDamangeNum(transform.position, false, true, dmg);
					
				if(!godMode){
					
					if (_currentHealth > maxHealth*0.011f && _currentHealth-dmg <= 0 
					    && myPlayerController.playerAug.unstoppableAug){
						_currentHealth = maxHealth*0.01f;
					}else{
						_currentHealth -= dmg;
					}
					if (PlayerCantDie && _currentHealth <= 0){
						_currentHealth = 0.1f;
					}
					CameraShakeS.C.CancelSloMo();
					//ChargeCheck(10f);
				}
				if (_currentHealth <= 0){
					myPlayerController.playerSound.PlayDeathSound();
					myPlayerController.playerSound.SetWalking(false);
						_currentHealth = 0;
						myPlayerController.myRigidbody.drag = DEATH_DRAG;
						myPlayerController.myRigidbody.AddForce(knockbackForce*DEATH_KNOCKBACK_MULT, ForceMode.Impulse);
						myPlayerController.myAnimator.SetTrigger("Dead");
						myPlayerController.myAnimator.SetBool("IsDead", true);

					List<int> saveBuddyList = new List<int>();
					saveBuddyList.Add(myPlayerController.ParadigmIBuddy().buddyNum);
					if (myPlayerController.ParadigmIIBuddy() != null){
						saveBuddyList.Add(myPlayerController.ParadigmIIBuddy().buddyNum);
					}
					PlayerInventoryS.I.SaveLoadout(myPlayerController.equippedWeapons, myPlayerController.subWeapons,
					                               saveBuddyList);
					//_uiReference.cDisplay.DeathPenalty();

					if (dontDoCountUp){
						dontDoCountUp = false;
					}else{
						_uiReference.transform.parent.GetComponentInChildren<DarknessPercentUIS>().ActivateDeathCountUp();
						_currentDarkness += DARKNESS_ADD_DEATH;
					}

					_killFlash.Flash();

					myPlayerController.myLockOn.enemyHealthUI.EndLockOn();

					GetComponent<BleedingS>().StartDeath();

					CameraFollowS.F.RemoveLimits();
					}
				else{
					_hurtFlash.Flash();
					myPlayerController.playerSound.PlayHurtSound();
					myPlayerController.myRigidbody.AddForce(knockbackForce, ForceMode.Impulse);
					}
				//}

				if (_currentHealth <= 0){
					CameraShakeS.C.LargeShake();
					CameraShakeS.C.TimeSleepBigPunch(0.4f);
					CameraShakeS.C.DeathTimeEffect();
				}
				else if (_currentHealth <= 1){
					CameraShakeS.C.LargeShake();
					CameraShakeS.C.TimeSleep(0.12f, true);
				}else{
					CameraShakeS.C.SpecialAttackShake();
					CameraShakeS.C.TimeSleep(0.08f);
				}
	
			}
		}else{
			if (myPlayerController.AllowDodgeEffect()){
				myPlayerController.WitchTime(damageSource);
			}
		}

	}

	public void ActivateDefense(){
		_currentDefense = maxDefense;
	}

	public bool PlayerIsDead(){
		return (_currentHealth <= 0);
	}

	public void AddBlocker(BlockDisplay3DS newBlock){
		myBlocker = newBlock;
	}
	public void AddRefresh(RefreshDisplayS newBlock){
		myRefresh = newBlock;
	}

	public float OverCooldownMult(){
		return (_currentCooldownTimer/(recoveryCooldownMax*BREAK_STAMINA_PENALTY));
	}

	//__________________________________STAT UPGRADES
	public void AddStamina(float numToAdd = 1){
		_addedMana+=numToAdd;
		_currentMana+=numToAdd;
		_addedLevel++;
	}

	public void AddHealth(float numToAdd = 1){
		_addedHealth += numToAdd;
		_currentHealth += numToAdd;
		_addedLevel++;
	}
	public void AddCharge(float numToAdd = 10){
		_addedCharge += numToAdd;
		_currentCharge += numToAdd;
		_addedLevel++;
	}

	public void ChangeVirtue(float numChange){
		_usedVirtue += numChange;
		if (_usedVirtue < 0){
			_usedVirtue = 0;
		}
	}

	public void FullRecover(){
		_currentHealth = maxHealth;
		_currentCharge = maxCharge;
		_overchargeMana = 0f;
		_currentMana = maxMana;
	}

	public void SaveStats ()
	{
		_savedHealth = _currentHealth;
		_savedCharge = _currentCharge;
		//_savedMana = _currentMana;
	}
	public void ResetCombatStats(){
		_currentHealth = _savedHealth;
		_currentCharge = _savedCharge;
		//_currentMana = _savedMana;
	}
}
