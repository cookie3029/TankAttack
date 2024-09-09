using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("Game Settings")]
    [SerializeField] private const string version = "1.0";
    [SerializeField] private string nickName = "ShibaDog";

    [Header("UI")]
    [SerializeField] private TMP_InputField nickNameIf;
    [SerializeField] private TMP_InputField roomNameIf;

    [Header("Button")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button makeRoomButton;

    [Header("RoomList")]
    // 룸 프리팹
    public GameObject roomPrefab;

    // 룸 프리팹을 생성할 부모 객체
    public Transform contentTr;

    // 품 목록을 저장하기 위한 딕셔너리
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();


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
        makeRoomButton.onClick.AddListener(() => OnMakeRoomButtonClick());
    }

    private void OnMakeRoomButtonClick()
    {
        // 닉네임 여부 확인
        SetNickName();

        // 룸 이름 입력 여부 확인
        if (string.IsNullOrEmpty(roomNameIf.text))
        {
            roomNameIf.text = $"Room_{Random.Range(0, 1000)}:0000";
        }

        // 룸 속성 정의
        RoomOptions ro = new RoomOptions();

        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;

        // 룸 생성
        PhotonNetwork.CreateRoom(roomNameIf.text, ro);
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

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            Debug.Log($"{room.Name} {room.PlayerCount}/{room.MaxPlayers}");

            // 삭제된 룸
            if (room.RemovedFromList)
            {
                // 품 삭제
                if (roomDict.TryGetValue(room.Name, out GameObject tempRoom))
                {
                    // room 프리팹 클론을 삭제
                    Destroy(tempRoom);

                    // 딕셔너리의 레코드를 삭제
                    roomDict.Remove(room.Name);
                }

                continue;
            }

            // 새로 생성된 룸, 변경된 경우
            if (roomDict.ContainsKey(room.Name) == false)
            {
                // 처음 생성된 룸
                var _room = Instantiate(roomPrefab, contentTr);

                // RoomPrefab에 RoomInfo 값을 저장
                _room.GetComponent<RoomData>().RoomInfo = room;

                roomDict.Add(room.Name, _room);
            }
            else
            {
                // 이전에 생성되었던 룸
                // 룸 정보 갱신
                if (roomDict.TryGetValue(room.Name, out GameObject tempRoom))
                {
                    tempRoom.GetComponent<RoomData>().RoomInfo = room;
                }
            }
        }
    }
}
