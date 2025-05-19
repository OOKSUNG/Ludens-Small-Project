using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;
using Cinemachine;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    CapsuleCollider2D CC;
    public Rigidbody2D RB;
    public Animator AN;
    public SpriteRenderer SR;
    public PhotonView PV;
    public Text NickNameText;

    //public GameObject WinPanel;
    //public GameObject LosePanel;

    public float jumpPower = 7f;
    public bool isJumping = false;
    public float maxSpeed = 0.1f;
    public bool isDie = false;
    public int health = 3;

    Vector3 curPos;

    void Awake()
    {
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        if (PV.IsMine)
        {
            //var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            //CM.Follow = transform;
            //CM.LookAt = transform;
        }

        RB = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        AN = GetComponent<Animator>();
        CC = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        if (PV.IsMine)
        {
            if (Input.GetButtonDown("Jump") && !isJumping)
            {
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
  
    }

    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            float h = Input.GetAxisRaw("Horizontal");
            RB.velocity = new Vector2(4 * h, RB.velocity.y);

            if (h != 0)
            {
                AN.SetBool("walk", true);
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
        if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "Player" && PV.IsMine)
        {
            AN.SetBool("isJumping", false);
            isJumping = false;
        }

    }

    void OnDamaged()
    {
        gameObject.layer = LayerMask.NameToLayer("PlayerDamaged");

        SR.color = new Color(1, 1, 1, 0.4f);


        AN.SetTrigger("doDamaged");

        Invoke("OffDamaged", 3);
        health--;

        if (health <= 0 && PV.IsMine)
        {
            GameObject.Find("Canvas").transform.Find("health 1").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
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
    void FlipXRPC(float axis) => SR.flipX = axis == -1;

    [PunRPC]
    void JumpRPC()
    {
        RB.velocity = Vector2.zero;
        RB.AddForce(Vector2.up * 700);
    }



    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

}