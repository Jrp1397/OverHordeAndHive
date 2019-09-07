using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public List<GameObject> ToggleOff;
    public List<GameObject> ToggleOn;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Toggle()
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

    public void ReverseToggle()
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
