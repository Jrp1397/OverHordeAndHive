using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Attack
{
    public int PenValue;
    public float ToHitValue, ToCritModifier;
    public Vector3 Damage;
}


public class Character : MonoBehaviour
{
    //3 main stats, weapon type and tier, armour type and teir, class type and tier
    public int UniqueID, Str, Wis, Cha, OffType, OffTier, DefType, DefTier, StanceType;
    public int MaxHealth, Health, Stamina, MP, MaxMP, Speed, Movement, Initiative;
    public string DisplayName;
    public Vector2Int MapPosition = new Vector2Int(-1, -1);

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

    public Attack GenerateAttack()
    {
        Attack outgoingAttack = new Attack(); 
        //                              Stances are Defensive, Nuetral, Offensive. Higher stances give a better result.  Stamina gives up to a 25% boost, and 25% penalty
        outgoingAttack.ToHitValue = ((float)Str * (((float)StanceType / 2.0f) + .5f) + (float)OffTier) * (.75f +(.5f * ((float)Stamina/MaxHealth)));
        outgoingAttack.ToCritModifier = 0 + OffTier + (1-OffType);
        float slashDMG = OffTier * (3 - OffType), pierceDMG = OffTier * (1 + (OffType % 2)), crushDMG;
        if(OffType != 2)
        {
            crushDMG = OffTier / 2;
        }
        else
        {
            crushDMG = OffTier * 1.5f;
        }

        outgoingAttack.Damage = new Vector3(slashDMG, pierceDMG, crushDMG);
        outgoingAttack.PenValue = OffTier + OffType;
        return outgoingAttack;
    }

    public float GenerateDefence()
    {
        float output = ((float)Str * ((1.5f - (float)StanceType / 2.0f) ) + (float)DefTier) * (.75f + (.5f * ((float)Stamina / MaxHealth)));
        Debug.Log(output);
        return output;
    }

    public void ProcessAttack( Attack IncAttack)
    {
        float DidHit = Random.Range(0.0f, 100.0f);
        Debug.Log("Random Chance =" + DidHit + "  Plus " + ((IncAttack.ToHitValue * 10) - (GenerateDefence() * 10)));
        DidHit += (IncAttack.ToHitValue * 10) - (GenerateDefence()*10);

        if(DidHit > 50)
        {//Successful Hit, calculate Damage
            float DidCrit = Random.Range(0.0f, 100.0f);
            DidCrit += (DefType + DefTier - 3) + IncAttack.ToCritModifier;
            if (DidCrit > 100)
            {
                float damagetotal = IncAttack.Damage.x + IncAttack.Damage.y + IncAttack.Damage.z;
                Debug.Log("Regular Hit, total Damage =  " + damagetotal);
            }
            else
            {
                float damagetotal = 0;
                //Damage totals       Slash damage is eaisest to reduce or remove,       pierce has an easier time penetrating, but blunt damage goes through the most
                float temp = ( 1+ (DefTier + DefType - IncAttack.PenValue));
                Debug.Log(temp);
                if (temp > 0) {
                    damagetotal +=( IncAttack.Damage.x / temp);
                }
                else
                {
                    damagetotal += IncAttack.Damage.x;
                }
                temp = (1+ ((DefTier / 2) + DefType - IncAttack.PenValue));
                Debug.Log(temp);
                if (temp > 0)
                {
                    damagetotal += (IncAttack.Damage.y / temp);
                }
                else
                {
                    damagetotal += IncAttack.Damage.y;
                }
                temp =(1+ (DefTier - IncAttack.PenValue));
                Debug.Log(temp);
                if (temp > 0)
                {
                    damagetotal += (IncAttack.Damage.z / temp);
                }
                else
                {
                    damagetotal += IncAttack.Damage.z;
                }
                Debug.Log("Regular Hit, total Damage =  " + damagetotal);
            }
        }
        else
        {
            Debug.Log("Hit FAILED");
        }
        

    }
    


}
