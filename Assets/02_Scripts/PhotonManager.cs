using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 게임 버전 v1.0, 2.0 --
    [SerializeField] private const string version = "1.0";
    // 유저명
    [SerializeField] private string nickName = "ShibaDog";

    void Awake()
    {
        // 게임 버전 설정
        PhotonNetwork.GameVersion = version;
        // 유저명 설정
        PhotonNetwork.NickName = nickName;
        // 방장이 씬을 로딩했을 때 클라이언트에 자동으로 해당 씬이 로딩되는 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        // 포톤 서버에 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤 서버에 접속되었을 때 호출되는 콜백
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속 완료");

        // 로비 입장 요청
        PhotonNetwork.JoinLobby();
    }

    // 로비에 입장했을 때 호출되는 콜백
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 완료");

        // 랜덤한 방을 선택해서 입장하도록 요청
        PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤 방입장 실패시 호출되는 콜백
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"방입장 실패 : {message}");

        // 방을 생성
        // 룸 옵션
        RoomOptions ro = new RoomOptions
        {
            MaxPlayers = 20,
            IsOpen = true,
            IsVisible = true
        };

        PhotonNetwork.CreateRoom("MyRoom", ro);
    }

    // 방 생성 완료 콜백
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 완료");
    }

    // 방 입장후 콜백
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");

        // 네트워크 플레이어를 생성

    }

}
