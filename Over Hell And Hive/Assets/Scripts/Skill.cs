using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public bool Ranged, unlocked;
    public int rangeModifier, toHitModifier, toCritModifier, cost, knockbackModifer;
    public Vector3Int damageModifier;
    public string Name, Desc;
    public Character myCharacter = null;
    public Sprite mySprite;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect()
    {//modify the character so that they attack with new values
        myCharacter.Str += toHitModifier;
        myCharacter.critMod += toCritModifier;
        myCharacter.DamageMod += damageModifier;
    }

    public void OnDeselect()
    {//Modify the character to reverse the changes of their old values.
        myCharacter.Str -= toHitModifier;
        myCharacter.critMod -= toCritModifier;
        myCharacter.DamageMod -= damageModifier;

    }
}
