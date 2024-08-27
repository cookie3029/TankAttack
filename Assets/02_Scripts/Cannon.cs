using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private float force = 1200.0f;

    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * force);
        Destroy(this.gameObject, 5.0f);
    }

    private void OnCollisionEnter(Collision coll)
    {
        Destroy(this.gameObject);
    }
}
