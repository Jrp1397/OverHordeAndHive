using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    [SerializeField] private List<Character> Combatants;
    [SerializeField] private BaseManager HomeBase;
    public GameObject FieldTilePrefab;
    private GameObject[,] BattleField = new GameObject [4,12];

    // Start is called before the first frame update
    void Start()
    {
        GenerateField();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GiveCharacterById(int incID)
    {
        Character temp = null;
        for (int i = Combatants.Count - 1; i >= 0; i--)
        {
            if (Combatants[i].UniqueID == incID)
            {
                temp = Combatants[i];
                Combatants.RemoveAt(i);
                break;
            }
        }
        HomeBase.TakeCharacter(temp);
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

    public void TickCharacters()
    {
        foreach(Character chara in Combatants)
        {
            chara.AlterStats();
        }
    }

    public void DisplayCharacters()
    {
        foreach (Character chara in Combatants)
        {
            chara.PingStrength();
        }
    }

   void OnDisable()
    {
        foreach (GameObject obby in BattleField)
        {
            obby.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (BattleField[0,0] == null) { return; }
        foreach (GameObject obby in BattleField)
        {
           
            obby.SetActive(true);
        }
    }

    public void GenerateField()
    {
        Vector3 tempPos = new Vector3(-6.275f, -1.05f, 0);
        for (int i = 0; i < 4; i++)
        {
            tempPos.x = -5.775f;
            for (int j = 0; j < 12; j++)
            {
                BattleField[i,j] = Instantiate(FieldTilePrefab, tempPos, Quaternion.identity, gameObject.transform);                
                tempPos.x += 1.05f;
            }
            tempPos.y += 1.05f;
        }
    }
}
