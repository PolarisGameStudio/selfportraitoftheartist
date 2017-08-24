﻿using UnityEngine;
using System.Collections;

public class PlayerAugmentsS : MonoBehaviour {

	// script handles all upgrades from sub-weapons and virtues
	private PlayerController _playerReference;

	public const float ADAPTIVE_DAMAGE_BOOST = 1.5f;
	public const float ENRAGED_DAMAGE_BOOST = 1.75f;
	public const float CONDEMNED_TIME = 3f;
	public const float HATED_MULT = 1.5f;

	//________________________________________________________weapon augmentations
	private bool _lunaAug = false;
	public bool lunaAug { get { return _lunaAug; } }
	public const float lunaAugAmt = 1.5f;

	private bool _animaAug = false;
	public bool animaAug { get { return _animaAug; } }
	public const float animaAugAmt = 0.9f;

	private bool _solAug = false;
	public bool solAug { get { return _solAug; } }
	public const float solAugAmt = 1.5f;

	private bool _thanaAug = false;
	public bool thanaAug { get { return _thanaAug; } }
	public const float thanaAugAmt = 1.4f;

	private bool _gaeaAug = false;
	public bool gaeaAug { get { return _gaeaAug; } }
	public const float gaeaAugAmt = 0.75f;

	private bool _realGaeaAug = false;
	public bool realGaeaAug { get { return _realGaeaAug; } }
	public const float realGaeaAugAmt = 1.5f;

	private bool _erebosAug = false;
	public bool erebosAug { get { return _erebosAug; } }
	public const float erebosAugAmt = 2f;

	private bool _aeroAug = false;
	public bool aeroAug { get { return _aeroAug; } }

	//____________________________________________________________virtue augmentations
	// index 0
	private bool _unstoppableAug = false;
	public bool unstoppableAug { get { return _unstoppableAug; } }

	// NOT USED
	private bool _opportunisticAug = false; // VIRTUE NOT USED
	public bool opportunisticAug { get { return _opportunisticAug; } }

	// NOT USED
	private bool _dashAug = false; // VIRTUE NOT USED
	public bool dashAug { get { return _dashAug; } }

	// index 1 (extra killAt dmg)
	private bool _determinedAug = false;
	public bool determinedAug { get { return _determinedAug; } }

	// index 2 (lower stamina at max health
	private bool _empoweredAug = false;
	public bool empowered { get { return _empoweredAug; } }

	// index 3
	private bool _enragedAug = false;
	public bool enragedAug { get { return _enragedAug; } }

	// index 4
	private bool _adaptiveAug = false;
	public bool adaptiveAug { get { return _adaptiveAug; } }

	// index 5
	private bool _perceptiveAug = false;
	public bool perceptiveAug { get { return _perceptiveAug; } }
	
	// index 9 (stamina recharge on dodge)
	private bool _agileAug = false;
	public bool agileAug { get { return _agileAug; } }

	// index 7 (parry projectiles for massive damage)
	private bool _repellantAug = false;
	public bool repellantAug { get { return _repellantAug; } }
	
	// index 8 (stronger buddies)
	private bool _trustingAug = false;
	public bool trustingAug { get { return _trustingAug; } }

	// index 6 (charge refill on crit)
	private bool _drivenAug = false;
	public bool drivenAug { get { return _drivenAug; } }
	
	// index 10 (ambient charge refill during combat)
	private bool _anxiousAug = false;
	public bool anxiousAug { get { return _anxiousAug; } }

	// index 11 (stay alive for a bit after death)
	private bool _condemnedAug = false;
	public bool condemnedAug { get { return _condemnedAug; } }

	// index 13 (extra dmg everywhere)
	private bool _hatedAug = false;
	public bool hatedAug { get { return _hatedAug; } }

	// index 17 (witch time)
	private bool _untetheredAug = false;
	public bool untetheredAug { get { return _untetheredAug; } }

	// index 12 (bloodborne rally)
	private bool _desperateAug = false;
	public bool desperateAug { get { return _desperateAug; } }

