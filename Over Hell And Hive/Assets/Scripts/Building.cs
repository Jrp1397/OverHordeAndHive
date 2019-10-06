using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private BaseManager myBase;

    // Start is called before the first frame update
    void Start()
    {
        myBase = gameObject.GetComponent<BaseManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
