using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encounter : MonoBehaviour
{
    [SerializeField] private List<Character> Friends;
    [SerializeField] private List<Monster> Foes;
    [SerializeField] private BaseManager HomeBase;
    public GameObject FieldTilePrefab;
    private GameObject[,] BattleField = new GameObject [4,12];
    public int SelectedFoeIndex = 0, SelectedFriendIndex = 0;
    [SerializeField] private List<int> Seed;
    public int DangerSeedModifer;
    [SerializeField] private Text FoeName, FriendName;


    // Start is called before the first frame update
    void Start()
    {
        GenerateField();
        foreach (GameObject obby in BattleField)
        {
            obby.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void EnableMap()
    {
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

    public void GenerateAttackFriendly(int incID)
    {
        foreach(Character chara in Friends)
        {
            if (chara.UniqueID == incID)
            {
                chara.GenerateAttack();
            }
        }
    }

}
