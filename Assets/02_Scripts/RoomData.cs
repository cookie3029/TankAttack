using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class RoomData : MonoBehaviour
{
    [SerializeField] private TMP_Text roomText;
    private RoomInfo roomInfo;

    // 프로퍼티
    // Getter, Setter
    public RoomInfo RoomInfo
    {
        // Getter
        get
        {
            return roomInfo;
        }

        // Setter
        set
        {
            roomInfo = value;

            // 룸이름 (11/20)
            roomText.text = $"{roomInfo.Name} ({roomInfo.PlayerCount}/{roomInfo.MaxPlayers})";

            // 버튼 이벤트를 연결
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => PhotonNetwork.JoinRoom(roomInfo.Name));
        }
    }
}
