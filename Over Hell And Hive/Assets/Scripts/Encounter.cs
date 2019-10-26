using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EncounterPhase { TextDecision, Deployment, CombatLoop }


public class Encounter : MonoBehaviour
{
    public EncounterPhase myECPhase = EncounterPhase.Deployment;
    public List<Character> Friends;
    public List<Monster> Foes;
    public List<string> Initative;
    public List<Weapon> WeaponLoot;
    public List<Armour> ArmourLoot;

    public List<Weapon> WeaponTypes;
    public List<Armour> ArmourTypes;
    public int ResourceGold = 0, ResourceConMat = 0, ResourceOre = 0, ThreatLevelMax = 10, ThreatLevelMin =3;
    
    [SerializeField] private BaseManager HomeBase;
    public Image FriendlyIcon;
    public Image EnemyIcon;
    public GameObject FieldTilePrefab, EmptyMonsterPrefab;
    private GameObject[,] BattleFieldObject = new GameObject [4,12];
    public CombatTile[,] BattleFieldTiles = new CombatTile[4, 12];
    public GameObject[] UIStatBars;
    public GameObject[] UIStatBarTexts;
    private int[] endedRounds = { 0, 0 };
    public int SelectedFoeIndex = 0, SelectedFriendIndex = 0;
    public int TurnFoeIndex = 0, TurnFriendIndex = 0;
    [SerializeField] private List<int> Seed;
    public int DangerSeedModifer;
    [SerializeField] private Text FoeName, FriendName;
    public SceneSwitcher ReturnsToMainBase;
    public Attack TestAttack;
    public GameObject SelectedCharacterPrefab, SelectedTilePrefab;
    bool CharactersFirst = true, PlayerNext = true, autotick = false;


