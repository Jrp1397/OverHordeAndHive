using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseManager : MonoBehaviour
{
    public List<Character> Combatants;
    [SerializeField] private Encounter ExplorationPhase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CopyCharacterById(int incID)
    {
        Character temp = null;
        for(int i = Combatants.Count - 1; i >= 0; i--)
        {
            if(Combatants[i].UniqueID == incID)
            {
                temp = Combatants[i];
                break;
            }
        }
        ExplorationPhase.TakeCharacter(temp);
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
}
