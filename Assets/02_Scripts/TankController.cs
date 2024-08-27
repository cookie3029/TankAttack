using UnityEngine;

public class TankController : MonoBehaviour
{
    private Transform tr;
    private Rigidbody rb;

    private float v => Input.GetAxis("Vertical");
    private float h => Input.GetAxis("Horizontal");
    private bool isFire => Input.GetMouseButtonDown(0);

    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float turnSpeed = 100.0f;

    public GameObject cannonPrefab;
    public Transform firePos;

    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Locomotion();

        if (isFire)
        {
            Instantiate(cannonPrefab, firePos.position, firePos.rotation);
        }
    }

    void Locomotion()
    {
        tr.Translate(Vector3.forward * Time.deltaTime * v * moveSpeed); // 이동
        tr.Rotate(Vector3.up * Time.deltaTime * h * turnSpeed);         // 회전
    }
}
