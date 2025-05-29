using Photon.Pun;
using UnityEngine;

public class OnlyOneThroughWall : MonoBehaviourPun
{
    public int PossibleActorNumber = 1;
    private Collider2D wallCollider;

    private void Awake()
    {
        wallCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PhotonView pv = collision.collider.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            bool shouldIgnore = pv.Owner.ActorNumber == PossibleActorNumber;
            Physics2D.IgnoreCollision(wallCollider, collision.collider, shouldIgnore);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PhotonView pv = collision.collider.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            Physics2D.IgnoreCollision(wallCollider, collision.collider, false);
        }
    }

    [PunRPC]
    public void TogglePassablePlayer()
    {
        PossibleActorNumber = PossibleActorNumber == 1 ? 2 : 1;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = PossibleActorNumber == 1 ? Color.red : Color.blue;
        }

        // 모든 플레이어에 대해 충돌 여부 갱신
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null)
            {
                Collider2D playerCol = player.GetComponent<Collider2D>();
                if (playerCol != null && wallCollider != null)
                {
                    bool shouldIgnore = pv.Owner.ActorNumber == PossibleActorNumber;
                    Physics2D.IgnoreCollision(wallCollider, playerCol, shouldIgnore);
                }
            }
        }
    }
}
