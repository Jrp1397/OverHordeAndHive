using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Attack
{
    public int PenValue;
    public float ToHitValue, ToCritModifier;
    public Vector3 Damage;
}

public enum Worktype { CombatFight =0, CombatSupport =1, QuarryStone=2, QuarryOre=3, TradeGold=4, TradeRecuit=5, ForgeArms=6, ForgeArmor=7, TowerSpells=9, TowerSkills=10}


public class Character : MonoBehaviour
{
    //3 main stats, weapon type and tier, armour type and teir, class type and tier
    public int UniqueID, Str, Wis, Cha, StanceType;
    public int MaxHealth, Health, Stamina, MP, MaxMP, Speed, Movement, Initiative;
    public string DisplayName;
    public Sprite mySprite;
    public Vector2Int MapPosition = new Vector2Int(-1, -1);
    public Worktype myWork;
    public Armour myArmor= null;
    public Weapon myWeapon = null;
    
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
        int OffTier = myWeapon.Tier;
        Attack outgoingAttack = new Attack(); 
        //                              Stances are Defensive, Nuetral, Offensive. Higher stances give a better result.  Stamina gives up to a 25% boost, and 25% penalty
        outgoingAttack.ToHitValue = ((float)Str * (((float)StanceType / 2.0f) + .5f) + (float)OffTier) * (.75f +(.5f * ((float)Stamina/MaxHealth)));
        outgoingAttack.ToCritModifier = OffTier * (myWeapon.CritMultiplier);
        float slashDMG =myWeapon.SlashOffence, pierceDMG = myWeapon.PierceOffence, crushDMG = myWeapon.CrushOffence;
     

        outgoingAttack.Damage = new Vector3(slashDMG, pierceDMG, crushDMG);
        outgoingAttack.PenValue = OffTier + myWeapon.CritMultiplier;
        return outgoingAttack;
    }

    public float GenerateDefence()
    {
        float output = ((float)Str * ((1.5f - (float)StanceType / 2.0f) ) + (float)myArmor.Tier) * (.75f + (.5f * ((float)Stamina / MaxHealth)));
        Debug.Log(output);
        return output;
    }

    public void ProcessAttack( Attack IncAttack)
    {
        int DefTier = myArmor.Tier;
        float DidHit = Random.Range(0.0f, 100.0f);
        Debug.Log("Random Chance =" + DidHit + "  Plus " + ((IncAttack.ToHitValue * 10) - (GenerateDefence() * 10)));
        DidHit += (IncAttack.ToHitValue * 10) - (GenerateDefence()*10);

        if(DidHit > 50)
        {//Successful Hit, calculate Damage
            float DidCrit = Random.Range(0.0f, 100.0f);
            DidCrit +=   IncAttack.ToCritModifier - (myArmor.CritDefence + DefTier);
            Debug.Log("Crit level=" + DidCrit);
            if (DidCrit > 100)
            {
                float damagetotal = IncAttack.Damage.x + IncAttack.Damage.y + IncAttack.Damage.z;
                Debug.Log("Regular Hit, total Damage =  " + damagetotal);
            }
            else
            {
                float damagetotal = 0;
                //Damage totals       Slash damage is eaisest to reduce or remove,       pierce has an easier time penetrating, but blunt damage goes through the most
                float temp = ( 1+ (myArmor.SlashDefence - IncAttack.PenValue));
                Debug.Log(temp);
                //See if the armor is completely penetrated or not, if it is, do full damage(like with a crit), otherwise, reduce the total damage
                if (temp > 0) {
                    damagetotal +=( IncAttack.Damage.x / temp);
                }
                else
                {
                    damagetotal += IncAttack.Damage.x;
                }
                temp = (1+  myArmor.PierceDefence - IncAttack.PenValue);
                Debug.Log(temp);
                if (temp > 0)
                {
                    damagetotal += (IncAttack.Damage.y / temp);
                }
                else
                {
                    damagetotal += IncAttack.Damage.y;
                }
                temp =(1+ (myArmor.CrushDefence- IncAttack.PenValue));
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
