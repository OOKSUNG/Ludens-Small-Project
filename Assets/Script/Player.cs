using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;
using Cinemachine;
using JetBrains.Annotations;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    CapsuleCollider2D CC;
    public Rigidbody2D RB;
    public Animator AN;
    public SpriteRenderer SR;
    public PhotonView PV;
    public AudioSource AS;
    public Text NickNameText;

    //public GameObject WinPanel;
    //public GameObject LosePanel;

    public float jumpPower = 7f;
    public bool isJumping = false;
    public float maxSpeed = 0.1f;
    public bool isDie = false;
    public int health = 1;

    Vector3 curPos;

    void Awake()
    {
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        if (PV.IsMine)
        {
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }

        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        AN = GetComponent<Animator>();
        CC = GetComponent<CapsuleCollider2D>();
        PV = GetComponent<PhotonView>();
        AS = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (PV.IsMine)
        {
            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                AS.Play();
                RB.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                AN.SetBool("isJumping", true);
                AN.SetBool("isWalking", false);
                isJumping = true;
            }
            if (Input.GetAxisRaw("Horizontal") == 0 && !isJumping)
            {
                AN.SetBool("isWalking", false);
            }
            if (Input.GetButton("Horizontal"))
            {
                SR.flipX = Input.GetAxisRaw("Horizontal") == -1;
                AN.SetBool("isWalking", true);
            }
        }
        if(transform.position.y < -8f) OnDamaged();
  
    }

    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            float h = Input.GetAxisRaw("Horizontal");
            RB.velocity = new Vector2(4 * h, RB.velocity.y);

            if (h != 0)
            {
                AN.SetBool("isWalking", true);
                PV.RPC("FlipXRPC", RpcTarget.AllBuffered, h); // 재접속시 filpX를 동기화해주기 위해서 AllBuffered
            }

            RB.AddForce(Vector2.right * h, ForceMode2D.Impulse);

            if (RB.velocity.x > maxSpeed)
                RB.velocity = new Vector2(maxSpeed, RB.velocity.y);

            else if (RB.velocity.x < (-1) * maxSpeed)
                RB.velocity = new Vector2(-maxSpeed, RB.velocity.y);

            if (!isJumping)
            {
                AN.SetBool("isWalking", RB.velocity.x > 0.1f || RB.velocity.x < 0.1f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "BOX" && PV.IsMine)
        {
            AN.SetBool("isJumping", false);
            isJumping = false;
        }

        if (collision.gameObject.tag == "Check" && PV.IsMine)
        {
            if(GameManager.instance.score == GameManager.instance.coinCounts[GameManager.instance.stageNum])
            {
                photonView.RPC("RequestStageMove", RpcTarget.MasterClient);
            }
            else
            {
                // Coin을 다 먹지 못했는데, 다음레벨로 가려고 할 때 뭔가 플레이어에게 알려줄수도.
            }
        }

        if (collision.gameObject.tag == "Enemy" && PV.IsMine)
        {
            OnDamaged();
        }

        if(collision.gameObject.tag == "Coin" && PV.IsMine)
        {
            collision.gameObject.GetComponent<PhotonView>().RPC("DestroySelf", RpcTarget.All);
            photonView.RPC("RequestScoreIncrease", RpcTarget.MasterClient);
        }
    }

    void OnDamaged()
    {
        //gameObject.layer = LayerMask.NameToLayer("PlayerDamaged");
        //SR.color = new Color(1, 1, 1, 0.4f);
        //AN.SetTrigger("isHit");
        //Invoke("OffDamaged", 3);
        health--;

        if (health <= 0 && PV.IsMine)
        {
            photonView.RPC("RequestAllBack", RpcTarget.MasterClient);
        }
    }

    void OffDamaged()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        SR.color = new Color(1, 1, 1, 1);
    }

    void OnAttack(Transform enemy)
    {
        Player player = enemy.GetComponent<Player>();
        player.OnDamaged();
    }

    [PunRPC]
    void RequestStageMove()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 모든 캐릭터를 이동
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null)
            {
                pv.RPC("StageMove", RpcTarget.All, new Vector3(0, 3, 0));
            }
        }

        // 모든 클라이언트에게 다음 스테이지 생성
        photonView.RPC("StartNextStage", RpcTarget.All);
    }

    [PunRPC]
    void RequestAllBack()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 모든 캐릭터를 이동
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null)
            {
                pv.RPC("StageMove", RpcTarget.All, new Vector3(0, 3, 0));
            }
        }
    }

    [PunRPC]
    void StageMove(Vector3 newPosition)
    {
        // 모든 클라이언트에서 위치 이동
        transform.position = newPosition;

        // 비활성화/활성화 대신 필요한 경우 다른 방식으로 처리
        // 예: 애니메이션 재생, 리스폰 효과 등
    }

    [PunRPC]
    void StartNextStage()
    {
        GameManager.instance.MakeNextStage();
    }

    [PunRPC]
    void FlipXRPC(float axis) => SR.flipX = axis == -1;

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    [PunRPC]
    void RequestScoreIncrease()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameManager.instance.score++;
        photonView.RPC("SyncScore", RpcTarget.All, GameManager.instance.score);
    }

    [PunRPC]
    void SyncScore(int syncedScore)
    {
        GameManager.instance.score = syncedScore;
        GameManager.instance.UpdateScoreUI();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

}