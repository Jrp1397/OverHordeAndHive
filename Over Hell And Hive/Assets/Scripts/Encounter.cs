﻿using System.Collections;
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
    public Image FriendlyIcon;
    public Image EnemyIcon;
    public GameObject FieldTilePrefab;
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
    public Attack TestAttack;
    public GameObject SelectedCharacterObject;
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
        foreach (Character chara in Friends)
        {

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
            TickUIElements();
            SelectedCharacterObject.transform.SetParent(BattleFieldObject[Friends[SelectedFriendIndex].MapPosition.x, Friends[SelectedFriendIndex].MapPosition.y].transform, false);
           // SelectedCharacterObject.transform.position = new Vector3(0, 0, 0);
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
            TickUIElements();
            SelectedCharacterObject.transform.SetParent(BattleFieldObject[Foes[SelectedFoeIndex].MapPosition.x, Foes[SelectedFoeIndex].MapPosition.y].transform, false);
           // SelectedCharacterObject.transform.position = new Vector3(0, 0, 0);

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
                TickUIElements();
                SelectedCharacterObject.transform.SetParent(BattleFieldObject[Friends[SelectedFriendIndex].MapPosition.x, Friends[SelectedFriendIndex].MapPosition.y].transform, false);

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
        int maxValue = Friends[SelectedFriendIndex].Movement; 
        Vector2Int tempPos;
        int Xvalue = maxValue;
        int Yvalue = 0;
        ClearMapOfMovement();
      
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
        ClearMapOfMovement();
        CalculatePlayerMovement();
        SelectedCharacterObject.transform.SetParent(BattleFieldObject[Friends[SelectedFriendIndex].MapPosition.x, Friends[SelectedFriendIndex].MapPosition.y].transform, false);

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

    public void TickUIElements()
    {
        FriendName.text = Friends[SelectedFriendIndex].DisplayName;
        FriendlyIcon.sprite = Friends[SelectedFriendIndex].mySprite;
        FoeName.text = Foes[SelectedFoeIndex].Title;
        EnemyIcon.sprite = Foes[SelectedFoeIndex].mySprite;

        UIStatBarTexts[0].GetComponent<Text>().text = (Friends[SelectedFriendIndex].Health + "/" + Friends[SelectedFriendIndex].MaxHealth);
        UIStatBars[0].transform.localScale = new Vector3((float)Friends[SelectedFriendIndex].Health / (float)Friends[SelectedFriendIndex].MaxHealth,1,1);

        UIStatBarTexts[1].GetComponent<Text>().text = (Friends[SelectedFriendIndex].Movement + "/" + Friends[SelectedFriendIndex].Speed);
        UIStatBars[1].transform.localScale = new Vector3((float)Friends[SelectedFriendIndex].Movement / (float)Friends[SelectedFriendIndex].Speed, 1, 1);

        UIStatBarTexts[2].GetComponent<Text>().text = (Foes[SelectedFoeIndex].Health + "/" + Foes[SelectedFoeIndex].MaxHealth);
        UIStatBars[2].transform.localScale = new Vector3((float)Foes[SelectedFoeIndex].Health / (float)Foes[SelectedFoeIndex].MaxHealth, 1, 1);

        UIStatBarTexts[3].GetComponent<Text>().text = (Foes[SelectedFoeIndex].Movement + "/" + Foes[SelectedFoeIndex].Speed);
        UIStatBars[3].transform.localScale = new Vector3((float)Foes[SelectedFoeIndex].Movement / (float)Foes[SelectedFoeIndex].Speed, 1, 1);
    }
}
