using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkillObject : MonoBehaviour
{

    public Skill mySkill;
    public  bool isCharacterInventory, isDirty, isFull=true;
    [SerializeField] private BaseManager myBaseManager;
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
                gameObject.GetComponent<Image>().sprite = mySkill.mySprite;
                 
            }
            else if (isCharacterInventory)
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
        Debug.Log("clicked")
;        if (isCharacterInventory)//Remove Skill
        {
            if (isFull)//occupied slot, clear it
            {
                mySkill = null;
                isFull = false;
                isDirty = true;
                myBaseManager.SelectedSkill = Index;
            }
            else
            {//unoccupied slot, fill it. 
                myBaseManager.SelectedSkill = Index;
            }
        }
        else//Base skill
        {           
            myBaseManager.AssignSkilltoSelectedUnit(mySkill);
            myBaseManager.UpdateSkillsPanel();
        }
    }


   }
