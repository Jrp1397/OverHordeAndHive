using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    //3 main stats, weapon type and tier, armour type and teir, class type and tier
    public int UniqueID, Str, Wis, Cha, OffType, OffTier, DefType, DefTier, StanceType;
    public int MaxHealth, Health, Stamina, MP, MaxMP, Speed;
    public string DisplayName;

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

    public float GenerateAttack()
    {
        float output = ((float)Str * (((float)StanceType / 2.0f) + .5f) + (float)OffTier) * (.75f +(.5f * ((float)Stamina/MaxHealth)));
        Debug.Log(output);
        return output;
    }

    public float GenerateDefence()
    {
        float output = ((float)Str * ((1.5f - (float)StanceType / 2.0f) ) + (float)DefTier) * (.75f + (.5f * ((float)Stamina / MaxHealth)));
        Debug.Log(output);
        return output;
    }
}
