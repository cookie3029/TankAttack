using UnityEngine;
using Unity.Cinemachine;
using Photon.Pun;
using TMPro;
using System;

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

    void Awake()
    {
        cvc = GameObject.Find("CinemachineCamera").GetComponent<CinemachineCamera>();

        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        pv = GetComponent<PhotonView>();

        nickName = transform.Find("Canvas/Panel/NickName").GetComponent<TMP_Text>();

        nickName.text = pv.Owner.NickName;

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
            pv.RPC(nameof(Fire), RpcTarget.AllViaServer);
        }
    }

    void Locomotion()
    {
        tr.Translate(Vector3.forward * Time.deltaTime * v * moveSpeed); // 이동
        tr.Rotate(Vector3.up * Time.deltaTime * h * turnSpeed);         // 회전
    }

    // RPC 정의
    [PunRPC]
    void Fire()
    {
        audio.PlayOneShot(fireSfx, 0.8f);
        Instantiate(cannonPrefab, firePos.position, firePos.rotation);
    }
}
