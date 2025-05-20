using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UIElements;

public class SpikeMob : MonoBehaviour, IPunObservable
{
    public bool isX = false;
    public float Pos = 0f;
    public float Max = 0f;
    public float Min = 0f;
    public float speed = 1.5f;
    private float Length;

    private PhotonView pv;
    private Vector3 networkPosition;

    // Start is called before the first frame update
    void Awake()
    {
        pv = GetComponent<PhotonView>();
        Length = Mathf.Abs(Max - Min);
        networkPosition = transform.position;
    }

    // Update is called once per frame

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (Length == 0)
                return;

            float h = Mathf.PingPong(Time.time * speed, Length) + Mathf.Min(Min, Max);
            if (!float.IsNaN(h))
            {
                if (isX)
                    transform.position = new Vector3(h, Pos, 0);
                else
                    transform.position = new Vector3(Pos, h, 0);
            }
        }
        else
        {
            // 부드럽게 따라가도록 보간
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
        }
    }

    // 위치 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 내 데이터 보냄
        {
            stream.SendNext(transform.position);
        }
        else // 다른 플레이어의 데이터 수신
        {
            networkPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
