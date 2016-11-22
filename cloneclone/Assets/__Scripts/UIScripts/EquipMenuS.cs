﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EquipMenuS : MonoBehaviour {

	public Image playerImage;
	public Text descriptionText;
	private PlayerController pRef;
	[Header("Text Properties")]
	public string paradigmIString;
	public string paradigmIIString;
	public string virtueString;
	public string inventoryString;

	private bool controlStickMoved = false;
	private bool selectButtonDown = false;
	private bool exitButtonDown = false;


	// loadout one
	public Image mantraMainParadigmI;
	public Image mantraSubParadigmI;
	public GameObject paradigmIMantraWhole;
	public Image buddyParadigmI;
	public Image buddyParadigmIOutline;

	// loadout subscreens
	public GameObject paradigmMantraSubscreen;
	public GameObject paradigmBuddySubscreen;

	// loadout two
	public Image mantraMainParadigmII;
	public Image mantraSubParadigmII;
	public GameObject paradigmIIMantraWhole;
	public Image buddyParadigmII;
	public Image buddyParadigmIIOutline;
	
	public GameObject virtueWhole;
	public GameObject virtueSubscreen;
	public GameObject inventoryWhole;
	public GameObject inventorySubscreen;

	public RectTransform selector;
	public RectTransform[] selectorPositions;
	public Image[] selectorElements;
	private int currentPos = 0;

	private float startElementAlpha;

	private bool onMainScreen = true;

	private bool _canBeQuit = false;
	public bool canBeQuit { get { return _canBeQuit; } }

	// PARADIGM ELEMENTS
	private bool inParadigmIMenu;
	private bool inParadigmIIMenu;
	private bool changingWeapon;
	private int currentWeaponSelected = 0;
	[Header("Paradigm Edit Components")]
	public EquipWeaponItemS[] allMantraItems;
	public EquipBuddyItemS[] allBuddyItems;
	public RectTransform[] selectorPositionsParadigmI;
	public Image[] selectorElementsParadigmI;
	public RectTransform[] selectorPositionsParadigmII;
	public Image[] selectorElementsParadigmII;
	public RectTransform[] selectorPositionsBuddy;
	public Image[] selectorElementsBuddy;

	// VIRTUE ELEMENTS
	private bool inVirtueMenu;
	private bool changingVirtue;
	private int currentVirtueSelected = 0;
	public EquipVirtueItemS[] allVirtueItems;
	public Image[] selectorElementsVirtues;
	public RectTransform[] selectorPositionsVirtues;
	private int virtueCapacity = 4;

	// INVENTORY ELEMENTS
	private bool inInventoryMenu;
	private bool changingInventory;
	private int currentInventorySelected = 0;
	public EquipInventoryItemS[] allInventoryItems;
	public Image[] selectorElementsInventory;
	public RectTransform[] selectorPositionsInventory;
	private int inventoryCapacity = 4;

	private bool _initialized = false;

	private PlayerInventoryS inventoryRef;


	public void TurnOn(){

		if (!_initialized){
			playerImage.enabled = false;
			mantraMainParadigmI.enabled = false;
			mantraSubParadigmI.enabled = false;
			buddyParadigmI.enabled = false;
			buddyParadigmII.enabled = false;
	
			inventoryRef = PlayerInventoryS.I;
			
			startElementAlpha = selectorElements[0].color.a;
			SetSelector(currentPos);
			_initialized = true;

			if (!pRef){
				pRef = GetComponentInParent<InGameMenuManagerS>().pRef;
				
				
				playerImage.color = pRef.myRenderer.color;
				playerImage.sprite = pRef.myRenderer.sprite;
				playerImage.enabled = true;

			}
		}

		descriptionText.text = "";
		SetSelector(0);
		UpdateMantraDisplay();
		UpdateBuddyDisplay();
		UpdateVirtueDisplay();
		UpdateMantras();
		UpdateBuddies();
		UpdateVirtues();
		gameObject.SetActive(true);
	}
	
	

	// Update is called once per frame
	void Update () {
		
		playerImage.color = pRef.myRenderer.color;
		playerImage.sprite = pRef.myRenderer.sprite;

		// MAIN MENU SECTION

		if (onMainScreen){

			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (targetPos > selectorPositions.Length-1){
						targetPos = 0;
					}
					SetSelector(targetPos);
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					controlStickMoved = true;
					int targetPos = currentPos-1;
					if (targetPos < 0){
						targetPos = selectorPositions.Length-1;
					}
					SetSelector(targetPos);
					controlStickMoved = true;
				}
			}

			if (pRef.myControl.MenuSelectButton() && !selectButtonDown){
				if (currentPos == 0){
					GoToParadigmISetUp();
				}
				if (currentPos == 1 && pRef.SubWeapon() != null){
					GoToParadigmIISetUp();
				}
				if (currentPos == 2 && inventoryRef.earnedVirtues.Count > 0){
					GoToVirtueSetUp();
				}
				if (currentPos == 3 && inventoryRef.collectedItems.Count > 0){
					GoToInventorySetUp();
				}
			}
		
			_canBeQuit = true;

		}

		//______________________________________________________START LOADOUT SECTION

		// EQUIP MANTRA PARADIGM I SECTION

		if (inParadigmIMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (!changingWeapon){
						if (targetPos > 2){
							targetPos = 0;
						}
						SetSelectorParadigmI(targetPos, 0);
					}else{
						if (targetPos > selectorPositionsParadigmI.Length-1){
							targetPos = 2;
						}
						SetSelectorParadigmI(currentPos, 0, 1);
					}
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					controlStickMoved = true;
					int targetPos = currentPos-1;
					if (!changingWeapon){
						if (targetPos < 0){
							targetPos = 2;
						}
						SetSelectorParadigmI(targetPos, 0);
					}else{
						if (targetPos < 2){
							targetPos = selectorPositionsParadigmI.Length-1;
						}
						SetSelectorParadigmI(currentPos, 0, -1);
					}
					controlStickMoved = true;
				}
			}

			// changing mantra function
			if (!changingWeapon){
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = true;
					selectButtonDown = true;
					currentWeaponSelected = currentPos;
					if (currentPos == 0){
						SetSelectorParadigmI(FindMantraPosition(pRef.EquippedWeapon().weaponNum), 0); // replace with mantra's pos
					}else if (currentPos == 1){
						// sub wep of main paradigm
						SetSelectorParadigmI(FindMantraPosition(pRef.EquippedWeaponAug().weaponNum), 0); // replace with mantra's pos
					}else{
						SetSelectorParadigmI(FindBuddyPosition(pRef.EquippedBuddy().buddyNum), 0);
					}
				}
			}
			else{
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = false;
					selectButtonDown = true;

					// swap actual mantra equip & update display
					if (currentWeaponSelected == 0){
						pRef.equippedWeapons[pRef.currentParadigm] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}else if (currentWeaponSelected == 1){
						pRef.subWeapons[pRef.currentParadigm] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}else{
						pRef.BuddyLoad(0, allBuddyItems[currentPos-3].buddyInstance);
						UpdateBuddyDisplay();
					}

					pRef.ParadigmCheck();

					UpdateMantraDisplay();
					// switch weapon positions (equip mantra)
					SetSelectorParadigmI(currentWeaponSelected, 0); // replace with swapped mantra's position

				}
				if (!exitButtonDown && pRef.myControl.ExitButton()){
					// exit out of mantra swap
					changingWeapon = false;
					exitButtonDown = true;
					SetSelectorParadigmI(currentWeaponSelected, 0); // replace with selected mantra's position
				}
			}

			if (!exitButtonDown && pRef.myControl.ExitButton()){
				SetSelector(0);
				paradigmMantraSubscreen.gameObject.SetActive(false);
				paradigmBuddySubscreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				inParadigmIMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
			}
			
			_canBeQuit = false;

		}

		// EQUIP MANTRA PARADIGM II SECTION
			
		if (inParadigmIIMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					controlStickMoved = true;
					int targetPos = currentPos+1;
					if (!changingWeapon){
						if (targetPos > 2){
							targetPos = 0;
						}
						SetSelectorParadigmII(targetPos);
					}else{
						if (targetPos > selectorPositionsParadigmII.Length-1){
							targetPos = 2;
						}
						SetSelectorParadigmII(currentPos, 1);
					}
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					controlStickMoved = true;
					int targetPos = currentPos-1;
					if (!changingWeapon){
						if (targetPos < 0){
							targetPos = 2;
						}
						SetSelectorParadigmII(targetPos);
					}else{
						if (targetPos < 2){
							targetPos = selectorPositionsParadigmII.Length-1;
						}
						SetSelectorParadigmII(currentPos, -1);
					}
					controlStickMoved = true;
				}
			}

			// changing mantra function
			if (!changingWeapon){
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = true;
					selectButtonDown = true;
					currentWeaponSelected = currentPos;
					if (currentPos == 0){
						SetSelectorParadigmII(FindMantraPosition(pRef.SubWeapon().weaponNum)); // replace with mantra's pos
					}else if (currentPos == 1){
						// sub wep of main paradigm
						SetSelectorParadigmII(FindMantraPosition(pRef.SubWeaponAug().weaponNum)); // replace with mantra's pos
					}else{
						SetSelectorParadigmII(FindBuddyPosition(pRef.SubBuddy().buddyNum));
					}
				}
			}
			else{
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingWeapon = false;
					selectButtonDown = true;
					// swap actual mantra equip & update display
					if (currentWeaponSelected == 0){
						pRef.equippedWeapons[pRef.subParadigm] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}else if (currentWeaponSelected == 1){
						pRef.subWeapons[pRef.subParadigm] = allMantraItems[currentPos-3].WeaponRefForSwitch();
					}
					else{
						pRef.BuddyLoad(1, allBuddyItems[currentPos-3].buddyInstance);
						UpdateBuddyDisplay();
					}


					UpdateMantraDisplay();
					// switch weapon positions (equip mantra)
					SetSelectorParadigmII(currentWeaponSelected); // replace with swapped mantra's position

				}
				if (!exitButtonDown && pRef.myControl.ExitButton()){
					// exit out of mantra swap
					changingWeapon = false;
					exitButtonDown = true;
					SetSelectorParadigmII(currentWeaponSelected); // replace with selected mantra's position
				}
			}
				
			if (!exitButtonDown && pRef.myControl.ExitButton()){
				SetSelector(1);
				paradigmMantraSubscreen.gameObject.SetActive(false);
				paradigmBuddySubscreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				inParadigmIIMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
			}

			_canBeQuit = false;
		}

		//_______________________________________________END LOADOUT SECTION

		//_______________________________________________START VIRTUE SECTION
		if (inVirtueMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					controlStickMoved = true;
					int targetPos = currentPos+1;
					SetSelectorVirtue(targetPos, 1);
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					controlStickMoved = true;
					int targetPos = currentPos-1;
					SetSelectorVirtue(targetPos, -1);
					controlStickMoved = true;
				}
			}
			// changing virtue function
			if (!changingVirtue){
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingVirtue = true;
					selectButtonDown = true;
					currentVirtueSelected = currentPos;
					if (pRef.equippedVirtues[currentVirtueSelected] > -1){
						SetSelectorVirtue(pRef.equippedVirtues[currentVirtueSelected]+virtueCapacity);
					}else{
						SetSelectorVirtue(FindNextAvailableVirtue(0,0));
					}
				}
			}
			else{
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingVirtue = false;
					selectButtonDown = true;

					// swap actual virtue equip & update display
					if (pRef.equippedVirtues.Contains(allVirtueItems[currentPos].virtueNum)){
						// unequip virtue if trying to swap with self
						if (pRef.equippedVirtues[currentVirtueSelected] == allVirtueItems[currentPos].virtueNum){
							pRef.equippedVirtues[currentVirtueSelected] =
								allVirtueItems[currentVirtueSelected].virtueNum = -1;
						}else{
							pRef.equippedVirtues[pRef.equippedVirtues.IndexOf(allVirtueItems[currentPos].virtueNum)] =
								allVirtueItems[pRef.equippedVirtues.IndexOf(allVirtueItems[currentPos].virtueNum)].virtueNum = -1;
							pRef.equippedVirtues[currentVirtueSelected] = 
								allVirtueItems[currentVirtueSelected].virtueNum = allVirtueItems[currentPos].virtueNum;
						}
					}else{

						pRef.equippedVirtues[currentVirtueSelected] = 
							allVirtueItems[currentVirtueSelected].virtueNum = allVirtueItems[currentPos].virtueNum;
					}

					
					
					UpdateVirtueDisplay();
					// switch virtue positions (equip virtue)
					SetSelectorVirtue(currentVirtueSelected);
					
				}
				if (!exitButtonDown && pRef.myControl.ExitButton()){
					// exit out of mantra swap
					changingVirtue = false;
					exitButtonDown = true;
					SetSelectorVirtue(currentVirtueSelected); // replace with selected mantra's position
				}
			}
			if (!exitButtonDown && pRef.myControl.ExitButton()){
				SetSelector(2);
				virtueSubscreen.gameObject.SetActive(false);
				inventoryWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				paradigmIMantraWhole.SetActive(true);
				paradigmIIMantraWhole.SetActive(true);
				inVirtueMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
			}
			
			_canBeQuit = false;
		}
		//_______________________________________________END VIRTUE SECTION

		//_______________________________________________START INVENTORY SECTION
		if (inInventoryMenu){
			if (!controlStickMoved){
				if (pRef.myControl.Horizontal() >= 0.1f || pRef.myControl.Vertical() <= -0.1f){
					controlStickMoved = true;
					int targetPos = currentPos+1;
					SetSelectorInventory(targetPos, 1);
					controlStickMoved = true;
				}
				if (pRef.myControl.Horizontal() <= -0.1f || pRef.myControl.Vertical() >= 0.1f){
					controlStickMoved = true;
					int targetPos = currentPos-1;
					SetSelectorInventory(targetPos, -1);
					controlStickMoved = true;
				}
			}
			// changing inventory function
			if (!changingInventory){
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingInventory = true;
					selectButtonDown = true;
					currentInventorySelected = currentPos;
					if (inventoryRef.iManager.equippedInventory[currentInventorySelected] > -1){
						SetSelectorInventory
							(inventoryRef.collectedItems.IndexOf(
								inventoryRef.iManager.equippedInventory[currentInventorySelected])+inventoryCapacity);
					}else{
						SetSelectorInventory(inventoryCapacity);
					}
				}
			}
			else{
				if (!selectButtonDown && pRef.myControl.MenuSelectButton()){
					changingInventory = false;
					selectButtonDown = true;
					
					// swap actual virtue equip & update display
					if (inventoryRef.iManager.equippedInventory.Contains(allInventoryItems[currentPos].itemNum)){
						// unequip virtue if trying to swap with self
						if (inventoryRef.iManager.equippedInventory[currentInventorySelected] 
						    == allInventoryItems[currentPos].itemNum){
							inventoryRef.iManager.equippedInventory[currentInventorySelected] =
								allInventoryItems[currentInventorySelected].itemNum = -1;
						}else{
							inventoryRef.iManager.equippedInventory[inventoryRef.iManager.equippedInventory.IndexOf(allInventoryItems[currentPos].itemNum)] =
								allInventoryItems[inventoryRef.iManager.equippedInventory.IndexOf(allInventoryItems[currentPos].itemNum)].itemNum = -1;
							inventoryRef.iManager.equippedInventory[currentInventorySelected] = 
								allInventoryItems[currentInventorySelected].itemNum = allInventoryItems[currentPos].itemNum;
						}
					}else{
						
						inventoryRef.iManager.equippedInventory[currentInventorySelected] = 
							allInventoryItems[currentInventorySelected].itemNum = allInventoryItems[currentPos].itemNum;
					}
					
					
					
					UpdateInventoryDisplay();
					// switch inventory positions (equip item)
					SetSelectorInventory(currentInventorySelected);
					
				}
				if (!exitButtonDown && pRef.myControl.ExitButton()){
					// exit out of mantra swap
					changingInventory = false;
					exitButtonDown = true;
					SetSelectorInventory(currentInventorySelected); // replace with selected item's position
				}
			}
			if (!exitButtonDown && pRef.myControl.ExitButton()){
				SetSelector(3);
				inventorySubscreen.gameObject.SetActive(false);
				virtueWhole.gameObject.SetActive(true);
				virtueWhole.gameObject.SetActive(true);
				paradigmIMantraWhole.SetActive(true);
				paradigmIIMantraWhole.SetActive(true);
				inVirtueMenu = false;
				onMainScreen = true;
				exitButtonDown = true;
			}
			
			_canBeQuit = false;
		}
		//_______________________________________________END INVENTORY SECTION

		if (pRef.myControl.MenuSelectUp()){
			selectButtonDown = false;
		}

		if (pRef.myControl.ExitButtonUp()){
			exitButtonDown = false;
			if (exitButtonDown){
				Debug.Log("Exit reset!");
			}
			if (onMainScreen){
				_canBeQuit = true;
			}else{
				_canBeQuit = false;
			}
		}

		if (Mathf.Abs(pRef.myControl.Horizontal()) < 0.1f && Mathf.Abs(pRef.myControl.Vertical()) < 0.1f){
			controlStickMoved = false;
		}

	}

	public void SetSelector(int newPos){

		Color changeCols = selectorElements[currentPos].color;
		changeCols.a = startElementAlpha;
		selectorElements[currentPos].color = changeCols;

		changeCols.a = 1f;
		selectorElements[newPos].color = changeCols;

		if (newPos == 0){
			descriptionText.text = paradigmIString;
		}
		if (newPos == 1){
			descriptionText.text = paradigmIIString;
		}
		
		if (newPos == 2){
			descriptionText.text = virtueString;
		}
		
		if (newPos == 3){
			descriptionText.text = inventoryString;
		}
		
		currentPos = newPos;
		selector.anchoredPosition = selectorPositions[currentPos].anchoredPosition;
	}

	public void SetSelectorVirtue(int newPos, int dir = 0){

		Color changeCols = selectorElementsVirtues[currentPos].color;
		changeCols.a = startElementAlpha;
		selectorElementsVirtues[currentPos].color = changeCols;

		int nextAvailable = newPos;
		if (changingVirtue){

			nextAvailable = FindNextAvailableVirtue(currentPos, dir);

		}else{
			if (nextAvailable >= virtueCapacity){
				nextAvailable = 0;
			}
			if (nextAvailable < 0){
				nextAvailable = virtueCapacity-1;
			}
		}

		currentPos = nextAvailable;
		changeCols.a = 1f;
		selectorElementsVirtues[nextAvailable].color = changeCols;
		selector.anchoredPosition = selectorPositionsVirtues[currentPos].anchoredPosition;
		if (allVirtueItems[currentPos].virtueNum > -1){
			if (!changingVirtue){
				descriptionText.text = "Virtue Slot 0" + (currentPos+1) + ": " +
					allVirtueItems[currentPos].virtueDescription;
			}else{
				descriptionText.text = allVirtueItems[currentPos].virtueDescription;
			}
		}else{
			descriptionText.text = "Virtue Slot 0" + (currentPos+1);
		}

	}

	public void SetSelectorInventory(int newPos, int dir = 0){
		
		Color changeCols = selectorElementsInventory[currentPos].color;
		changeCols.a = startElementAlpha;
		selectorElementsInventory[currentPos].color = changeCols;
		
		int nextAvailable = newPos;
		if (changingInventory){
			
			nextAvailable = newPos;
			if (nextAvailable-inventoryCapacity >= inventoryRef.collectedItems.Count){
				nextAvailable = inventoryCapacity; 
			}
			
		}else{
			if (nextAvailable >= inventoryCapacity){
				nextAvailable = 0;
			}
			if (nextAvailable < 0){
				nextAvailable = inventoryCapacity-1;
			}
		}
		
		currentPos = nextAvailable;
		changeCols.a = 1f;
		selectorElementsInventory[nextAvailable].color = changeCols;
		selector.anchoredPosition = selectorPositionsInventory[currentPos].anchoredPosition;
		if (allInventoryItems[currentPos].itemNum > -1){
			if (!changingInventory){
				descriptionText.text = "Inventory Slot 0" + (currentPos+1) + ": " +
					allInventoryItems[currentPos].GetItemDescription();
			}else{
				descriptionText.text = allInventoryItems[currentPos].GetItemDescription();
			}
		}else{
			descriptionText.text = "Inventory Slot 0" + (currentPos+1);
		}
		
	}

	public void SetSelectorParadigmI(int newPos, int paradigmNum, int dir = 0){

		Color changeCols = selectorElementsParadigmI[currentPos].color;
		changeCols.a = startElementAlpha;

		if (!paradigmBuddySubscreen.gameObject.activeSelf || currentPos <= 2){
			if (paradigmNum == 0){
				changeCols.a = startElementAlpha;
				selectorElementsParadigmI[currentPos].color = changeCols;
			}else{
				changeCols = selectorElementsParadigmII[currentPos].color;
				changeCols.a = startElementAlpha;
				selectorElementsParadigmII[currentPos].color = changeCols;
			}
		}
		else{
			changeCols.a = startElementAlpha;
			selectorElementsBuddy[currentPos-3].color = changeCols;
		}

		int nextAvailable = newPos;
		
		if (dir == 0 && newPos <= 2){
			if (newPos == 2){
				paradigmBuddySubscreen.gameObject.SetActive(true);
				paradigmMantraSubscreen.gameObject.SetActive(false);
				if (paradigmNum == 0){
					descriptionText.text = allBuddyItems[pRef.EquippedBuddy().buddyNum].buddyDescription;
				}else{
					descriptionText.text = allBuddyItems[pRef.SubBuddy().buddyNum].buddyDescription;
				}
			}else{
				paradigmBuddySubscreen.gameObject.SetActive(false);
				paradigmMantraSubscreen.gameObject.SetActive(true);
				if (paradigmNum == 0){
					if (newPos == 0){
						descriptionText.text = allMantraItems[pRef.EquippedWeapon().weaponNum].weaponDescriptionMain;
					}else{
						descriptionText.text = allMantraItems[pRef.EquippedWeaponAug().weaponNum].weaponDescriptionSub;
					}
				}else{
					if (newPos == 0){
						descriptionText.text = allMantraItems[pRef.SubWeapon().weaponNum].weaponDescriptionMain;
					}else{
						descriptionText.text = allMantraItems[pRef.SubWeaponAug().weaponNum].weaponDescriptionSub;
					}
				}
			}
		}
		else{
			if (newPos > 2 && dir != 0){
				if (!paradigmBuddySubscreen.gameObject.activeSelf){
					nextAvailable = FindNextAvailableMantra(newPos, dir);
				}else{
					nextAvailable = FindNextAvailableBuddy(newPos, dir);
				}
			}
		}

		currentPos = nextAvailable;
		changeCols.a = 1f;
		
		if (!paradigmBuddySubscreen.gameObject.activeSelf || currentPos <= 2){
			if (paradigmNum == 0){
				selectorElementsParadigmI[nextAvailable].color = changeCols;
				selector.anchoredPosition = selectorPositionsParadigmI[currentPos].anchoredPosition;
				if(currentPos > 2){

					if (currentWeaponSelected == 0){
						descriptionText.text = allMantraItems[nextAvailable-3].weaponDescriptionMain;
					}else{
						descriptionText.text = allMantraItems[nextAvailable-3].weaponDescriptionSub;
					}

				}
			}else{
				selectorElementsParadigmII[nextAvailable].color = changeCols;
				selector.anchoredPosition = selectorPositionsParadigmII[currentPos].anchoredPosition;
				if(currentPos > 2){

					if (currentWeaponSelected == 0){
						descriptionText.text = allMantraItems[nextAvailable-3].weaponDescriptionMain;
					}else{
						descriptionText.text = allMantraItems[nextAvailable-3].weaponDescriptionSub;
					}

				}
			}
		}else{
			selectorElementsBuddy[nextAvailable-3].color = changeCols;
			selector.anchoredPosition = selectorPositionsBuddy[currentPos-3].anchoredPosition;
			descriptionText.text = allBuddyItems[nextAvailable-3].buddyDescription;
		}

	}
	public void SetSelectorParadigmII(int newPos, int dir = 0){
		
		SetSelectorParadigmI(newPos, 1, dir);
		
	}

	public void TurnOff(){
		paradigmMantraSubscreen.gameObject.SetActive(false);
		paradigmBuddySubscreen.gameObject.SetActive(false);
		virtueSubscreen.gameObject.SetActive(false);
		inventorySubscreen.gameObject.SetActive(false);
		inventoryWhole.gameObject.SetActive(true);
		virtueWhole.gameObject.SetActive(true);
		paradigmIMantraWhole.gameObject.SetActive(true);
		paradigmIIMantraWhole.gameObject.SetActive(true);
		inParadigmIMenu = false;
		inParadigmIIMenu = false;
		inInventoryMenu = false;
		inVirtueMenu = false;
		exitButtonDown = true;
		currentPos = 0;
		onMainScreen = true;
		gameObject.SetActive(false);
	}

	private void UpdateMantras(){
		foreach (EquipWeaponItemS w in allMantraItems){
			w.Initialize(inventoryRef);
		}
	}
	private void UpdateBuddies(){
		foreach (EquipBuddyItemS b in allBuddyItems){
			b.Initialize(inventoryRef);
		}
	}
	private void UpdateVirtues(){
		foreach (EquipVirtueItemS v in allVirtueItems){
			v.Initialize(inventoryRef);
		}
	}
	private void UpdateInventory(){
		foreach (EquipInventoryItemS i in allInventoryItems){
			i.Initialize(inventoryRef);
		}
	}

	// TODO adjust so that we can get rid of the 3, just look at mantra/mantra/buddy of paradigm
	private int FindNextAvailableBuddy(int startPt, int dir = 1){

		int nextAvail = startPt-3;

		if (dir > 0){
			if (startPt+3 < allBuddyItems.Length){
				for (int i = startPt-3; i < allBuddyItems.Length; i++){
					if (allBuddyItems[i].unlocked && nextAvail == startPt-3){
						nextAvail = i;
					}
				}
			}
			if (nextAvail == startPt-3){
				for (int j = 0; j < startPt-3; j++){
					if (allBuddyItems[j].unlocked && nextAvail == startPt-3){
						nextAvail = j;
					}
				}
			}
		}else{
			if (startPt-3 > 0){
				for (int j = startPt-3; j >= 0; j--){
					if (allBuddyItems[j].unlocked && nextAvail == startPt-3){
						nextAvail = j;
					}
				}
			}
			if (nextAvail == startPt-3){
				for (int i = allBuddyItems.Length-1; i > startPt-3; i--){
					if (allBuddyItems[i].unlocked && nextAvail == startPt-3){
						nextAvail = i;
					}
				}
			}
		}

		return nextAvail+3;

	}

	private int FindNextAvailableMantra(int startPt, int dir = 1){
		
		int nextAvail = startPt-3;
		
		if (dir > 0){
			if (startPt+3 < allMantraItems.Length){
				for (int i = startPt-3; i < allMantraItems.Length; i++){
					if (allMantraItems[i].unlocked && nextAvail == startPt-3){
						nextAvail = i;
					}
				}
			}
			if (nextAvail == startPt-3){
				for (int j = 0; j < startPt-3; j++){
					if (allMantraItems[j].unlocked && nextAvail == startPt-3){
						nextAvail = j;
					}
				}
			}
		}else{
			if (startPt-3 > 0){
				for (int j = startPt-3; j >= 0; j--){
					if (allMantraItems[j].unlocked && nextAvail == startPt-3){
						nextAvail = j;
					}
				}
			}
			if (nextAvail == startPt-3){
				for (int i = allMantraItems.Length-1; i > startPt-3; i--){
					if (allMantraItems[i].unlocked && nextAvail == startPt-3){
						nextAvail = i;
					}
				}
			}
		}
		
		return nextAvail+3;
		
	}

	private int FindNextAvailableVirtue(int startPt, int dir){
		int nextAvail = startPt;
		if (dir > 0){
			if (startPt < allVirtueItems.Length){
				for (int i = startPt; i < allVirtueItems.Length; i++){
					if (allVirtueItems[i].unlocked  && nextAvail == startPt){
						nextAvail = i;
					}
				}
			}
			if (nextAvail == startPt){
				for (int j = virtueCapacity; j < startPt; j++){
					if (allVirtueItems[j].unlocked && nextAvail == startPt){
						nextAvail = j;
					}
				}
			}
		}else if (dir == 0){
			nextAvail = -1;
			for (int i = virtueCapacity; i < allVirtueItems.Length; i++){
				if (allVirtueItems[i].unlocked  && nextAvail <0){
					nextAvail = i;
				}
			}
		}else{
			if (startPt > virtueCapacity){
				for (int j = startPt; j >= virtueCapacity; j--){
					if (allVirtueItems[j].unlocked && nextAvail == startPt){
						nextAvail = j;
					}
				}
			}
			if (nextAvail == startPt){
				for (int i = allVirtueItems.Length-1; i > startPt; i--){
					if (allVirtueItems[i].unlocked && nextAvail == startPt){
						nextAvail = i;
					}
				}
			}
		}
		
		return nextAvail;
	}


	// FIND POSITION Functions
	private int FindMantraPosition(int wepNum){
		int returnNum = 0;
		int listCount = 0;
		foreach (EquipWeaponItemS w in allMantraItems){
			if (w.weaponNum == wepNum){
				returnNum =  listCount;
			}
			listCount++;
		}
		return returnNum+3;
	}

	private int FindBuddyPosition(int budNum){
		int returnNum = 0;
		int listCount = 0;
		foreach (EquipBuddyItemS b in allBuddyItems){
			if (b.buddyNum == budNum){
				returnNum =  listCount;
			}
			listCount++;
		}
		return returnNum+3;
	}

	private void UpdateMantraDisplay(){

		mantraMainParadigmI.color = pRef.EquippedWeapon().swapColor;
		mantraMainParadigmI.sprite = pRef.EquippedWeapon().swapSprite;
		mantraMainParadigmI.enabled = true;

		if (pRef.EquippedWeaponAug() != null){
			mantraSubParadigmI.color = pRef.EquippedWeaponAug().swapColor;
			mantraSubParadigmI.sprite = pRef.EquippedWeaponAug().swapSprite;
			mantraSubParadigmI.enabled = true;
		}else{
			mantraSubParadigmI.enabled = false;
		}

		if (pRef.SubWeapon() != null){
		mantraMainParadigmII.color = pRef.SubWeapon().swapColor;
		mantraMainParadigmII.sprite = pRef.SubWeapon().swapSprite;
		mantraMainParadigmII.enabled = true;
		}else{	
			mantraMainParadigmII.enabled = false;
		}

		if (pRef.SubWeaponAug() != null){
			mantraSubParadigmII.color = pRef.SubWeaponAug().swapColor;
			mantraSubParadigmII.sprite = pRef.SubWeaponAug().swapSprite;
			mantraSubParadigmII.enabled = true;
		}
		else{
			mantraSubParadigmII.enabled = false;
		}

		pRef.playerAug.RefreshAll();
	}

	private void UpdateBuddyDisplay(){
		buddyParadigmI.color = buddyParadigmIOutline.color = pRef.EquippedBuddy().shadowColor;
		buddyParadigmI.sprite = pRef.EquippedBuddy().buddyMenuSprite;
		buddyParadigmI.enabled = true;

		if (pRef.SubBuddy() != null){
			buddyParadigmII.color = buddyParadigmIIOutline.color = pRef.SubBuddy().shadowColor;
			buddyParadigmII.sprite = pRef.SubBuddy().buddyMenuSprite;
			buddyParadigmII.enabled = buddyParadigmIIOutline.enabled = true;
		}else{
			buddyParadigmII.enabled = buddyParadigmIIOutline.enabled = false;
		}
	}

	private void UpdateVirtueDisplay(){

		for (int i = 0; i < virtueCapacity; i++){

			if (allVirtueItems[i].virtueNum < 0){
				allVirtueItems[i].Hide();
			}else{
				allVirtueItems[i].virtueImage.sprite = 
					allVirtueItems[allVirtueItems[i].virtueNum+virtueCapacity].virtueImage.sprite;
				allVirtueItems[i].virtueDescription = 
					allVirtueItems[allVirtueItems[i].virtueNum+virtueCapacity].virtueDescription;
				allVirtueItems[i].Show();
			}
		} 
	}

	private void UpdateInventoryDisplay(){
		
		UpdateInventory();
	}

	private void GoToParadigmISetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inParadigmIMenu = true;
		onMainScreen = false;
		paradigmBuddySubscreen.gameObject.SetActive(false);
		paradigmMantraSubscreen.gameObject.SetActive(true);
		inventoryWhole.gameObject.SetActive(false);
		virtueWhole.gameObject.SetActive(false);
		SetSelectorParadigmI(0, 0);
	}

	private void GoToParadigmIISetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inParadigmIIMenu = true;
		onMainScreen = false;
		paradigmBuddySubscreen.gameObject.SetActive(false);
		paradigmMantraSubscreen.gameObject.SetActive(true);
		inventoryWhole.gameObject.SetActive(false);
		virtueWhole.gameObject.SetActive(false);
		SetSelectorParadigmII(0);
	}

	private void GoToVirtueSetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inVirtueMenu = true;
		onMainScreen = false;
		paradigmBuddySubscreen.gameObject.SetActive(false);
		paradigmIMantraWhole.gameObject.SetActive(false);
		paradigmIIMantraWhole.gameObject.SetActive(false);
		paradigmMantraSubscreen.gameObject.SetActive(false);
		inventoryWhole.gameObject.SetActive(false);
		inventorySubscreen.gameObject.SetActive(false);
		virtueWhole.gameObject.SetActive(true);
		virtueSubscreen.gameObject.SetActive(true);
		currentVirtueSelected = 0;
		changingVirtue = false;
		SetSelectorVirtue(0);
	}
	private void GoToInventorySetUp(){
		currentPos = 0;
		selectButtonDown = true;
		inInventoryMenu = true;
		onMainScreen = false;
		paradigmBuddySubscreen.gameObject.SetActive(false);
		paradigmIMantraWhole.gameObject.SetActive(false);
		paradigmIIMantraWhole.gameObject.SetActive(false);
		paradigmMantraSubscreen.gameObject.SetActive(false);
		inventoryWhole.gameObject.SetActive(true);
		inventorySubscreen.gameObject.SetActive(true);
		virtueWhole.gameObject.SetActive(false);
		virtueSubscreen.gameObject.SetActive(false);
		currentInventorySelected = 0;
		changingInventory = false;
		SetSelectorInventory(0);
	}


}
