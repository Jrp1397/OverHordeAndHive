using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] private BaseManager myBase;
    [SerializeField] private int BuildingType;
    public int Level, ManpowerTotal, ManpowerLeft, ManpowerRight;
    public int [] BaseCostToUpgrade = new int [3] ;
    public Image ButtonSR; 
    public Text StatusText, CostToUpgrade;

    // Start is called before the first frame update
    void Start()
    {
        UpdateStatusText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void IncreaseManpower( bool Left)
    {       
        if (Left)
        {
            ManpowerLeft++;
        }
        else
        {
            ManpowerRight++;
        }
        ManpowerTotal = ManpowerRight + ManpowerLeft;
        UpdateStatusText();

    }

    public void DecreaseManpower()
    {
        if (ManpowerLeft > 0 &&(ManpowerLeft >= ManpowerRight))
        {
            ManpowerLeft--;
        }
        else if(ManpowerRight > 0){
            ManpowerRight--;
        }
        ManpowerTotal = ManpowerRight + ManpowerLeft;
        UpdateStatusText();
    }

    public void ShiftManpower(bool Left)
    {
        if (Left)
        {
            if (ManpowerRight >= 1)
            {
                ManpowerLeft++;
                ManpowerRight--;
            }
        }
        else
        {
            if (ManpowerLeft >= 1)
            {
                ManpowerLeft--;
                ManpowerRight++;
            }
        }

        ManpowerTotal = ManpowerRight + ManpowerLeft;
        UpdateStatusText();

    }



    public void UpdateStatusText()
    {//Activates when pressed
        switch (BuildingType)
        {
            case 0://Expedition

                StatusText.text = "Current Manpower = " + ManpowerTotal + "\n";
                StatusText.text += "Current Max Party Size= " + ManpowerLeft + "\n";
                StatusText.text += "Current Max Expedition Length = " + (ManpowerRight*2) +5 + "\n";
                break;
            case 1://Quarry
                StatusText.text = "Level " + Level + " Quarry\n";
                StatusText.text += "Current Manpower = " + ManpowerTotal + "\n";
                StatusText.text += "Current Stone Production = " + ManpowerLeft * 2 + "\n";
                StatusText.text += "Current Ore Production = " + ManpowerRight * 2 + "\n";
                break;

            case 2://Trading Post
                StatusText.text = "Level " + Level + " Trading Post\n";
                StatusText.text += "Current Manpower = " + ManpowerTotal + "\n";
                StatusText.text += "Current Gold Production = " + ManpowerLeft * 5 + "\n";
                StatusText.text += "Current Units Recuited = " + ManpowerRight + "\n";
                break;
            case 3://Forge
                StatusText.text = "Level " + Level + " Forge\n";
                StatusText.text += "Current Manpower = " + ManpowerTotal + "\n";
                StatusText.text += "Current Ore Consumption = " + ManpowerTotal*2 + "\n";
                StatusText.text += "Current Weapon Production = " + ManpowerLeft *3 + "\n";
                StatusText.text += "Current Armor Production = " + ManpowerRight * 2 + "\n";

                break;
            default:
                break;
        }

        CostToUpgrade.text = " ";
        if(BaseCostToUpgrade[0] > 0)
        {
            CostToUpgrade.text += BaseCostToUpgrade[0] * Level + " Gold ";
        }
        if (BaseCostToUpgrade[1] > 0)
        {
            CostToUpgrade.text += BaseCostToUpgrade[1] * Level + " Brick ";
        }
        if (BaseCostToUpgrade[2] > 0)
        {
            CostToUpgrade.text += BaseCostToUpgrade[2] * Level + " Ore ";
        }

        if (myBase.ResourceGold > BaseCostToUpgrade[0]*Level && myBase.ResourceConMat > BaseCostToUpgrade[1] * Level && myBase.ResourceOre > BaseCostToUpgrade[2] * Level)
        {
            ButtonSR.color = Color.white;
        }
        else
        {
            ButtonSR.color = Color.red;
        }
        myBase.UpdateIncomes();
    }

    public void UpgradeBuilding()
    {//Upgrades the building, and increases the basic manpower present if it is a resource producing building
        if (myBase.ResourceGold > BaseCostToUpgrade[0] * Level && myBase.ResourceConMat > BaseCostToUpgrade[1] * Level && myBase.ResourceOre > BaseCostToUpgrade[2] * Level)
        {
            myBase.ResourceGold -= BaseCostToUpgrade[0] * Level;
            myBase.ResourceConMat -= BaseCostToUpgrade[1] * Level;
            myBase.ResourceOre -= BaseCostToUpgrade[2] * Level;
            Level++;
            if(BuildingType != 0)
            {
                ManpowerLeft++;
                ManpowerRight++;
            }



            myBase.UpdateIncomes();
            UpdateStatusText();
        }

    }

}
