using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public InputField NickNameInput;
    public GameObject DisconnectPanel;
    //�÷��̾� ���帶�� ĳ���� �ٸ���
    public int PlayerDivision = 1;
    public bool isStart = false;


    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
    }

    public override void OnJoinedRoom()
    {
        isStart = true;
        GameManager.instance.StageCanvas.SetActive(true);
        DisconnectPanel.SetActive(false);
        int division = PhotonNetwork.CurrentRoom.PlayerCount % 2 == 0 ? -1 : 1;
        Spawn(division);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsConnected) { PhotonNetwork.Disconnect(); }
    }

    public void Spawn(int division)
    {
        if(division == 1)PhotonNetwork.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);
        else PhotonNetwork.Instantiate("Player0", new Vector3(0, 0, 0), Quaternion.identity);
    }
    public void QuitGame()
    {
        Application.Quit();
        // �����Ϳ����� ���ᰡ �ȵǹǷ� �α� ��¿� �ڵ� �߰� (������)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    void GameEnd()
    {
        
        //RespawnPanel.SetActive(true);
    }
    public override void OnDisconnected(DisconnectCause cause) => DisconnectPanel.SetActive(true);//RespawnPanel.SetActive(false);

}
