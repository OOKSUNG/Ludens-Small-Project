using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class OnlyOneThroughWall : MonoBehaviourPun
{
    public int PossibleActorNumber = 1;
    private void OnTriggerEnter2D(Collider2D other)
    {
        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            if (pv.Owner.ActorNumber == PossibleActorNumber)
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, true);
            else
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other, false);
        }
    }

    [PunRPC]
    public void TogglePassablePlayer()
    {
        PossibleActorNumber = PossibleActorNumber == 1 ? 2 : 1;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = PossibleActorNumber == 1 ? Color.red : Color.blue;
    }
}
