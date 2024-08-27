using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    private float r => Input.GetAxis("Mouse ScrollWheel");

    void Start()
    {

    }

    void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * r * speed);
    }
}
