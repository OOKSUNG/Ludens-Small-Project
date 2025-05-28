using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public NetworkManager nM;
    public GameObject StageCanvas;
    public Text stageText;
    public Text scoreText;
    public GameObject[] Stages;
    public int stageNum = 0;
    public int lastStage = 10;
    public int score = 0;
    public int[] coinCounts = new int[] { 3, 3, 3, 3 };

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
        StageCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(nM.isStart) StageCanvas.SetActive(true);
        stageText.text = "Stage :" + (stageNum + 1);
        scoreText.text = score.ToString() + "/" + coinCounts[stageNum];
    }

    public void MakeNextStage()
    {
        if (stageNum != lastStage)
        {
            Stages[stageNum++].SetActive(false);
            Stages[stageNum].SetActive(true);

            score = 0;
        }
    }

}
