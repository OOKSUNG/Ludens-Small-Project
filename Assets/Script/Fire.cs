using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Animator AN;
    public SpriteRenderer SR;
    public bool isOn = true;


    // Start is called before the first frame update
    void Start()
    {
        AN = GetComponent<Animator>();
        AN.SetBool("isOn", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOn) { Off();}
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "BOX")
        {
            isOn = false;
        }
    }
    void Off()
    {
        AN.SetBool("isOn", false);
        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged");
    }
}
