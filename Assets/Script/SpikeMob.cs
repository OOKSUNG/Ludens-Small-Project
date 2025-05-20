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
            // �ε巴�� ���󰡵��� ����
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
        }
    }

    // ��ġ ����ȭ
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // �� ������ ����
        {
            stream.SendNext(transform.position);
        }
        else // �ٸ� �÷��̾��� ������ ����
        {
            networkPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
