﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState {Empty=0, Blocked=1, Ally=2, Enemy=3, Deployable=4, Movable=5, Threatened=6, Attackable=7, InRange=8}

public class CombatTile : MonoBehaviour
{
    private SpriteRenderer mySR;
    public TileState myTileState;
    public bool isTileStateDirty;
    public Vector2Int MapPosition;
    public Encounter myEncounter;

    
    // Start is called before the first frame update
    void Start()
    {
        mySR = gameObject.GetComponent<SpriteRenderer>();    
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTileStateDirty)
        {
            isTileStateDirty = false;
            ChangeColor();
        }
        
    }

    public void ChangeState(int incomingState)
    {
        if(incomingState != (int)myTileState)
        {//only run the update if things actually change
            isTileStateDirty = true;
            myTileState = (TileState)incomingState;
        }
    }

    void ChangeColor()
    {
        switch (myTileState)
        {
            case TileState.Empty:
                mySR.color = Color.white;
                break;
            case TileState.Blocked:
                mySR.color = Color.black;
                break;
            case TileState.Ally:
                mySR.color = Color.blue;
                break;
            case TileState.Enemy:
                mySR.color = Color.red;
                break;
            case TileState.Deployable:
                mySR.color = new Color(83.0f / 256.0f, 196.0f / 256.0f, 61.0f / 256.0f);
                break;
            case TileState.Movable:
                mySR.color = Color.green;
                break;
            case TileState.Threatened:
                mySR.color = Color.yellow;
                break;
            case TileState.Attackable:
                mySR.color = Color.magenta;
                break;
            case TileState.InRange:
                mySR.color = Color.Lerp(Color.red, Color.white, .5f);
                break;
            default:
                break;
        }
    }

    public void ChangeToMovable()
    {
        switch (myTileState)
        {
            case TileState.Empty:
                ChangeState(5);
                break;
            case TileState.Enemy:
                ChangeState(7);
                break;
            default:
                break;
        }
    }

    public void ChangeFromMovable()
    {
        switch (myTileState)
        {
            case TileState.Movable:
                ChangeState(0);
             break;
            case TileState.Attackable:
                ChangeState(3);
            break;

}
        }
   
    private void OnMouseDown()
    {
        Debug.Log("You clicked on a " + myTileState.ToString() + " Tile");
        Vector2Int oldPosition;
        switch (myTileState)
        {
            case TileState.Empty:
                Debug.Log("You clicked on a " + myTileState.ToString() + " Tile. This Does nothing.");
                break;
            case TileState.Blocked:
                Debug.Log("You clicked on a " + myTileState.ToString() + " Tile. You can't go here, sorry.");
                break;
            case TileState.Ally:
                Debug.Log("You clicked on a " + myTileState.ToString() + " Tile. This should change the selected Hero to this guy. we're working on it...");
                break;
            case TileState.Enemy:
                Debug.Log("You clicked on a " + myTileState.ToString() + " Tile. This should change the selected Enemy to this guy. we're working on it...");
                break;
            case TileState.Deployable:
                 oldPosition = myEncounter.Friends[myEncounter.SelectedFriendIndex].MapPosition;
                if (oldPosition.x >= 0)
                {
                    myEncounter.BattleFieldTiles[oldPosition.x, oldPosition.y].ChangeState(4);// make the old position available.
                }
                myEncounter.MoveSelectedCharacterTo(MapPosition, 0);
                ChangeState(2);//Place Ally here
                myEncounter.CycleSelFriend(true);
                Debug.Log("You clicked on a " + myTileState.ToString() + " Tile. You have moved a character into this spot.");
                break;
            case TileState.Movable:
                oldPosition = myEncounter.Friends[myEncounter.SelectedFriendIndex].MapPosition;
                if (oldPosition.x >= 0)
                {
                    myEncounter.BattleFieldTiles[oldPosition.x, oldPosition.y].ChangeState(0);// make the old position available.
                }
                myEncounter.MoveSelectedCharacterTo(MapPosition, 0);
                int movementTotal = Mathf.Abs(oldPosition.x - MapPosition.x) + Mathf.Abs(oldPosition.y - MapPosition.y);
                myEncounter.Friends[myEncounter.SelectedFriendIndex].Movement -= movementTotal;
                myEncounter.OnPlayerMovement();
                ChangeState(2);//Place Ally here
                Debug.Log("You clicked on a " + myTileState.ToString() + " Tile. You have moved a character into this spot. But we need to account for the speed, and reduce his movement.");
                break;
            case TileState.Threatened:
                Debug.Log("You clicked on a " + myTileState.ToString() + " Tile. This is a dangerous spot for someone.");
                mySR.color = Color.yellow;
                break;
            case TileState.Attackable:
                Debug.Log("You clicked on a " + myTileState.ToString() + " Tile.This should generate and then distribute an attack. Eek.");
                mySR.color = Color.magenta;
                break;
            case TileState.InRange:

                Debug.Log("You clicked on a " + myTileState.ToString() + " Tile. This shows you were you could attack, but theres no-one here...");
                mySR.color = Color.Lerp(Color.red, Color.white, .5f);
                break;
            default:
                break;
        }

    }

}
