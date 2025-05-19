using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] Stages;
    public int stageNum = 0;
    public int lastStage = 10;

    //ΩÃ±€≈Ê
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeNextStage()
    {
        if (stageNum != lastStage)
        {
            Stages[stageNum++].SetActive(false);
            Stages[stageNum].SetActive(true);
        }
    }



}
