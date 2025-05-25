using UnityEngine;
using Photon.Pun;

public class ToggleWallButton : MonoBehaviour
{
    public OnlyOneThroughWall targetWall;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            // 버튼을 밟은 플레이어가 벽 상태를 변경
            targetWall.photonView.RPC("TogglePassablePlayer", RpcTarget.AllBuffered);
        }
    }
}