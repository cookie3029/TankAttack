using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private PhotonView pv;

    [SerializeField] private float turnSpeed = 20.0f;

    void Start()
    {
        pv = transform.root.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!pv.IsMine) return;

        // 메인 카메라에서 Mouse Position 위치로 발사하는 Ray 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);

        // 레이캐스팅
        if (Physics.Raycast(ray, out RaycastHit hit, math.INFINITY, 1 << 8))
        {
            // hit 월드 좌표를 터렛 기준의 로컬 좌표로 전환
            Vector3 pos = transform.InverseTransformPoint(hit.point);

            // 각도 계산 Atan(x/z) --> Atan2(x, z); 라디안 -> 오일러각 변환
            float angle = Mathf.Atan2(pos.x, pos.z) * Mathf.Rad2Deg;

            // 터렛 회전
            transform.Rotate(Vector3.up * angle * Time.deltaTime * turnSpeed);
        }
    }
}
