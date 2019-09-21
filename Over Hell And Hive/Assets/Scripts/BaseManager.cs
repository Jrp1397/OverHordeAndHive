﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseManager : MonoBehaviour
{
    public List<Character> Combatants;
    public int selectedChar = 0;
    public Attack TestAttack;
    public int ResourceGold = 0, ResourceConMat = 0, ResourceOre = 0, ResourceTomes = 0;
    public List<Armour> BaseArmorInventory;
    public List<Weapon> BaseWeaponInventory;
    [SerializeField] private Encounter ExplorationPhase;
    [SerializeField] private Text AddorRemovetextBox;

    // Start is called before the first frame update
    void Start()
    {
        TestAttack.ToHitValue = 11;
        TestAttack.PenValue = 2;
        TestAttack.ToCritModifier = 5;
        TestAttack.Damage = new Vector3(3, 2, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void GiveCharacterById(int incID)
    {
        Character temp = null;
        for (int i = Combatants.Count-1; i >= 0; i--)
        {
            if (Combatants[i].UniqueID == incID)
            {
                temp = Combatants[i];
                ExplorationPhase.TakeCharacter(temp);
                Combatants.RemoveAt(i);
                break;
            }
        }
    }

    public void TakeCharacter(Character incChar)
    {
        if (incChar == null) { return; }
        for (int i = Combatants.Count - 1; i >= 0; i--)//if a character is being updated, replace the old character
        {
            if (Combatants[i].UniqueID == incChar.UniqueID)
            {
                Combatants[i] = incChar;
                return;
            }
        }//otherwise, its a new version, add it to the list.

        Combatants.Add(incChar);

    }

    public void DisplayCharacters()
    {
        foreach (Character chara in Combatants)
        {
            chara.PingStrength();
        }
    }


    public void ToggleActiveCharacterExpedition()
    {
        for (int i = Combatants.Count - 1; i >= 0; i--)
        {
            if (Combatants[i].UniqueID == selectedChar)//if this is the selected Character
            {
                GiveCharacterById(selectedChar);
                AddorRemovetextBox.text = "Remove Unit From Expedition";
                return;
            }
        }
        ExplorationPhase.GiveCharacterById(selectedChar);
        AddorRemovetextBox.text = "Add Unit To Expedition";
    }


    public void SetSelectedCharacter(int incnumb)
    {//change the value, and then update any required UI elements.
        selectedChar = incnumb;
        bool isCharhere = false;

        for (int i = Combatants.Count - 1; i >= 0; i--)
        {
            if (Combatants[i].UniqueID == selectedChar)//if this is the selected Character
            {
                isCharhere = true;
            }
        }

        if (isCharhere)
        {
            AddorRemovetextBox.text = "Add Unit To Expedition";
        }
        else
        {
            AddorRemovetextBox.text = "Remove Unit From Expedition";
        }
    }

    public void AttackSelectedUnit()
    {
        for (int i = Combatants.Count - 1; i >= 0; i--)
        {
            if (Combatants[i].UniqueID == selectedChar)//if this is the selected Character
            {
                Combatants[i].ProcessAttack(TestAttack);
            }
        }
    }

    //takes item from unit, and adds it to bases list
    public void TakeItemFromSelectedUnit(bool isArmor)
    {
        if (isArmor)
        {
            if (Combatants[selectedChar].myArmor != null)
            {
                BaseArmorInventory.Add(Combatants[selectedChar].myArmor);
                Combatants[selectedChar].myArmor = null;
            }
        }
        else
        {
            if (Combatants[selectedChar].myWeapon != null)
            {
                BaseWeaponInventory.Add(Combatants[selectedChar].myWeapon);
                Combatants[selectedChar].myWeapon = null;
            }
        }
    }

    //takes item from bases list, and gives it to unit
    public void GiveItemToSelectedUnit(bool isArmor, int index)
    {
        if(isArmor && index < BaseArmorInventory.Count)
        {
            Combatants[selectedChar].myArmor = BaseArmorInventory[index];
            BaseArmorInventory.RemoveAt(index);
        }
        else if ( index < BaseWeaponInventory.Count)
        {
            Combatants[selectedChar].myWeapon = BaseWeaponInventory[index];
            BaseWeaponInventory.RemoveAt(index);
        }
    }


}
