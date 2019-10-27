using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseManager : MonoBehaviour
{
    public List<Character> Combatants;
    public int selectedChar = 0;
    public Attack TestAttack;
    public int ResourceGold = 0, ResourceConMat = 0, ResourceOre = 0, ResourceTomes = 0;
    public List<Armour> BaseArmorInventory;
    public List<Weapon> BaseWeaponInventory;
    public List<Skill> AvailableSkills;
    public int SelectedSkill;
    [SerializeField] private Text[] StatsBlock = new Text[9];
    [SerializeField] private GameObject[] DisplayInventory;
    [SerializeField] private GameObject[] DisplaySkills;
    [SerializeField] private Skill[] SkillInventory;
    [SerializeField] private GameObject[] PlayerInventory;
    [SerializeField] private GameObject[] PlayerSkills;
    [SerializeField] private Encounter ExplorationPhase;
    [SerializeField] private Text AddorRemovetextBox;
    [SerializeField] private Text[] CurrencyBoxes;
    [SerializeField] private Image CharacterDisplay;
    [SerializeField] private Sprite[] WorkIcons;
    [SerializeField] private Building[] Buildings;
    public GameObject CharacterSelectorPrefab;
    public GameObject CharacterSelectionBox;
    private List<GameObject> CharacterSelectionButtons;
    public GameObject LastClickedPlayerInventory = null;

    // Start is called before the first frame update
    void Start()
    {
        TestAttack.ToHitValue = 11;
        TestAttack.PenValue = 2;
        TestAttack.ToCritModifier = 5;
        TestAttack.Damage = new Vector3(3, 2, 1);
        SetSelectedCharacter(0);
        UpdateCharacterSelection();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignSelectedCharacterJob(int incWork)
    {
        Character tempChar = AccessSelectedCharacter();
        Buildings[(int)tempChar.myWork / 2].DecreaseManpower();
        tempChar.myWork = (Worktype)incWork;
        Buildings[(int)incWork / 2].IncreaseManpower(true);
        UpdateCharacterSelection();
    }

    public Character AccessSelectedCharacter()
    {
        Character temp = null;
        for (int i = Combatants.Count - 1; i >= 0; i--)
        {
            if (Combatants[i].UniqueID == selectedChar)
            {
                temp = Combatants[i];
                break;
            }
        }

        if (temp == null)
        {
            temp = ExplorationPhase.AccessCharacterById(selectedChar);
        }

        return temp;



    }
    
    public void GiveCharacterById(int incID)
    {
        Character temp = null;
        for (int i = Combatants.Count-1; i >= 0; i--)
        {
            if (Combatants[i].UniqueID == incID)
            {
                temp = Combatants[i];
                ExplorationPhase.TakeCharacter(temp);
                Combatants.RemoveAt(i);
                break;
            }
        }
    }

    public void TakeCharacter(Character incChar)
    {
        if (incChar == null) { return; }
        for (int i = Combatants.Count - 1; i >= 0; i--)//if a character is being updated, replace the old character
        {
            if (Combatants[i].UniqueID == incChar.UniqueID)
            {
                Combatants[i] = incChar;
                return;
            }
        }//otherwise, its a new version, add it to the list.

        Combatants.Add(incChar);

    }

    public void DisplayCharacters()
    {
        foreach (Character chara in Combatants)
        {
            chara.PingStrength();
        }
    }

    public void ToggleActiveCharacterExpedition()
    {
        for (int i = Combatants.Count - 1; i >= 0; i--)
        {
            if (Combatants[i].UniqueID == selectedChar)//if this is the selected Character
            {
                GiveCharacterById(selectedChar);
                AddorRemovetextBox.text = "Remove Unit From Expedition";
                return;
            }
        }
        ExplorationPhase.GiveCharacterById(selectedChar);
        AddorRemovetextBox.text = "Add Unit To Expedition";
    }

    public void DirectionalToggleActiveCharacterExpedition(bool embarking)
    {
        if (embarking)
        {
            GiveCharacterById(selectedChar);
            AddorRemovetextBox.text = "Remove Unit From Expedition";
        }
        else
        {
            ExplorationPhase.GiveCharacterById(selectedChar);
            AddorRemovetextBox.text = "Add Unit To Expedition";
        }
    }

    public void SetSelectedCharacter(int incnumb)
    {//change the value, and then update any required UI elements.
        selectedChar = incnumb;
        bool isCharhere = false;
        LastClickedPlayerInventory = null;

        for (int i = Combatants.Count - 1; i >= 0; i--)
        {
            if (Combatants[i].UniqueID == selectedChar)//if this is the selected Character
            {
                isCharhere = true;
                UpdateStatsPanel(Combatants[i]);
                UpdateInventoryPanel(Combatants[i], 0);
                UpdateSkillsPanel();
            }
        }

        if (isCharhere)
        {
            AddorRemovetextBox.text = "Add Unit To Expedition";
        }
        else
        {
            UpdateStatsPanel(ExplorationPhase.AccessCharacterById(incnumb));
            UpdateInventoryPanel(ExplorationPhase.AccessCharacterById(incnumb),0);
            UpdateSkillsPanel();
            AddorRemovetextBox.text = "Remove Unit From Expedition";
        }
    }

    //takes item from unit, and adds it to bases list
    public void TakeItemFromSelectedUnit(bool isArmor, Character IncChar)
    {
        if (isArmor)
        {
            if (IncChar != null)
            {
                if (IncChar.myArmor != null)
                {
                    BaseArmorInventory.Add(IncChar.myArmor);
                    IncChar.myArmor = null;
                }
            }
        }
        else
        {
            if (IncChar.myWeapon != null)
            {
                if (IncChar.myWeapon != null)
                {
                    BaseWeaponInventory.Add(IncChar.myWeapon);
                    IncChar.myWeapon = null;
                }
            }
        }
    }

    //takes item from bases list, and gives it to unit
    public void GiveItemToSelectedUnit(bool isArmor, Character IncChar, int index)
    {
        Debug.Log("index =" + index);
        if(isArmor && index < BaseArmorInventory.Count)
        {
            IncChar.myArmor = BaseArmorInventory[index];
            BaseArmorInventory.RemoveAt(index);
        }
        else if ( index < BaseWeaponInventory.Count)
        {
            IncChar.myWeapon = BaseWeaponInventory[index];
            BaseWeaponInventory.RemoveAt(index);
        }
    }

    public void AssignSkilltoSelectedUnit(Skill IncSkill)
    {
        AccessSelectedCharacter().AvailableSkills[SelectedSkill] = IncSkill;
    }

    public void UpdateStatsPanel(Character IncChar){
        //current order = Name, Health, Attack, Armor, Stamina, Speed, Magic, Resistance, and Mana
        StatsBlock[0].text = (IncChar.DisplayName);

        StatsBlock[1].text = ("Health: " +IncChar.Health + "/" + IncChar.MaxHealth);
        if (IncChar.myWeapon == null)
        {
            StatsBlock[3].text = ("Attack: 0/0/0");
        }
        else
        {
            StatsBlock[2].text = ("Attack: " + IncChar.myWeapon.SlashOffence + "/" + IncChar.myWeapon.PierceOffence + "/" + IncChar.myWeapon.CrushOffence);
        }
        if (IncChar.myArmor == null)
        {
            StatsBlock[3].text = ("Armor: 0/0/0");
        }
        else
        {
            StatsBlock[3].text = ("Armor: " + IncChar.myArmor.SlashDefence + "/" + IncChar.myArmor.PierceDefence + "/" + IncChar.myArmor.CrushDefence);
        }
        StatsBlock[4].text = ("Stamina: " + IncChar.Stamina + "/" + IncChar.MaxHealth);
        StatsBlock[5].text = ("Speed: " + IncChar.Speed);
        StatsBlock[6].text = ("Wisdom: " + IncChar.Wis);
        StatsBlock[7].text = ("Resistance: " + IncChar.Cha);
        StatsBlock[8].text = ("Mana: " + +IncChar.MP+ "/" + IncChar.MaxMP);
        CharacterDisplay.sprite = IncChar.mySprite;
    }

    public void UpdateInventoryPanel(Character IncChar, int isNullArmororWeapon)
    {
        foreach(GameObject obby in DisplayInventory)
        {
            obby.SetActive(false);
        }

        //player inventory
        InventoryObject temp = PlayerInventory[0].GetComponent<InventoryObject>();
        temp.possibleArmor = IncChar.myArmor; 
        if(IncChar.myArmor == null) { temp.isFull = false; } else { temp.isFull = true; }
        temp.isDirty = true;
        //player inventory
        temp = PlayerInventory[1].GetComponent<InventoryObject>();
        temp.possibleWeapon = IncChar.myWeapon;
        if (IncChar.myWeapon == null) { temp.isFull = false; } else { temp.isFull = true; }
        temp.isDirty = true;

        if(isNullArmororWeapon == 1)
        {
            int index = 0;
            foreach(Armour arms in BaseArmorInventory)
            {
                InventoryObject basetemp = DisplayInventory[index%15].GetComponent<InventoryObject>();
                basetemp.possibleArmor = BaseArmorInventory[index];
                basetemp.IsArmor = true; basetemp.isFull = true; basetemp.isDirty = true;
                basetemp.Index = index;
                Debug.Log(index);
                DisplayInventory[index % 15].SetActive(true);
                index++;
            }
        }
        else if(isNullArmororWeapon == 2)
        {
            int index = 0;
            foreach (Weapon arms in BaseWeaponInventory)
            {
                InventoryObject basetemp = DisplayInventory[index % 15].GetComponent<InventoryObject>();
                basetemp.possibleWeapon = BaseWeaponInventory[index];
                basetemp.IsArmor = false; basetemp.isFull = true; basetemp.isDirty = true;
                basetemp.Index = index;
                Debug.Log(index);
                DisplayInventory[index % 15].SetActive(true);
                index++;
            }
        }





    }

    public void UpdateSkillsPanel()
    {
        foreach (GameObject obby in DisplaySkills)
        {
           obby.SetActive(false);
        }

        //player inventory
        for (int i = 0; i < 3; i++)
        {
            SkillObject temp = PlayerSkills[i].GetComponent<SkillObject>();
            temp.mySkill = AccessSelectedCharacter().AvailableSkills[i];
            if (temp.mySkill == null) { temp.isFull = false; } else { temp.isFull = true; }
            temp.isDirty = true;

        }
        int index = 0;
        foreach(Skill tempSkill in SkillInventory)
        {
            Debug.Log("Activatibng skill #" + index + " of " + SkillInventory.Length);
            SkillObject basetemp = DisplaySkills[index % 10].GetComponent<SkillObject>();
            basetemp.mySkill = tempSkill;
            basetemp.isFull = true; basetemp.isDirty = true;
            basetemp.Index = index;
            DisplaySkills[index].SetActive(true);
            index++;
        }

    }


    public void UpdateCharacterSelection()
    {
        List<Character> TempList = new List<Character>();
        foreach (Character Chara in Combatants)
        {
            TempList.Add(Chara);
        }
        foreach(Character Chara in ExplorationPhase.Friends)
        {
            TempList.Add(Chara);
        }

        TempList.Sort((p1, p2) => p1.UniqueID.CompareTo(p2.UniqueID));

       foreach(Transform child in CharacterSelectionBox.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject != CharacterSelectionBox)
            {
                Destroy(child.gameObject);
            }
        }

       foreach (Character chara in TempList)
        {
            GameObject temp = Instantiate(CharacterSelectorPrefab, CharacterSelectionBox.transform);
            temp.GetComponent<Button>().onClick.AddListener(delegate { SetSelectedCharacter(chara.UniqueID); });
            temp.GetComponent<Image>().sprite = WorkIcons[(int)chara.myWork];
            chara.AvailableSkills[0].myCharacter = chara;
            chara.AvailableSkills[1].myCharacter = chara;
            chara.AvailableSkills[2].myCharacter = chara;
        }

    }

    public void UpdateIncomes()
    {
        CurrencyBoxes[0].text = (ResourceGold + " + " + Buildings[2].ManpowerLeft * 5);
        CurrencyBoxes[1].text = (ResourceConMat + " + " + Buildings[1].ManpowerLeft * 2);
        CurrencyBoxes[2].text = (ResourceOre + " + " + Buildings[1].ManpowerRight * 2);
    }

    public void TickBuildings()
    {
        ResourceGold += (Buildings[2].ManpowerLeft * 5);
        ResourceConMat += (Buildings[1].ManpowerLeft * 2);
        ResourceOre += (Buildings[1].ManpowerRight * 2);

        UpdateIncomes();
    }

    public void OnReturnFromCombat()
    {
        TickBuildings();

        for (int i = Combatants.Count-1; i > 0; i--)
        {
            if(Combatants[i].myWork == Worktype.CombatFight || Combatants[i].myWork == Worktype.CombatSupport)
            {
                GiveCharacterById(Combatants[i].UniqueID);
            }
        }
    }

}
