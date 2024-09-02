using UnityEngine;
using Unity.Cinemachine;
using Photon.Pun;
using TMPro;
using System;
using Photon.Realtime;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TankController : MonoBehaviour
{
    private CinemachineCamera cvc;

    private Transform tr;
    private Rigidbody rb;
    private new AudioSource audio;
    private PhotonView pv;

    private float v => Input.GetAxis("Vertical");
    private float h => Input.GetAxis("Horizontal");
    private bool isFire => Input.GetMouseButtonDown(0);

    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float turnSpeed = 100.0f;

    public GameObject cannonPrefab;
    public Transform firePos;
    public AudioClip fireSfx;

    [NonSerialized]
    public TMP_Text nickName;

    private float initHp = 100.0f;
    private float currHp = 100.0f;

    private MeshRenderer[] renderers;

    public Image hpBar;

    void Awake()
    {
        cvc = GameObject.Find("CinemachineCamera").GetComponent<CinemachineCamera>();

        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();

        nickName = transform.Find("Canvas/Panel/NickName").GetComponent<TMP_Text>();

        nickName.text = pv.Owner.NickName;

        renderers = GetComponentsInChildren<MeshRenderer>();

        // 카메라 연결
        if (pv.IsMine == true)
        {
            cvc.Follow = tr;
            cvc.LookAt = tr;
        }
        else
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        if (!pv.IsMine) return;

        Locomotion();

        if (isFire)
        {
            // RPC 함수 호출
            pv.RPC(nameof(Fire), RpcTarget.AllViaServer, pv.OwnerActorNr);
        }
    }

    void Locomotion()
    {
        tr.Translate(Vector3.forward * Time.deltaTime * v * moveSpeed); // 이동
        tr.Rotate(Vector3.up * Time.deltaTime * h * turnSpeed);         // 회전
    }

    // RPC 정의
    [PunRPC]
    void Fire(int actorNumber)
    {
        audio.PlayOneShot(fireSfx, 0.8f);

        var cannon = Instantiate(cannonPrefab, firePos.position, firePos.rotation);

        cannon.GetComponent<Cannon>().shooterID = actorNumber;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("CANNON"))
        {
            // ActorNumber => NickName
            int actorNumber = coll.gameObject.GetComponent<Cannon>().shooterID;

            Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNumber);

            currHp -= 20.0f;

            // HpBar 설정
            hpBar.fillAmount = currHp / initHp;

            if (currHp <= 0)
            {
                string msg = $"<color=#00ff00>{pv.Owner.NickName}</color>님은 사망했습니다. 막타는 <color=#ff0000>{player.NickName}</color>!";
                GameManager.Instanace.DisplayMessage(msg);

                SetVisibleTank(false);
                Invoke(nameof(RespawnTank), 3.0f);
            }
        }
    }

    void RespawnTank()
    {
        currHp = initHp;
        hpBar.fillAmount = 1.0f;
        SetVisibleTank(true);
    }

    void SetVisibleTank(bool IsVisible)
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = IsVisible;
        }

        tr.Find("Canvas").gameObject.SetActive(IsVisible);
    }
}
