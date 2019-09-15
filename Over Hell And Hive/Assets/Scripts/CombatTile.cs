using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState {Empty=0, Blocked=1, Ally=2, Enemy=3, Deployable=4, Movable=5, Threatened=6, Attackable=7, InRange=8}

public class CombatTile : MonoBehaviour
{
    private SpriteRenderer mySR;
    public TileState myTileState;
    public bool isTileStateDirty;

    
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

}
