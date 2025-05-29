using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject GO;
    public bool isAppear = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(!isAppear)GO.SetActive(false);
        else GO.SetActive(true);
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (!isAppear) GO.SetActive(true);
        else GO.SetActive(false);
    }
}
