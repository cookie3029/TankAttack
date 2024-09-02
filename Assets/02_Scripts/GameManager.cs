using System.Collections;
using TMPro;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instanace = null;

    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text conntectInfo;
    [SerializeField] private TMP_Text msgText;

    [SerializeField] private Button sendMsgButton;
    [SerializeField] private TMP_InputField chatMsgIf;

    private PhotonView pv;

    void Awake()
    {
        Instanace = this;
    }

    IEnumerator Start()
    {
        exitButton.onClick.AddListener(() => OnExitButtonClick());
        sendMsgButton.onClick.AddListener(() => SendChatMessage());

        pv = GetComponent<PhotonView>();

        yield return new WaitForSeconds(0.5f);
        CreateTank();
        DisplayConnectInfo();
    }

    void CreateTank()
    {
        Vector3 pos = new Vector3(Random.Range(-100, 100), 30.0f, Random.Range(-100, 100));

        // 네트워크 플레이어를 생성
        PhotonNetwork.Instantiate("Tank", pos, Quaternion.identity, 0);
    }

    public void SendChatMessage()
    {
        // [Zackiller] 안녕하세요.
        string msg = $"<color=#00ff00>[{PhotonNetwork.NickName}]</color> {chatMsgIf.text}";

        DisplayMessage(msg);
        pv.RPC(nameof(DisplayMessage), RpcTarget.OthersBuffered, msg);
    }

    [PunRPC]
    public void DisplayMessage(string msg)
    {
        msgText.text += msg + "\n";
    }

    #region 사용자 정의 콜백
    // 룸을 나갔을 때 호출
    private void OnExitButtonClick()
    {
        // 방에서 나가겠다는 요청
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region 포톤 콜백함수
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    // 룸에 클라이언트가 입장했을 때
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        DisplayConnectInfo();
    }

    private void DisplayConnectInfo()
    {
        int currPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        int maxPlayer = PhotonNetwork.CurrentRoom.MaxPlayers;
        string roomName = PhotonNetwork.CurrentRoom.Name;
        string msg = $"[{roomName}] {currPlayer}/{maxPlayer}";

        conntectInfo.text = msg;
    }

    // 룸에 클라리언트가 나갔을 때
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        DisplayConnectInfo();
    }

    #endregion
}
