using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    //3 main stats, weapon type and tier, armour type and teir, class type and tier
    public Encounter myEncounter;
    public int UniqueID, Str, Wis, Cha, OffType, OffTier, DefType, DefTier, SkillType, SkillTier, StanceType;
    public int Health, Stamina, MP, MaxHealth, Movement, Initiative, Speed, MaxRangeOpportunity=1;
    public string Title;
    public Vector2Int MapPosition = new Vector2Int(-1, -1);
    public Sprite mySprite;
    public Armour myArmor = null;
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
        Debug.Log("str of " + UniqueID + " = " + Str);

    }

    public Attack GenerateAttack()
    {
        int OffTier = myWeapon.Tier;
        Attack outgoingAttack = new Attack();
        //                              Stances are Defensive, Nuetral, Offensive. Higher stances give a better result.  Stamina gives up to a 25% boost, and 25% penalty
        outgoingAttack.ToHitValue = ((float)Str * (((float)StanceType / 2.0f) + .5f) + (float)OffTier) * (.75f + (.5f * ((float)Stamina / MaxHealth)));
        outgoingAttack.ToCritModifier = OffTier * (myWeapon.CritMultiplier);
        float slashDMG = myWeapon.SlashOffence, pierceDMG = myWeapon.PierceOffence, crushDMG = myWeapon.CrushOffence;


        outgoingAttack.Damage = new Vector3(slashDMG, pierceDMG, crushDMG);
        outgoingAttack.PenValue = OffTier + myWeapon.APvalue;
        outgoingAttack.KnockbackValue = 0;
        outgoingAttack.SourcePos = MapPosition;
        return outgoingAttack;
    }

    public float GenerateDefence()
    {
        float output = ((float)Str * ((1.5f - (float)StanceType / 2.0f)) + (float)myArmor.Tier) * (.75f + (.5f * ((float)Stamina / MaxHealth)));
        Debug.Log(output);
        return output;
    }

    public void ProcessAttack(Attack IncAttack)
    {
        int DefTier = myArmor.Tier;
        IncAttack.PenValue -= DefTier;
        float DidHit = Random.Range(0.0f, 100.0f);
        Debug.Log("Random Chance =" + DidHit + "  Plus " + ((IncAttack.ToHitValue * 10) - (GenerateDefence() * 10)));
        DidHit += (IncAttack.ToHitValue * 10) - (GenerateDefence() * 10);

        if (DidHit > 50)
        {//Successful Hit, calculate Damage
            float DidCrit = Random.Range(0.0f, 100.0f);
            DidCrit += IncAttack.ToCritModifier - (myArmor.CritDefence + DefTier);
            Debug.Log("Crit level=" + DidCrit);
            if (DidCrit > 100)
            {//Crit, do full raw/damage
                float damagetotal = IncAttack.Damage.x * IncAttack.Damage.x + IncAttack.Damage.y * IncAttack.Damage.y + IncAttack.Damage.z* IncAttack.Damage.z;
                Debug.Log("Crit  Hit, total Damage =  " + damagetotal);
                Health -= (int)damagetotal;
            }
            else
            {
                float damagetotal = 0;
                //Damage totals       Slash damage is eaisest to reduce or remove,       pierce has an easier time penetrating, but blunt damage goes through the most
                    float temp = (1 + (myArmor.SlashDefence - IncAttack.PenValue));
                if (IncAttack.Damage.x > 0)
                {//Ignore negative damage values
                    //See if the armor is completely penetrated or not, if it is, do full damage(like with a crit), otherwise, reduce the total damage
                    if (temp > 0)
                    {
                        damagetotal += ((IncAttack.Damage.x * IncAttack.Damage.x) / (2 * temp));
                    }
                    else
                    {
                        damagetotal += IncAttack.Damage.x * IncAttack.Damage.x;
                    }
                }
                temp = (1 + myArmor.PierceDefence - IncAttack.PenValue);

                if (IncAttack.Damage.y > 0)
                {//Ignore negative damage values
                    if (temp > 0)
                    {
                        damagetotal += ((IncAttack.Damage.y * IncAttack.Damage.y) / (2 * temp));
                    }
                    else
                    {
                        damagetotal += IncAttack.Damage.y * IncAttack.Damage.y;
                    }

                }

                if (IncAttack.Damage.z > 0)
                {//Ignore negative damage values
                    temp = (1 + (myArmor.CrushDefence - IncAttack.PenValue));
                    if (temp > 0)
                    {
                        damagetotal += ((IncAttack.Damage.z * IncAttack.Damage.z) / (2 * temp));
                    }
                    else
                    {
                        damagetotal += IncAttack.Damage.z * IncAttack.Damage.z;
                    }
                }

                if(IncAttack.KnockbackValue > 0)
                {
                    Vector2 relativeDistance = MapPosition - IncAttack.SourcePos;
                    int i = IncAttack.KnockbackValue;
                    
                    while (i > 0)
                    {
                        if(Mathf.Abs(relativeDistance.x) > Mathf.Abs(relativeDistance.y))
                        {//bigger x difference than y
                            Vector2Int newtemp = MapPosition;
                            newtemp.x += (int)(relativeDistance.x / Mathf.Abs(relativeDistance.x));
                            if(newtemp.x >0 && newtemp.x <4 && (myEncounter.BattleFieldTiles[newtemp.x, newtemp.y].myTileState == TileState.Empty || myEncounter.BattleFieldTiles[newtemp.x, newtemp.y].myTileState == TileState.Threatened || myEncounter.BattleFieldTiles[newtemp.x, newtemp.y].myTileState == TileState.InRange))
                            {
                                myEncounter.BattleFieldTiles[MapPosition.x, MapPosition.y].ChangeState(0);
                                MapPosition = newtemp;
                                myEncounter.BattleFieldTiles[MapPosition.x, MapPosition.y].ChangeState(3);
                            }
                            else
                            {
                                damagetotal += 1;
                            }
                        }
                        else
                        {
                            Vector2Int newtemp = MapPosition;
                            newtemp.y += (int)(relativeDistance.y / Mathf.Abs(relativeDistance.y));
                            if (newtemp.y > 0 && newtemp.y < 12 && (myEncounter.BattleFieldTiles[newtemp.x, newtemp.y].myTileState == TileState.Empty || myEncounter.BattleFieldTiles[newtemp.x, newtemp.y].myTileState == TileState.Threatened  || myEncounter.BattleFieldTiles[newtemp.x, newtemp.y].myTileState == TileState.InRange))
                            {
                                myEncounter.BattleFieldTiles[MapPosition.x, MapPosition.y].ChangeState(0);
                                MapPosition = newtemp;
                                myEncounter.BattleFieldTiles[MapPosition.x, MapPosition.y].ChangeState(3);
                            }
                            else
                            {
                                damagetotal += 1;
                            }
                        }

                        i--;
                    }


                }

                Debug.Log("Regular Hit, total Damage =  " + damagetotal);
                
                Health -= (int)damagetotal;
            }
        }
        else
        {
            Debug.Log("Hit FAILED");
        }


        if(Health <= 0)
        {
            myEncounter.OnEntityDeath(MapPosition, false);
        }

    }

    public void AssignRandomStats(int Points)
    {
        int HealthAssigned = 0, SpeedAssigned = 1;
        Points -= 1;

        int Ratio = 2 + Random.Range(0, 4);
        SpeedAssigned += (Points / Ratio);
        Points -= (Points / Ratio); ;
        HealthAssigned = Points;

        MaxHealth = HealthAssigned * 3;
        Health = MaxHealth;
        Speed = SpeedAssigned;

    }


}
