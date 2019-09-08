using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public List<GameObject> ToggleOff;
    public List<GameObject> ToggleOn;
    private bool Toggled = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BiDirectionalToggle()//switches to on or off.
    {
       
       foreach (GameObject obby in ToggleOff)
       {
           obby.SetActive(!Toggled);
       }
       foreach (GameObject obby in ToggleOn)
       {
           obby.SetActive(Toggled);
       }
       Toggled = !Toggled;
       
    }




    public void Toggle()//switches off-list to off, and on-list to on
    {
        foreach(GameObject obby in ToggleOff)
        {
            obby.SetActive(false);
        }
        foreach (GameObject obby in ToggleOn)
        {
            obby.SetActive(true);
        }
    }

    public void ReverseToggle()// Switches off-list back on, and On-list to off.
    {
        foreach (GameObject obby in ToggleOff)
        {
            obby.SetActive(true);
        }
        foreach (GameObject obby in ToggleOn)
        {
            obby.SetActive(false);
        }
    }

}
