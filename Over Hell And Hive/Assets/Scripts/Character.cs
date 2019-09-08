using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //3 main stats, weapon type and tier, armour type and teir, class type and tier
    public int UniqueID, Str, Wis, Cha, OffType, OffTier, DefType, DefTier, SkillType, SkillTier;
    public int Health, Stamina, MP;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AlterStats()
    {
        Str += 5;
    }

    public void PingStrength()
    {
        Debug.Log("str of " +UniqueID +" = " + Str);

    }
}
