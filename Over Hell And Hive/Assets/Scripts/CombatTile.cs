using System.Collections;
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
                if (myEncounter.Friends[myEncounter.SelectedFriendIndex].Movement > 0) { 
                    ChangeState(7);
                }
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

 
   
    private void OnMouseOver()
    {
        Vector2Int oldPosition;
      
        if (Input.GetMouseButtonDown(1))//right Click
        {
            myEncounter.SelectTileOnRightClick(MapPosition);
        }

        if (Input.GetMouseButtonDown(0))
        {//For Left Clicks
            switch (myTileState)
            {
                case TileState.Empty:
                    break;
                case TileState.Blocked:
                    break;
                case TileState.Ally:
                    break;
                case TileState.Enemy:
                    myEncounter.OnEnemyTileClick(MapPosition);
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
                    break;
                case TileState.Threatened:
                    mySR.color = Color.yellow;
                    break;
                case TileState.Attackable:
                    if (myEncounter.Friends[myEncounter.SelectedFriendIndex].Movement > 0)
                    {
                        myEncounter.AttackFoeAt(MapPosition);
                        myEncounter.OnPlayerMovement();
                        mySR.color = Color.magenta;
                    }
                    else
                    {
                        Debug.Log("not Enough Movement Left.");

                    }
                    break;
                case TileState.InRange:

                    mySR.color = Color.Lerp(Color.red, Color.white, .5f);
                    break;
                default:
                    break;
            }
        }

    }

}