    // Start is called before the first frame update
    void Start()
    {
        GenerateField();
        foreach (GameObject obby in BattleFieldObject)
        {
            obby.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (autotick)
        {
            autotick = false;
            CombatTick();
        }
    }

    public Character AccessCharacterById(int incID)
    {
        Character temp = null;
        for (int i = Friends.Count - 1; i >= 0; i--)
        {
            if (Friends[i].UniqueID == incID)
            {
                temp = Friends[i];
                break;
            }
        }
        return temp;
    }

    public void SelectTileOnRightClick(Vector2Int incMapPosition)
    {
        SelectedTilePrefab.transform.SetParent(BattleFieldObject[incMapPosition.x, incMapPosition.y].transform, false);

    }

    public void OnEnemyTileClick(Vector2Int incMapPosition)
    {
        int i = 0;
        foreach(Monster Mob in Foes)
        {
            if(Mob.MapPosition == incMapPosition)
            {
                SelectedFoeIndex = i;
                TickUIElements();
            }
            i++;
        }
    }

    public void GiveCharacterById(int incID)
    {
        Character temp = null;
        for (int i = Friends.Count - 1; i >= 0; i--)
        {
            if (Friends[i].UniqueID == incID)
            {
                temp = Friends[i];
                Friends.RemoveAt(i);
                break;
            }
        }
        HomeBase.TakeCharacter(temp);
    }

    public void TakeCharacter(Character incChar)
    {
        incChar.MapPosition = new Vector2Int(-1, -1);
        if (incChar == null) { return; }
        for (int i = Friends.Count - 1; i >= 0; i--)//if a character is being updated, replace the old character
        {
            if (Friends[i].UniqueID == incChar.UniqueID)
            {
                Friends[i] = incChar;
                return;
            }
        }//otherwise, its a new version, add it to the list.

        Friends.Add(incChar);

    }

    public void DisplayCharacters()
    {
        foreach (Character chara in Friends)
        {
            chara.PingStrength();
        }
    }

   void OnDisable()
    {
        foreach (GameObject obby in BattleFieldObject)
        {
            obby.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (BattleFieldObject[0,0] == null) { return; }
        foreach (GameObject obby in BattleFieldObject)
        {
           
            obby.SetActive(true);
        }

        StartDeployment(4, 4);
    }

    public void EnableMap()
    {
        foreach (GameObject obby in BattleFieldObject)
        {
            obby.SetActive(true);
        }
        GenerateCombatEncounter();
        StartDeployment(4,4);
    }

    public void GenerateField()
    {
        Vector3 tempPos = new Vector3(-6.275f, -1.05f, 0);
        for (int i = 0; i < 4; i++)
        {
            tempPos.x = -5.775f;
            for (int j = 0; j < 12; j++)
            {
                BattleFieldObject[i,j] = Instantiate(FieldTilePrefab, tempPos, Quaternion.identity, gameObject.transform);
                BattleFieldTiles[i, j] = BattleFieldObject[i, j].GetComponent<CombatTile>();
                BattleFieldTiles[i, j].MapPosition = new Vector2Int(i, j);
                BattleFieldTiles[i, j].myEncounter = this;
               tempPos.x += 1.05f;
            }
            tempPos.y += 1.05f;
        }
    }

    public void CycleSelFoe(bool Forward)
    {
        if (Forward)
        {
            SelectedFoeIndex++;
            if (SelectedFoeIndex > Foes.Count-1) { SelectedFoeIndex = 0; }
        }
        else
        {
            SelectedFoeIndex--;
            if (SelectedFoeIndex < 0) { SelectedFoeIndex = Foes.Count-1; }
        }

        TickUIElements();
    }

    public void CycleSelFriend(bool Forward)
    {
        if (Forward)
        {
            SelectedFriendIndex++;
            if (SelectedFriendIndex > Friends.Count-1) { SelectedFriendIndex = 0; }
        }
        else
        {
            SelectedFriendIndex--;
            if (SelectedFriendIndex < 0) { SelectedFriendIndex = Friends.Count-1; }
        }
        TickUIElements();
    }

    public void StartDeployment(int FriendlyDeployspace, int EnemyDeployspace)
    {
        for(int i =0; i < 4; i++)
        {
            for (int j = 0; j < FriendlyDeployspace; j++)
            {
                if (BattleFieldTiles[i, j].myTileState == TileState.Empty)
                {
                    BattleFieldTiles[i, j].ChangeState(4);//Deployable
                }
            }
        }

        foreach(Monster mob in Foes)
        {
            while (mob.MapPosition.x == -1)
            {
                int yValue = (int)Random.Range(0, 4);
                int xValue = (int)Random.Range(12-EnemyDeployspace, 12);
                if (BattleFieldTiles[yValue, xValue].myTileState == TileState.Empty)
                {
                    BattleFieldTiles[yValue, xValue].ChangeState(3);//Deployable
                    mob.MapPosition = new Vector2Int(yValue, xValue);
                }
            }
            
        }




    }

    public void EndDeployment()
    {
        foreach(Character chara in Friends)
        {
            if(chara.MapPosition.x < 0)
            {
                return;
            }
        }
        //run the controls to start standard combat.

        foreach(CombatTile comtil in BattleFieldTiles)
        {
            if(comtil.myTileState == TileState.Deployable)
            {
                comtil.ChangeState(0);
            }
        }

        RollInitiative();
        
        CombatTick();
        CalculatePlayerMovement();


    }

    public void MoveSelectedCharacterTo(Vector2Int incLoc,int Distance)
    {
        Friends[SelectedFriendIndex].MapPosition = incLoc;
        Friends[SelectedFriendIndex].Movement -= Distance;
    }

    private void RollInitiative()
    {
        int maxSpeed = 0;
       
        foreach(Character chara in Friends)
        {
            chara.Initiative = (int)Random.Range(0, 20) + chara.Speed;
            if(chara.Initiative > maxSpeed) { maxSpeed = chara.Initiative; }
        }
        
        foreach(Monster mob in Foes)
        {
            mob.Initiative = (int)Random.Range(0, 20) + mob.Speed;
            if (mob.Initiative > maxSpeed) { maxSpeed = mob.Initiative; CharactersFirst = false; PlayerNext = false; }
        }

        Friends.Sort((p2, p1) => p1.Initiative.CompareTo(p2.Initiative));
        Foes.Sort((p2, p1) => p1.Initiative.CompareTo(p2.Initiative));
    }
    
    public void CombatTick()
    {
        bool MonstersNow = false;
       // AfterMapMovement();
        if (PlayerNext)
        {//Player Controls
           
           

        }
        else
        {//Monster turn
            MonsterTurn();
         //Debug.Log(Foes[TurnFoeIndex].Title + "'s Turn");
            TickUIElements();
            SelectedCharacterPrefab.transform.SetParent(BattleFieldObject[Foes[TurnFoeIndex].MapPosition.x, Foes[TurnFoeIndex].MapPosition.y].transform, false);
         

            TurnFoeIndex++;
            if(TurnFoeIndex > Foes.Count - 1)
            {
                endedRounds[1] = 1;
                TurnFoeIndex = 0;
            }
        }
        
        if(endedRounds[0] ==0 || endedRounds[1]==0)
        {
            //if the monsters aren't going, or if the selected player has a greater initative than the monsters, then players go.
            if((Friends[SelectedFriendIndex].Initiative >= Foes[TurnFoeIndex].Initiative && endedRounds[0]==0) || endedRounds[1] == 1)
            {//Players go next
                PlayerNext = true;
                TickUIElements();
                SelectedCharacterPrefab.transform.SetParent(BattleFieldObject[Friends[SelectedFriendIndex].MapPosition.x, Friends[SelectedFriendIndex].MapPosition.y].transform, false);
                Friends[TurnFriendIndex].Movement = Friends[TurnFriendIndex].Speed;
            SelectedFriendIndex = TurnFriendIndex;
            TickUIElements();
            SelectedCharacterPrefab.transform.SetParent(BattleFieldObject[Friends[SelectedFriendIndex].MapPosition.x, Friends[SelectedFriendIndex].MapPosition.y].transform, false);
          
            CalculatePlayerMovement();
                TurnFriendIndex++;
                if (TurnFriendIndex > Friends.Count - 1)
                {
                    PlayerNext = false;
                    endedRounds[0] = 1;
                }
            }
            else
            {
                MonstersNow = true;
                FoeName.text = Foes[TurnFoeIndex].Title;
                PlayerNext = false;
            }
        }
        else
        {//round restarts.
            endedRounds[0] = 0;
            endedRounds[1] = 0;
            TurnFoeIndex = 0;
            TurnFriendIndex = 0;
            PlayerNext = CharactersFirst;
                   
        }
        if (MonstersNow)
        {
            CombatTick();
        }
    }

    public void CalculatePlayerRange()
    {
        Vector2Int selectedPosition = Friends[SelectedFriendIndex].MapPosition;
        int maxValue = Friends[SelectedFriendIndex].myWeapon.Reach + Friends[SelectedFriendIndex].AvailableSkills[Friends[SelectedFriendIndex].SelectedSkill].rangeModifier;
        Vector2Int tempPos;
        int Xvalue = maxValue;
        int Yvalue = 0;
        AfterMapMovement();

        while (Xvalue >= -maxValue)
        {
            Yvalue = 0;
            do
            {
                tempPos = new Vector2Int(selectedPosition.x + Xvalue, selectedPosition.y + Yvalue);

                if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                {
                    BattleFieldTiles[tempPos.x, tempPos.y].ChangeToAttackable();
                }

                tempPos = new Vector2Int(selectedPosition.x + Xvalue, selectedPosition.y - Yvalue);
                if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                {
                    BattleFieldTiles[tempPos.x, tempPos.y].ChangeToAttackable();
                }

                Yvalue++;

            } while ((Mathf.Abs(Xvalue) + Yvalue) <= maxValue);


            Xvalue--;
        }
    }

    public void CalculatePlayerMovement()
    {
        //Propagate across movement range empty tiles to moveable, enemy tiles to Attackable
        //Get position of selected character
        //Get their movement left
        //Start loop that paints in all directions, checking for valid states of the tiles
        
        Vector2Int selectedPosition = Friends[SelectedFriendIndex].MapPosition;
        int maxValue = Friends[SelectedFriendIndex].Movement; 
        Vector2Int tempPos;
        int Xvalue = maxValue;
        int Yvalue = 0;
        AfterMapMovement();
      
        while (Xvalue >= -maxValue)
        {
            Yvalue = 0;
            do
            {
                tempPos = new Vector2Int(selectedPosition.x + Xvalue, selectedPosition.y + Yvalue);

                if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                {
                    BattleFieldTiles[tempPos.x, tempPos.y].ChangeToMovable();
                }

                tempPos = new Vector2Int(selectedPosition.x + Xvalue, selectedPosition.y - Yvalue);
                if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                {
                    BattleFieldTiles[tempPos.x, tempPos.y].ChangeToMovable();
                }

                Yvalue++;

            } while ((Mathf.Abs(Xvalue) + Yvalue) <= maxValue);


            Xvalue--;
        }

    }

    public void OnPlayerMovement()
    {
        TickUIElements();
        AfterMapMovement();
        CalculatePlayerMovement();
        SelectedCharacterPrefab.transform.SetParent(BattleFieldObject[Friends[SelectedFriendIndex].MapPosition.x, Friends[SelectedFriendIndex].MapPosition.y].transform, false);

    }

    public void AfterMapMovement()
    {
        foreach (CombatTile CT in BattleFieldTiles)
        {
            CT.ChangeFromMovable();
            CT.ChangeFromAttackable();
            CT.ClearLists();
        }

        //ensure that tiles are properly threatened
        foreach(Character chara in Friends)
        {
            //do the movement Calculation, but for 
            Vector2Int selectedPosition = chara.MapPosition;
            int maxValue = chara.MaxRangeOpportunity;
            Vector2Int tempPos;
            int Xvalue = maxValue;
            int Yvalue = 0;

            while (Xvalue >= -maxValue)
            {
                Yvalue = 0;
                do
                {
                    tempPos = new Vector2Int(selectedPosition.x + Xvalue, selectedPosition.y + Yvalue);

                    if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                    {
                        BattleFieldTiles[tempPos.x, tempPos.y].AddThreateningPlayer(chara);
                    }

                    tempPos = new Vector2Int(selectedPosition.x + Xvalue, selectedPosition.y - Yvalue);
                    if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                    {
                        BattleFieldTiles[tempPos.x, tempPos.y].AddThreateningPlayer(chara);
                    }

                    Yvalue++;

                } while ((Mathf.Abs(Xvalue) + Yvalue) <= maxValue);


                Xvalue--;
            }

        }
        foreach (Monster chara in Foes)
        {
            //do the movement Calculation, but for 
            Vector2Int selectedPosition = chara.MapPosition;
            int maxValue = chara.MaxRangeOpportunity;
            Vector2Int tempPos;
            int Xvalue = maxValue;
            int Yvalue = 0;

            while (Xvalue >= -maxValue)
            {
                Yvalue = 0;
                do
                {
                    tempPos = new Vector2Int(selectedPosition.x + Xvalue, selectedPosition.y + Yvalue);

                    if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                    {
                        BattleFieldTiles[tempPos.x, tempPos.y].AddThreateningMonster(chara);
                    }

                    tempPos = new Vector2Int(selectedPosition.x + Xvalue, selectedPosition.y - Yvalue);
                    if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                    {
                        BattleFieldTiles[tempPos.x, tempPos.y].AddThreateningMonster(chara);
                    }

                    Yvalue++;

                } while ((Mathf.Abs(Xvalue) + Yvalue) <= maxValue);


                Xvalue--;
            }

        }
    }

    public void ActivateCharacterSkill(int index)
    {
        if (Friends[SelectedFriendIndex].AvailableSkills.Length > index && Friends[SelectedFriendIndex].AvailableSkills[index] != null)
        {//if the selected skill is invalid, just do nothing.
            Friends[SelectedFriendIndex].AvailableSkills[Friends[SelectedFriendIndex].SelectedSkill].OnDeselect();
            Friends[SelectedFriendIndex].SelectedSkill = index;
            Friends[SelectedFriendIndex].AvailableSkills[Friends[SelectedFriendIndex].SelectedSkill].OnSelect();
        }
    }

    public void AttackFoeAt(Vector2Int location)
    {
        Attack tempAttack = Friends[SelectedFriendIndex].GenerateAttack();
        foreach (Monster foe in Foes)
        {
            if (foe.MapPosition == location)
            {
                Debug.Log("attack found viable Target");
                Friends[SelectedFriendIndex].Movement -= Friends[SelectedFriendIndex].AvailableSkills[Friends[SelectedFriendIndex].SelectedSkill].cost;
                foe.ProcessAttack(tempAttack);
                break;
            }
        }
    }

    public void TickUIElements()
    {
        FriendName.text = Friends[SelectedFriendIndex].DisplayName;
        FriendlyIcon.sprite = Friends[SelectedFriendIndex].mySprite;

        UIStatBarTexts[0].GetComponent<Text>().text = (Friends[SelectedFriendIndex].Health + "/" + Friends[SelectedFriendIndex].MaxHealth);
        UIStatBars[0].transform.localScale = new Vector3((float)Friends[SelectedFriendIndex].Health / (float)Friends[SelectedFriendIndex].MaxHealth,1,1);

        UIStatBarTexts[1].GetComponent<Text>().text = (Friends[SelectedFriendIndex].Movement + "/" + Friends[SelectedFriendIndex].Speed);
        UIStatBars[1].transform.localScale = new Vector3((float)Friends[SelectedFriendIndex].Movement / (float)Friends[SelectedFriendIndex].Speed, 1, 1);
        if (SelectedFoeIndex >= Foes.Count || SelectedFoeIndex == -1)
        {//what to do if no enemy is selected
            UIStatBarTexts[2].GetComponent<Text>().text = ("Enemy Health");
            UIStatBars[2].transform.localScale = new Vector3(1, 1, 1);

            UIStatBarTexts[3].GetComponent<Text>().text = ("Enemy Stamina");
            UIStatBars[3].transform.localScale = new Vector3(1, 1, 1);

        }
        else
        {
            FoeName.text = Foes[SelectedFoeIndex].Title;
            EnemyIcon.sprite = Foes[SelectedFoeIndex].mySprite;



            UIStatBarTexts[2].GetComponent<Text>().text = (Foes[SelectedFoeIndex].Health + "/" + Foes[SelectedFoeIndex].MaxHealth);
            UIStatBars[2].transform.localScale = new Vector3((float)Foes[SelectedFoeIndex].Health / (float)Foes[SelectedFoeIndex].MaxHealth, 1, 1);

            UIStatBarTexts[3].GetComponent<Text>().text = (Foes[SelectedFoeIndex].Movement + "/" + Foes[SelectedFoeIndex].Speed);
            UIStatBars[3].transform.localScale = new Vector3((float)Foes[SelectedFoeIndex].Movement / (float)Foes[SelectedFoeIndex].Speed, 1, 1);
        }
    }

    public void OnEntityDeath(Vector2Int location, bool isPlayer)
    {
        if (isPlayer)
        {//player Death
            foreach(Character chara in Friends)
            {
                int i = 0;
                if (chara.MapPosition == location)
                {
                    WeaponLoot.Add(chara.myWeapon);
                    ArmourLoot.Add(chara.myArmor);
                    Destroy(chara);
                    BattleFieldTiles[location.x, location.y].ChangeState(0);
                    Friends.RemoveAt(i);
                    if (TurnFriendIndex > Friends.Count - 1)
                    {
                        TurnFriendIndex = 0;
                        endedRounds[0] = 1;
                    }
                    if(SelectedFriendIndex > Friends.Count - 1)
                    {
                        SelectedFriendIndex = 0;
                    }
                }
                i++;
            }
        }
        else
        {//enemy death
            int i =0;
            foreach (Monster chara in Foes)
            {
                if (chara.MapPosition == location)
                {
                    WeaponLoot.Add(chara.myWeapon);
                    ArmourLoot.Add(chara.myArmor);
                    Destroy(chara);
                    BattleFieldTiles[location.x, location.y].ChangeState(0);
                    break;
                }
                i++;
            }
            
            Foes.RemoveAt(i);
            if (TurnFoeIndex > Foes.Count - 1)
            {

                TurnFoeIndex = 0;
                endedRounds[1] = 1;
            }
        }



        if(Foes.Count > 0 && Friends.Count > 0)
        {
            if(Friends[0].Initiative >= Foes[0].Initiative)
            {
                CharactersFirst = true;
            }
            else
            {
                CharactersFirst = false;
            }
        
        }
        else
        {//Hey, maps cleared buddy.
            if(Foes.Count == 0)
            {
                ReturnToBase(true);
            }
            else
            {
                ReturnToBase(false);
            }
        }

    }

    public void ReturnToBase(bool successful)
    {
        if (successful)
        {//all enemies defeated
            HomeBase.ResourceConMat += ResourceConMat;
            HomeBase.ResourceGold += ResourceGold;
            HomeBase.ResourceOre += ResourceOre;
            HomeBase.BaseWeaponInventory.AddRange(WeaponLoot);
            HomeBase.BaseArmorInventory.AddRange(ArmourLoot);

            for (int i = Friends.Count - 1; i >= 0; i--)
            {
                Character temp = null;
                temp = Friends[i];
                Friends.RemoveAt(i);

                HomeBase.TakeCharacter(temp);
                break;
            }
        }
        else
        {
            if(Friends.Count > 0)
            {//Retreat button hit
                //get survivors out
                for (int i = Friends.Count - 1; i >= 0; i--)
                {
                    Character temp = null;
                    temp = Friends[i];
                    Friends.RemoveAt(i);

                    HomeBase.TakeCharacter(temp);
                    break;
                }
                //take home half the stuff you would have gotten on a victory
                HomeBase.ResourceConMat += ResourceConMat/2;
                HomeBase.ResourceGold += ResourceGold/2;
                HomeBase.ResourceOre += ResourceOre/2;
                int removeAmount = WeaponLoot.Count / 2;
                while(removeAmount > 0)
                {
                    WeaponLoot.RemoveAt(Random.Range(0, WeaponLoot.Count - 1));
                    removeAmount--;
                }
                HomeBase.BaseWeaponInventory.AddRange(WeaponLoot);


                removeAmount = ArmourLoot.Count / 2;
                while (removeAmount > 0)
                {
                    ArmourLoot.RemoveAt(Random.Range(0, ArmourLoot.Count - 1));
                    removeAmount--;
                }
                HomeBase.BaseArmorInventory.AddRange(ArmourLoot);

            }
            else
            {//everyone's dead

            }

        }
        //stuff that always happens.
        WeaponLoot.Clear();
        ArmourLoot.Clear();
        ResourceConMat = 0; ResourceGold = 0; ResourceOre = 0;
        foreach(CombatTile CT in BattleFieldTiles)
        {
            CT.ChangeState(0);
        }
        foreach(Character friend in Friends)
        {
            friend.MapPosition = new Vector2Int(-1, -1);
        }
        Foes.Clear();
        HomeBase.OnReturnFromCombat();
        ReturnsToMainBase.Toggle();
    }


    private void MonsterTurn()
    {
        int minDistance = 5000, targetIndex = -1, i=0;
        Vector2Int MyMapPos = Foes[TurnFoeIndex].MapPosition, RelativePosition = new Vector2Int(0,0);
        Foes[TurnFoeIndex].Movement = Foes[TurnFoeIndex].Speed;

        foreach (Character chara in Friends)
        {//Determine the closest character. Later on determien most THREATENING character
            if(Mathf.Abs(MyMapPos.x- chara.MapPosition.x) + Mathf.Abs(MyMapPos.y - chara.MapPosition.y) < minDistance)
            {
                RelativePosition.x = chara.MapPosition.x - MyMapPos.x;
                RelativePosition.y = chara.MapPosition.y - MyMapPos.y;
                minDistance = Mathf.Abs(MyMapPos.x - chara.MapPosition.x) + Mathf.Abs(MyMapPos.y - chara.MapPosition.y);
                targetIndex = i;
            }
            i++;
        }
        //Move towards target, and/or attack
        while(Foes[TurnFoeIndex].Movement > 0)
        {//2 here is weapon range
            if (minDistance <= 2)
            {
                Friends[targetIndex].ProcessAttack(Foes[TurnFoeIndex].GenerateAttack());
                break;
            }
            else
            {
                if(Mathf.Abs(RelativePosition.x) > Mathf.Abs(RelativePosition.y))
                {
                    BattleFieldTiles[MyMapPos.x, MyMapPos.y].ChangeState(0);
                    MyMapPos.x += RelativePosition.x / Mathf.Abs(RelativePosition.x);
                    if( BattleFieldTiles[MyMapPos.x, MyMapPos.y].myTileState == TileState.Empty) { //Prevent Overlapping
                        BattleFieldTiles[MyMapPos.x, MyMapPos.y].ChangeState(3);
                        Foes[TurnFoeIndex].MapPosition = MyMapPos;
                        RelativePosition.x -= RelativePosition.x / Mathf.Abs(RelativePosition.x);
                    }
                    else
                    {
                        MyMapPos.x -= RelativePosition.x / Mathf.Abs(RelativePosition.x);
                        BattleFieldTiles[MyMapPos.x, MyMapPos.y].ChangeState(3);
                    }
                }
                else
                {

                    BattleFieldTiles[MyMapPos.x, MyMapPos.y].ChangeState(0);
                    MyMapPos.y += RelativePosition.y/ Mathf.Abs(RelativePosition.y);

                    if (BattleFieldTiles[MyMapPos.x, MyMapPos.y].myTileState == TileState.Empty || BattleFieldTiles[MyMapPos.x, MyMapPos.y].myTileState == TileState.Threatened)
                    { //Prevent Overlapping
                        BattleFieldTiles[MyMapPos.x, MyMapPos.y].ChangeState(3);
                        Foes[TurnFoeIndex].MapPosition = MyMapPos;
                        RelativePosition.y -= RelativePosition.y / Mathf.Abs(RelativePosition.y);
                    }
                    else
                    {
                        MyMapPos.y -= RelativePosition.y / Mathf.Abs(RelativePosition.y);
                        BattleFieldTiles[MyMapPos.x, MyMapPos.y].ChangeState(3);
                    }
                }
                minDistance--;
                Foes[TurnFoeIndex].Movement--;
            }
          
        }
        TickUIElements();
        AfterMapMovement();
    }

    public void GenerateCombatEncounter()
    {
        int enemyNumbers = (int)((.5 + Random.Range(0, 1)) * Friends.Count);
        enemyNumbers++;

        for(int i=0; i< enemyNumbers; i++)
        {//Generate enemies, give them weapons and armor
           
            Monster tempMonster = Instantiate(EmptyMonsterPrefab, gameObject.transform).GetComponent<Monster>();
            tempMonster.myEncounter = this;
            tempMonster.myWeapon = tempMonster.gameObject.AddComponent<Weapon>();
            tempMonster.myArmor = tempMonster.gameObject.AddComponent<Armour>();
            int seed = Random.Range(ThreatLevelMin, ThreatLevelMax);
            int gearAlloc = Random.Range(0, seed);
            if(gearAlloc > 20)
            {
                gearAlloc = 20;
            }
            else if(gearAlloc > (.75 * seed))
            {
                gearAlloc = (int)(.75 * seed);
            }

            int temp = Mathf.Clamp(Random.Range(0, gearAlloc), 0, 10);
            tempMonster.myWeapon = WeaponTypes[temp];
            tempMonster.myArmor =ArmourTypes[gearAlloc - temp];
            tempMonster.AssignRandomStats(seed - gearAlloc);

            Foes.Add(tempMonster);


        }


    }


}