	// index 14 (extra defense)
	private bool _lovedAug = false;
	public bool lovedAug { get { return _lovedAug; } }

	// index 20 (extra dmg per combo chain)
	private bool _paranoidAug = false;
	public bool paranoidAug { get { return _paranoidAug; } }

	// index 21 (decrease stats, increase VP)
	private bool _scornedAug = false;
	public bool scornedAug { get { return _scornedAug; } }

	private bool _initialized;

	// Update is called once per frame
	void Update () {

		if (_initialized){

		}
	
	}

	private void Initialize(){

		if (!_initialized){
			_initialized = true;
		}
		RefreshAll();

	}

	public void SetPlayerRef(PlayerController newRef){
		_playerReference = newRef;
		Initialize();
	}

	private void TurnOffAll(){

		// turn off weapon augs
		_lunaAug = false;
		_animaAug = false;
		_solAug = false;
		_gaeaAug = false;
		_aeroAug = false;
		_thanaAug = false;
		_realGaeaAug = false;
		_erebosAug = false;

		// turn off all virtue augs
		_opportunisticAug = false;
		_unstoppableAug = false;
		_dashAug = false;
		_enragedAug = false;
		_determinedAug = false;
		_empoweredAug = false;
		_adaptiveAug = false;
		_perceptiveAug = false;
		_agileAug = false;
		_repellantAug = false;
		_trustingAug = false;
		_drivenAug = false;
		_anxiousAug = false;
		_untetheredAug = false;
		_desperateAug = false;
		_lovedAug = false;
		_hatedAug = false;
		_paranoidAug = false;


	}

	public void RefreshAll(){
		TurnOffAll();

		// turn on weapon augs
		if (_playerReference.EquippedWeaponAug() != null){
			TurnOnWeaponAugs();
		}

		// turn on virtues
		if (PlayerController.equippedVirtues.Count > 0){
			TurnOnVirtueAugs();
		}
	}

	private void TurnOnWeaponAugs(){
		if (_playerReference.EquippedWeaponAug().weaponNum == 0){
			_lunaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 1){
			_thanaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 2){
			_aeroAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 3){
			_gaeaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 4){
			_animaAug = true;
		}
		
		if (_playerReference.EquippedWeaponAug().weaponNum == 5){
			_solAug = true;
		}
		if (_playerReference.EquippedWeaponAug().weaponNum == 6){
			_realGaeaAug = true;
		}
		if (_playerReference.EquippedWeaponAug().weaponNum == 7){
			_erebosAug = true;
		}
	}

	private void TurnOnVirtueAugs(){

		// as always, remove // before if statements when done testing
		if (PlayerController.equippedVirtues.Contains(0)){
			_unstoppableAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(1)){
			_determinedAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(2)){
			_empoweredAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(3)){
			_enragedAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(4)){
			_adaptiveAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(5)){
			_perceptiveAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(9)){
			_agileAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(7)){
			_repellantAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(8)){
			_trustingAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(6)){
			_drivenAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(10)){
			_anxiousAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(11)){
			_condemnedAug = true;
		}

		if (PlayerController.equippedVirtues.Contains(13)){
			_hatedAug = true;
		}

		if (PlayerController.equippedVirtues.Contains(17)){
		_untetheredAug = true;
		}
		if (PlayerController.equippedVirtues.Contains(14)){
			_lovedAug = true;
		}

		if (PlayerController.equippedVirtues.Contains(12)){
			_desperateAug = true;
		}

		if (PlayerController.equippedVirtues.Contains(20)){
			_paranoidAug = true;
		}

	}

	public float GetGaeaAug(){
		if (_realGaeaAug){
			return realGaeaAugAmt;
		}else{
			return 1f;
		}
	}
	public float GetErebosAug(){
		if (_erebosAug){
			return erebosAugAmt;
		}else{
			return 1f;
		}
	}

	public bool HasWitchAug(){
		if (_untetheredAug || _agileAug){
			return true;
		}else{
			return false;
		}
	}
}
