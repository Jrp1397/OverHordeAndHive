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
    [SerializeField] private BaseManager HomeBase;
    public GameObject FieldTilePrefab;
    private GameObject[,] BattleFieldObject = new GameObject [4,12];
    public CombatTile[,] BattleFieldTiles = new CombatTile[4, 12];
    private int[] endedRounds = { 0, 0 };
    public int SelectedFoeIndex = 0, SelectedFriendIndex = 0;
    public int TurnFoeIndex = 0, TurnFriendIndex = 0;
    [SerializeField] private List<int> Seed;
    public int DangerSeedModifer;
    [SerializeField] private Text FoeName, FriendName;
    public Attack TestAttack;
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

    public void TickCharacters()
    {
        foreach(Character chara in Friends)
        {
            chara.AlterStats();
        }
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

        FoeName.text = Foes[SelectedFoeIndex].Title;
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
        FriendName.text = Friends[SelectedFriendIndex].DisplayName;
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
        foreach (Character chara in Friends)
        {
            Debug.Log(chara.Initiative);
        }
        CombatTick();


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
       // ClearMapOfMovement();
        if (PlayerNext)
        {//Player Controls
            Friends[TurnFriendIndex].Movement = Friends[TurnFriendIndex].Speed;
            SelectedFriendIndex = TurnFriendIndex;
            FriendName.text = Friends[SelectedFriendIndex].DisplayName;            
            CalculatePlayerMovement();
            TurnFriendIndex++;
            if(TurnFriendIndex > Friends.Count - 1)
            {
              
                endedRounds[0] = 1;
            }

        }
        else
        {//Monster turn
            //Debug.Log(Foes[TurnFoeIndex].Title + "'s Turn");
            TurnFoeIndex++;
            if(TurnFoeIndex > Foes.Count - 1)
            {
                endedRounds[1] = 1;
            }
        }
        
        if(endedRounds[0] ==0 || endedRounds[1]==0)
        {
            if(Friends[SelectedFriendIndex].Initiative >= Foes[SelectedFoeIndex].Initiative || endedRounds[1] == 1)
            {//Players go next
                PlayerNext = true;
            FriendName.text = Friends[SelectedFriendIndex].DisplayName;
            }
            else
            {

                FoeName.text = Foes[TurnFoeIndex].Title;
                PlayerNext = false;
                autotick = true;
            }
        }
        else
        {//round restarts.
            endedRounds[0] = 0;
            endedRounds[1] = 0;
            TurnFoeIndex = 0;
            TurnFriendIndex = 0;
            PlayerNext = CharactersFirst;
            if (!PlayerNext)
            {
                autotick = true;
            }           
        }
    }

    private void CalculatePlayerMovement()
    {
        //Propagate across movement range empty tiles to moveable, enemy tiles to Attackable
        //Get position of selected character
        //Get their movement left
        //Start loop that paints in all directions, checking for valid states of the tiles
        
        Vector2Int selectedPosition = Friends[SelectedFriendIndex].MapPosition;
        Vector2Int tempPos;
        int Xvalue = Friends[SelectedFriendIndex].Movement;
        int Yvalue = 0;
        ClearMapOfMovement();
        Debug.Log("calculating Movement for player # " + SelectedFriendIndex + " With a movement of " + Xvalue );
        for (int i=0; i <= Friends[SelectedFriendIndex].Movement; i++)
        {
            while (Xvalue >= 0)
            {
                tempPos = selectedPosition;
                tempPos.x += Xvalue;
                tempPos.y += Yvalue;
                if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                {
                    BattleFieldTiles[tempPos.x, tempPos.y].ChangeToMovable();
                }

                tempPos = selectedPosition;
                tempPos.x -= Xvalue;
                tempPos.y += Yvalue;

                if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                {
                    BattleFieldTiles[tempPos.x, tempPos.y].ChangeToMovable();
                }


                tempPos = selectedPosition;
                tempPos.x += Xvalue;
                tempPos.y -= Yvalue;
                if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                {
                    BattleFieldTiles[tempPos.x, tempPos.y].ChangeToMovable();
                }


                tempPos = selectedPosition;
                tempPos.x -= Xvalue;
                tempPos.y -= Yvalue;
                if ((tempPos.x >= 0 && tempPos.x < 4) && (tempPos.y >= 0 && tempPos.y < 12))
                {
                    BattleFieldTiles[tempPos.x, tempPos.y].ChangeToMovable();
                }

                Xvalue--;
            }
            Yvalue++;
            Xvalue = Friends[TurnFriendIndex].Movement - Yvalue;
        }



    }

    public void OnPlayerMovement()
    {
        ClearMapOfMovement();
    }

    public void ClearMapOfMovement()
    {
        Debug.Log("Clearing Map");
        foreach (CombatTile CT in BattleFieldTiles)
        {
            CT.ChangeFromMovable();
        }
    }

    public void AttackFoeAt(Vector2Int location, Attack incAttack)
    {
        foreach(Monster foe in Foes)
        {
            if (foe.MapPosition == location)
            {
                Debug.Log("attack found viable Target");
                foe.ProcessAttack(incAttack);
            }
        }
    }

}
