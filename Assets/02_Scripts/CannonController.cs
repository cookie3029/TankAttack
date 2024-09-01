using Photon.Pun;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    private PhotonView pv;

    private float r => Input.GetAxis("Mouse ScrollWheel");

    [SerializeField] private float speed = 10.0f;

    void Start()
    {
        pv = transform.root.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!pv.IsMine) return;

        transform.Rotate(Vector3.right * Time.deltaTime * r * speed);
    }
}
