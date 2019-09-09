using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseManager : MonoBehaviour
{
    public List<Character> Combatants;
    public int selectedChar = 0;
    [SerializeField] private Encounter ExplorationPhase;
    [SerializeField] private Text AddorRemovetextBox;

    // Start is called before the first frame update
    void Start()
    {
        
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


}
