using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("Game Settings")]
    [SerializeField] private const string version = "1.0";
    [SerializeField] private string nickName = "ShibaDog";

    [Header("UI")]
    [SerializeField] private TMP_InputField nickNameIf;

    [Header("Button")]
    [SerializeField] private Button loginButton;


    void Awake()
    {
        // 게임 버전 설정
        PhotonNetwork.GameVersion = version;

        // 방장이 씬을 로딩했을 때 클라이언트에 자동으로 해당 씬이 로딩되는 옵션
        PhotonNetwork.AutomaticallySyncScene = true;

        // 포톤 서버에 접속
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void Start()
    {
        // 지정된 닉네임이 있을 경우 출력
        if (PlayerPrefs.HasKey("NICK_NAME"))
        {
            nickName = PlayerPrefs.GetString("NICK_NAME");
            nickNameIf.text = nickName;
        }

        SetNickName();
        loginButton.onClick.AddListener(() => OnLoginButtonClick());
    }

    private void SetNickName()
    {
        // 닉네임에 비어있는지 확인
        if (string.IsNullOrEmpty(nickNameIf.text))
        {
            // 닉네임 랜덤하게 설정
            nickName = $"User_{Random.Range(0, 1000):0000}";
            nickNameIf.text = nickName;
        }

        nickName = nickNameIf.text;

        // 포톤의 닉네임설정
        PhotonNetwork.NickName = nickName;
    }

    public void OnLoginButtonClick()
    {
        SetNickName();

        // 닉네임을 짖어
        PlayerPrefs.SetString("NICK_NAME", nickName);
        PhotonNetwork.JoinRandomRoom();
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
        // PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤 방입장 실패시 호출되는 콜백
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"방입장 실패 : {message}");

        // 방을 생성
        // 룸 옵션
        RoomOptions ro = new RoomOptions
        {
            MaxPlayers = 100,
            IsOpen = true,
            IsVisible = true
        };

        PhotonNetwork.CreateRoom("MyRoom" + Random.Range(0, 100), ro);
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

        if (PhotonNetwork.IsMasterClient)
        {
            // 전투 씬을 로딩
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

}
