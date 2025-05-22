using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject GO;


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
        GO.SetActive(false);
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        GO.SetActive(true);
    }
}
