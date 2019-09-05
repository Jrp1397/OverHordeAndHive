using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started up");
        Debug.Log(SceneManager.sceneCount);
      


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchToManagement()
    {

        Debug.Log("switching to management");
        SceneManager.LoadSceneAsync("BaseManagement");

        Debug.Log(SceneManager.sceneCount);
    }

    public void SwitchToCombat()
    {
        SceneManager.LoadSceneAsync("CombatScene");
        Debug.Log("switching to combat");
    }

}
