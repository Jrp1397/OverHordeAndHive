using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryObject : MonoBehaviour
{

    public BaseManager HomeBase;
    public Encounter ExplorationPhase;
    public bool IsArmor, isBaseInventory, isFull, isDirty;
    public Armour possibleArmor;
    public Weapon possibleWeapon;
    public Sprite DefaultTexture;
    public int Index;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDirty)
        {//update the sprite
            isDirty = false;
            if (isFull)
            {
                if (IsArmor)
                {
                    if (possibleArmor.ArmorIcon != null)
                    {
                        gameObject.GetComponent<Image>().sprite = possibleArmor.ArmorIcon;
                    }
                }
                else
                {
                    if (possibleWeapon.WeaponIcon != null)
                    {
                        gameObject.GetComponent<Image>().sprite = possibleWeapon.WeaponIcon;
                    }
                }

            }
            else  if(!isBaseInventory)
            {
                gameObject.GetComponent<Image>().sprite = DefaultTexture;
            }
            else
            {
                gameObject.SetActive(false);
            }

        }
    }

    public void OnMouseDown()
    {
        Debug.Log("Clicked Object");


        if (isBaseInventory)//Base Inventory- moves things into player inventory
        {
            if (IsArmor)
            {
                HomeBase.GiveItemToSelectedUnit(true, HomeBase.AccessSelectedCharacter(), Index);
            }
            else
            {
                HomeBase.GiveItemToSelectedUnit(false, HomeBase.AccessSelectedCharacter(), Index);
            }
               
        }
        else//Player inventory- if its already clicked, moved the object back to base inventory
        {
            if (gameObject == HomeBase.LastClickedPlayerInventory)
            {//already clicked
                if (isFull)
                {//if the item is empty, don't do anything to it
                    if (IsArmor)
                    {
                        HomeBase.TakeItemFromSelectedUnit(true, HomeBase.AccessSelectedCharacter());
                        isFull = false;
                        possibleArmor = null;
                        isDirty = true;
                    }
                    else
                    {
                        HomeBase.TakeItemFromSelectedUnit(false, HomeBase.AccessSelectedCharacter());
                        isFull = false;
                        possibleWeapon = null;
                        isDirty = true;
                    }
                }
            }
            else {

                HomeBase.LastClickedPlayerInventory = gameObject;
                if (IsArmor)
                {
                    HomeBase.UpdateInventoryPanel(HomeBase.Combatants[HomeBase.selectedChar], 1);
                }
                else
                {
                    HomeBase.UpdateInventoryPanel(HomeBase.Combatants[HomeBase.selectedChar], 2);
                }
            }
        }





    }
}
